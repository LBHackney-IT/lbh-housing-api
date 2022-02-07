using Amazon.S3;
using Amazon.S3.Model;
using HousingRegisterApi.V1.Domain.Report;
using HousingRegisterApi.V1.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HousingRegisterApi.V1.Gateways
{
    public class FileExportGateway : IFileGateway
    {
        private readonly ILogger<FileExportGateway> _logger;
        private readonly IAmazonS3 _amazonS3;
        private readonly string _bucketName;

        public FileExportGateway(
            ILogger<FileExportGateway> logger,
            IAmazonS3 amazonS3)
        {
            _logger = logger;
            _amazonS3 = amazonS3;
            _bucketName = Environment.GetEnvironmentVariable("HOUSINGREGISTER_EXPORT_BUCKET_NAME");
        }

        public async Task<ExportFile> GetFile(string fileName, string parentFolderName = "")
        {
            _logger.LogInformation($"Attempting to retrieve {fileName} from bucket {_bucketName}");

            string fileKey = GetFileKey(fileName, parentFolderName);
            var s3Object = await GetS3Object(fileKey).ConfigureAwait(false);
            var s3ObjectTags = await GetS3ObjectTags(fileKey).ConfigureAwait(false);

            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                s3Object.ResponseStream.CopyTo(ms);
                data = ms.ToArray();
            };

            var file = new ExportFile(fileName, s3Object.Headers.ContentType, data)
            {
                Attributes = s3ObjectTags.ToDictionary()
            };

            return file;
        }

        public async Task<List<ExportFileItem>> ListFiles(string parentFolderName = "")
        {
            _logger.LogInformation($"Attempting to list files from bucket {_bucketName}");

            var files = new List<ExportFileItem>();

            var s3Objects = await GetS3Objects(parentFolderName).ConfigureAwait(false);

            foreach (var s3Object in s3Objects)
            {
                var s3ObjectTags = (await GetS3ObjectTags(s3Object.Key).ConfigureAwait(false));
                var fileName = Path.GetFileName(s3Object.Key);

                files.Add(new ExportFileItem(fileName)
                {
                    LastModified = s3Object.LastModified,
                    Size = s3Object.Size,
                    Attributes = s3ObjectTags.ToDictionary()
                });
            }

            return files;
        }

        public async Task SaveFile(ExportFile file, string parentFolderName = "")
        {
            _logger.LogInformation($"Attempting to save {file.FileName} to bucket {_bucketName}");
            string fileKey = GetFileKey(file.FileName, parentFolderName);
            await PutS3Object(file, fileKey).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, string>> GetAttributes(string fileName, string parentFolderName = "")
        {
            _logger.LogInformation($"Attempting to get {fileName} attributes");
            string fileKey = GetFileKey(fileName, parentFolderName);
            var s3ObjectTags = await GetS3ObjectTags(fileKey).ConfigureAwait(false);
            return s3ObjectTags.ToDictionary();
        }

        public async Task UpdateAttributes(string fileName, Dictionary<string, string> attributes, string parentFolderName = "")
        {
            _logger.LogInformation($"Attempting to set {fileName} attributes");
            string fileKey = GetFileKey(fileName, parentFolderName);
            await PutS3ObjectTags(attributes, fileKey).ConfigureAwait(false);
        }

        private async Task<List<S3Object>> GetS3Objects(string parentFolderName = "")
        {
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = string.IsNullOrWhiteSpace(parentFolderName) ? _bucketName : parentFolderName.ToUpper() + "/"
            };

            var response = await _amazonS3.ListObjectsV2Async(request).ConfigureAwait(false);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError($"Unable to get files in {parentFolderName}");
            }

            return response.S3Objects;
        }

        private async Task<GetObjectResponse> GetS3Object(string key)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _amazonS3.GetObjectAsync(request).ConfigureAwait(false);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError($"Unable to get file {key}");
            }

            return response;
        }

        private async Task PutS3Object(ExportFile file, string fileKey)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
                FilePath = "",
                ContentType = file.FileMimeType,
                InputStream = new MemoryStream(file.Data),
                TagSet = file.Attributes.ToTagList()
            };

            var response = await _amazonS3.PutObjectAsync(request).ConfigureAwait(false);
            _logger.LogError($"File {fileKey} saved to {_bucketName} status code {response.HttpStatusCode}");

            var json = new JavaScriptSerializer().Serialize(response);
            _logger.LogError(json);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError($"Unable to save file {fileKey} to {_bucketName}");
            }
        }

        private async Task<List<Tag>> GetS3ObjectTags(string key)
        {
            GetObjectTaggingRequest request = new GetObjectTaggingRequest
            {
                BucketName = _bucketName,
                Key = key,
            };

            var response = await _amazonS3.GetObjectTaggingAsync(request).ConfigureAwait(false);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError($"Unable to get tags for {key}");
            }

            return response.Tagging;
        }

        private async Task PutS3ObjectTags(Dictionary<string, string> attributes, string key)
        {
            // get existing tags
            var s3ObjectTags = await GetS3ObjectTags(key).ConfigureAwait(false);

            PutObjectTaggingRequest request = new PutObjectTaggingRequest
            {
                BucketName = _bucketName,
                Key = key,
                Tagging = new Tagging()
                {
                    TagSet = s3ObjectTags.AppendAttributes(attributes)
                }
            };

            if (s3ObjectTags.Any() || request.Tagging.TagSet.Any())
            {
                var response = await _amazonS3.PutObjectTaggingAsync(request).ConfigureAwait(false);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Unable to set tags for {key}");
                }
            }
            else
            {
                _logger.LogInformation($"No tags to update for {key}");
            }
        }

        private static string GetFileKey(string fileName, string parentFolderName = "")
        {
            string key = fileName;

            if (!string.IsNullOrWhiteSpace(parentFolderName))
            {
                key = $"{parentFolderName.ToUpper()}/{fileName}";
            }

            return key;
        }
    }
}

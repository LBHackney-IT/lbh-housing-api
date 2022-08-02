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
using System.Text.Json;

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

        public List<ExportFileItem> ListFiles(string parentFolderName = "", int numberOfMonthsHistory = 3)
        {
            _logger.LogInformation($"Attempting to list files from bucket {_bucketName}");

            var files = new List<ExportFileItem>();

            var s3Objects = GetNovaletS3Objects(parentFolderName, numberOfMonthsHistory);

            foreach (var s3Object in s3Objects)
            {
                var fileName = Path.GetFileName(s3Object.Key);

                files.Add(new ExportFileItem(fileName)
                {
                    LastModified = s3Object.LastModified,
                    Size = s3Object.Size,
                    Attributes = new Dictionary<string, string>()
                });
            }

            return files.OrderByDescending(x => x.LastModified).ToList();
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

        private List<S3Object> GetNovaletS3Objects(string parentFolderName = "", int numberToReturn = 3)
        {
            numberToReturn = Math.Max(numberToReturn, 3);//Limit the number of months to look back on to 3   
            List<S3Object> s3Blobs = new List<S3Object>();
            List<Task> s3Tasks = new List<Task>();

            List<string> datePrefixes = new List<string>();
            for (int i = 0; i < numberToReturn; i++)
            {
                DateTime currentDate = DateTime.UtcNow.AddMonths(i * -1);
                datePrefixes.Add($"LBH-APPLICANT FEED-{currentDate.Year}{currentDate.Month.ToString().PadLeft(2, '0')}");
            }

            foreach (var datePrefix in datePrefixes)
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    MaxKeys = numberToReturn,
                    Prefix = string.IsNullOrWhiteSpace(parentFolderName) ? _bucketName : parentFolderName.ToUpper() + "/" + datePrefix
                };

                s3Tasks.Add(_amazonS3.ListObjectsV2Async(request));
            }

            //Wait for the parallel tasks to complete
            Task.WaitAll(s3Tasks.ToArray());

            foreach (var s3Task in s3Tasks)
            {
                Task<ListObjectsV2Response> listResponseTask = (Task<ListObjectsV2Response>) s3Task;

                if (listResponseTask.Result.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Unable to get files in {parentFolderName}");
                }
                else
                {
                    s3Blobs.AddRange(listResponseTask.Result.S3Objects);
                }
            }

            return s3Blobs;
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
            _logger.LogInformation($"File {fileKey} saved to {_bucketName} status code {response.HttpStatusCode}");
            _logger.LogInformation(_bucketName);
            _logger.LogInformation(fileKey);
            _logger.LogInformation(file.FileMimeType);
            var json = JsonSerializer.Serialize(response);

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

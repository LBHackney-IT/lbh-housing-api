using Amazon.S3;
using Amazon.S3.Model;
using HousingRegisterApi.V1.Domain.FileExport;
using HousingRegisterApi.V1.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ExportFile> GetFile(string fileName)
        {
            _logger.LogInformation($"Attempting to retrieve {fileName} from bucket {_bucketName}");

            var response = await GetAwsFile(fileName).ConfigureAwait(false);
            var tags = await GetAwsFileTags(fileName).ConfigureAwait(false);

            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                response.ResponseStream.CopyTo(ms);
                data = ms.ToArray();
            };

            var file = new ExportFile(response.Key, response.Headers.ContentType, data)
            {
                Attributes = tags.Tagging.ToDictionary()
            };

            return file;
        }

        public async Task<List<string>> ListFiles()
        {
            _logger.LogInformation($"Attempting to list files from bucket {_bucketName}");

            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = _bucketName
            };

            var response = await _amazonS3.ListObjectsAsync(request).ConfigureAwait(false);

            return response.S3Objects.Select(x => x.Key).ToList();
        }

        public async Task SaveFile(ExportFile file)
        {
            _logger.LogInformation($"Attempting to save {file.FileName} to bucket {_bucketName}");

            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = file.FileName,
                FilePath = "",
                ContentType = file.FileMimeType,
                InputStream = new MemoryStream(file.Data),
                TagSet = file.Attributes.ToTagList()
            };

            await _amazonS3.PutObjectAsync(request).ConfigureAwait(false);
        }

        public async Task UpdateAttributes(string fileName, Dictionary<string, string> attributes)
        {
            _logger.LogInformation($"Attempting to set {fileName} attributes");

            if (attributes?.Any() == true)
            {
                var currentTagsResponse = await GetAwsFileTags(fileName).ConfigureAwait(false);

                PutObjectTaggingRequest request = new PutObjectTaggingRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    Tagging = new Tagging()
                    {
                        TagSet = attributes.AppendTags(currentTagsResponse.Tagging)
                    }
                };

                await _amazonS3.PutObjectTaggingAsync(request).ConfigureAwait(false);
            }
        }

        private async Task<GetObjectResponse> GetAwsFile(string fileName)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
            };

            return await _amazonS3.GetObjectAsync(request).ConfigureAwait(false);
        }

        private async Task<GetObjectTaggingResponse> GetAwsFileTags(string fileName)
        {
            GetObjectTaggingRequest request = new GetObjectTaggingRequest
            {
                BucketName = _bucketName,
                Key = fileName,
            };

            return await _amazonS3.GetObjectTaggingAsync(request).ConfigureAwait(false);
        }
    }
}

using Amazon.S3;
using Amazon.S3.Model;
using HousingRegisterApi.V1.Domain.FileExport;
using HousingRegisterApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public class FileExportGateway : IFileGateway
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly string _bucketName;

        public FileExportGateway(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
            _bucketName = Environment.GetEnvironmentVariable("HOUSINGREGISTER_EXPORT_BUCKET_NAME");
        }

        public async Task<ExportFile> GetFile(string fileName)
        {
            var response = await GetAwsFile(fileName).ConfigureAwait(false);
            var tags = await GetAwsFileTags(fileName).ConfigureAwait(false);

            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                response.ResponseStream.CopyTo(ms);
                data = ms.ToArray();
            };

            var file = new ExportFile(response.Key, response.ContentRange, data);
            file.Metadata = tags.Tagging.ToDictionary();

            return file;
        }

        public async Task SaveFile(ExportFile file)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = file.FileName,
                FilePath = "",
                ContentType = file.FileMimeType,
                InputStream = new MemoryStream(file.Data),
                TagSet = file.Metadata.ToTagList()
            };

            await _amazonS3.PutObjectAsync(request).ConfigureAwait(false);
        }

        public async Task UpdateMetadata(string fileName, Dictionary<string, string> metadata)
        {
            if (metadata?.Any() == true)
            {
                var currentTagsResponse = await GetAwsFileTags(fileName).ConfigureAwait(false);

                PutObjectTaggingRequest request = new PutObjectTaggingRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    Tagging = new Tagging()
                    {
                        TagSet = metadata.AppendTags(currentTagsResponse.Tagging)
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

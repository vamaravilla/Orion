using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SevenDays.Util
{
    public class StorageAccountService
    {
        private string AzureAzureStorageConnectionString;
        private readonly IConfiguration Configuration;
        public StorageAccountService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Upload image to blobstorage
        /// </summary>
        /// <param name="image">File image</param>
        /// <param name="name">display name</param>
        /// <returns>Upload result</returns>
        public async Task<StorageResult> UploadImageToStorage(IFormFile image,string name)
        {
            StorageResult result = new StorageResult();
            try
            {
                // Get Azure storage connection string from appsettings
                AzureAzureStorageConnectionString = ConfigurationExtensions.GetConnectionString(this.Configuration, "AzureStorageConnectionString");

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(AzureAzureStorageConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("moviesimages");

                var picBlob = cloudBlobContainer.GetBlockBlobReference($"{name}_{image.FileName}");
                // Asunc upload of image to Azure Storage
                await picBlob.UploadFromStreamAsync(image.OpenReadStream());

                // Setting URL and success
                result.Uri = picBlob.Uri.AbsoluteUri;
                result.Success = true;
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}

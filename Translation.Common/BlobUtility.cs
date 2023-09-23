using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Translation.Common
{
    public class BlobUtility
    {
        private static readonly string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=eastransferdev;AccountKey=Fb2t75TrcIeuS394IguuEXdc2NiuJTu7PTv/p6Fbq4cGbdi+VmdfxD3kUn3Bs99Hei+1WcbZfQKI4nGpu/S2Ow==;EndpointSuffix=core.windows.net";

        public async Task<string> UploadToBlobAsync(string connectionString, string containerName, string filename)
        {
            var container = new BlobContainerClient(connectionString, containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
            var blob = container.GetBlobClient(Guid.NewGuid().ToString());
            var fileStream = File.OpenRead(filename);
            await blob.UploadAsync(fileStream, true);
            fileStream.Close();
            return blob.Uri.AbsoluteUri;
        }

        //Add a function to download from BlobContainer
        public async Task DownloadFromBlobAsync(string connectionString, string containerName, string filename)
        {
            var container = new BlobContainerClient(connectionString, containerName);
            var blob = container.GetBlobClient(filename);
            var fileStream = File.OpenWrite(filename);
            await blob.DownloadToAsync(fileStream);
            fileStream.Close();
        }

        //Add a function to download from Blob URL
        public async Task DownloadFromBlobAsync(string blobURL, string filename)
        {
            var blob = new BlobClient(new Uri(blobURL));
            var fileStream = File.OpenWrite(filename);
            await blob.DownloadToAsync(fileStream);
            fileStream.Close();
        }
    }
}
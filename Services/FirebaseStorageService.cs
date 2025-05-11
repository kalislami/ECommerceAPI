using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace ECommerceApi.Services
{
    public class FirebaseStorageService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public FirebaseStorageService(IConfiguration configuration)
        {
            var credentialsPath = configuration["Firebase:CredentialsPath"];
            _bucketName = configuration["Firebase:Bucket"]!;

            // Load credentials secara eksplisit dari file
            var credential = GoogleCredential.FromFile(credentialsPath);
            _storageClient = StorageClient.Create(credential);
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var extension = Path.GetExtension(fileName);
            var cleanName = Path.GetFileNameWithoutExtension(fileName);
            var newFileName = $"{cleanName}_{timestamp}{extension}";

            var objectName = "images/" + newFileName;

            await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                "image/jpeg",
                fileStream);

            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }
    }
}
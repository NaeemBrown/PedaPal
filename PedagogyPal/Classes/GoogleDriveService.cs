// GoogleDriveService.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace PedagogyPal.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _service;

        public GoogleDriveService(string credentialsPath)
        {
            var credential = GoogleCredential.FromFile(credentialsPath)
                .CreateScoped(DriveService.Scope.DriveFile);

            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "PedagogyPal",
            });
        }

        /// <summary>
        /// Uploads a file to Google Drive.
        /// </summary>
        /// <param name="filePath">The path of the file to upload.</param>
        /// <param name="folderId">Optional folder ID to upload the file into.</param>
        /// <returns>The uploaded file's ID if successful; otherwise, null.</returns>
        public async Task<string> UploadFileAsync(string filePath, string folderId = null)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = Path.GetFileName(filePath),
                Parents = folderId != null ? new List<string> { folderId } : null
            };

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var request = _service.Files.Create(fileMetadata, fileStream, GetMimeType(filePath));
                request.Fields = "id";

                var response = await request.UploadAsync();
                if (response.Status == UploadStatus.Completed)
                {
                    var file = request.ResponseBody;
                    MessageBox.Show($"File uploaded successfully! File ID: {file.Id}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return file.Id; // Return the file ID
                }
                else
                {
                    string errorMessage = response.Exception != null ? response.Exception.Message : "Unknown error.";
                    MessageBox.Show($"File upload failed: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null; // Return null or handle accordingly
                }
            }
        }

        /// <summary>
        /// Downloads a file from Google Drive.
        /// </summary>
        /// <param name="fileId">The ID of the file to download.</param>
        /// <param name="downloadPath">The local path to save the downloaded file.</param>
        /// <returns>The path of the downloaded file if successful; otherwise, null.</returns>
        public async Task<string> DownloadFileAsync(string fileId, string downloadPath)
        {
            try
            {
                var request = _service.Files.Get(fileId);
                using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
                {
                    var progress = await request.DownloadAsync(fileStream);
                    if (progress.Status == DownloadStatus.Completed)
                    {
                        Console.WriteLine($"File downloaded successfully to {downloadPath}");
                        return downloadPath;
                    }
                    else
                    {
                        Console.WriteLine("File download failed.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves metadata of a file from Google Drive.
        /// </summary>
        /// <param name="fileId">The ID of the file.</param>
        /// <returns>The file metadata if successful; otherwise, null.</returns>
        public async Task<Google.Apis.Drive.v3.Data.File> GetFileMetadataAsync(string fileId)
        {
            try
            {
                var request = _service.Files.Get(fileId);
                request.Fields = "id, name, mimeType";
                var file = await request.ExecuteAsync();
                return file;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving file metadata: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deletes a file from Google Drive using its File ID.
        /// </summary>
        /// <param name="fileId">The unique ID of the file to delete.</param>
        /// <returns>True if deletion is successful; otherwise, false.</returns>
        public async Task<bool> DeleteFileAsync(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                throw new ArgumentException("File ID cannot be null or empty.", nameof(fileId));

            try
            {
                var request = _service.Files.Delete(fileId);
                await request.ExecuteAsync();
                Console.WriteLine($"File with ID {fileId} deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file with ID {fileId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Determines the MIME type based on the file extension.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <returns>The MIME type as a string.</returns>
        private string GetMimeType(string filePath)
        {
            string mimeType = "application/octet-stream"; // Default MIME type
            string extension = Path.GetExtension(filePath).ToLower();

            // Add more mappings as needed
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);
            if (registryKey != null && registryKey.GetValue("Content Type") != null)
            {
                mimeType = registryKey.GetValue("Content Type").ToString();
            }

            return mimeType;
        }
    }
}

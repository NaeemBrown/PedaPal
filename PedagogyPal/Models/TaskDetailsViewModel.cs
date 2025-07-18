using PedagogyPal.Models;
using PedagogyPal.Services;
using System;
using System.Windows.Input;
using System.IO;
using System.Windows;
using System.Threading.Tasks;

namespace PedagogyPal.ViewModels
{
    public class TaskDetailsViewModel : BaseViewModel
    {
        private readonly GoogleDriveService _googleDriveService;

        public TaskModel Task { get; set; }

        public ICommand DownloadDocumentCommand { get; }

        public TaskDetailsViewModel(TaskModel task, GoogleDriveService googleDriveService)
        {
            Task = task;
            _googleDriveService = googleDriveService;
            DownloadDocumentCommand = new RelayCommand<string>(async link => await DownloadDocumentAsync(link), link => !string.IsNullOrEmpty(link));
        }

        private async Task DownloadDocumentAsync(string documentLink)
        {
            if (string.IsNullOrEmpty(documentLink))
            {
                MessageBox.Show("No attachment found for this task.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                string fileId = documentLink;
                if (documentLink.Contains("drive.google.com"))
                {
                    fileId = ExtractFileId(documentLink);
                    if (string.IsNullOrEmpty(fileId))
                    {
                        MessageBox.Show("Invalid document link.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Get file metadata to retrieve the original file name
                var file = await _googleDriveService.GetFileMetadataAsync(fileId);
                if (file == null)
                {
                    MessageBox.Show("File not found or access denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string fileName = file.Name;
                string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

                // Download the file using GoogleDriveService
                var resultPath = await _googleDriveService.DownloadFileAsync(fileId, downloadPath);
                if (!string.IsNullOrEmpty(resultPath))
                {
                    MessageBox.Show($"Document downloaded successfully to {resultPath}", "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to download the document.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ExtractFileId(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.Segments;
                int index = Array.IndexOf(segments, "d/") + 1;
                if (index > 0 && index < segments.Length)
                {
                    return segments[index].TrimEnd('/');
                }
            }
            catch
            {
                // Handle invalid URLs gracefully
            }
            return null;
        }
    }
}

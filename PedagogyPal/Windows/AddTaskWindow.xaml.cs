// AddTaskWindow.xaml.cs
using PedagogyPal.Models;
using PedagogyPal.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PedagogyPal.Windows
{
    public partial class AddTaskWindow : Window
    {
        private readonly FirebaseService _firebaseService;
        private readonly GoogleDriveService _googleDriveService;

        public TaskModel NewTask { get; private set; }

        public AddTaskWindow(FirebaseService firebaseService, GoogleDriveService googleDriveService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
            _googleDriveService = googleDriveService;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to select attachment
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                AttachmentTextBox.Text = openFileDialog.FileName;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a title for the task.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DueDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a due date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Create a new TaskModel instance with UTC DueDate
            DateTime localDate = DueDatePicker.SelectedDate.Value.Date;
            DateTime utcDate = new DateTime(localDate.Year, localDate.Month, localDate.Day, 0, 0, 0, DateTimeKind.Utc);
            NewTask = new TaskModel
            {
                Title = TitleTextBox.Text,
                DueDate = utcDate, // Set to UTC DateTime
                Status = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString(),
                DocumentLink = null, // Will be set after uploading
                UserId = _firebaseService.CurrentUserId // Assign the current user's ID
            };

            Console.WriteLine($"Creating task for UserId: {_firebaseService.CurrentUserId}");

            // Handle file upload to Google Drive
            if (!string.IsNullOrEmpty(AttachmentTextBox.Text) && File.Exists(AttachmentTextBox.Text))
            {
                try
                {
                    string sharedFolderId = "1fA-D1ugaB8fLipzt4coCYzyarq5n7bzh"; // Replace with your actual folder ID
                    string fileId = await _googleDriveService.UploadFileAsync(AttachmentTextBox.Text, sharedFolderId);
                    if (!string.IsNullOrEmpty(fileId))
                    {
                        NewTask.DocumentLink = fileId; // Store only the File ID
                        MessageBox.Show("Attachment uploaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Console.WriteLine("Attachment uploaded successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to upload the attachment to Google Drive.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine("Failed to upload the attachment to Google Drive.");
                        return;
                    }
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show($"File access error: {ioEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"File access error: {ioEx.Message}");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    return;
                }
            }

            try
            {
                // The task will be added to Firestore in the TasksViewModel
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Failed to add task: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without saving
            this.DialogResult = false;
            this.Close();
        }
    }
}

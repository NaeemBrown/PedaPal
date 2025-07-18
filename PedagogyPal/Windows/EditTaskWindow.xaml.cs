// EditTaskWindow.xaml.cs
using PedagogyPal.Models;
using PedagogyPal.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PedagogyPal.Windows
{
    public partial class EditTaskWindow : Window
    {
        private TaskModel _taskToEdit;
        private readonly FirebaseService _firebaseService;
        private readonly GoogleDriveService _googleDriveService;

        public EditTaskWindow(FirebaseService firebaseService, GoogleDriveService googleDriveService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
            _googleDriveService = googleDriveService;
        }

        public void SetTask(TaskModel task)
        {
            _taskToEdit = task;
            PopulateFields();
        }

        private void PopulateFields()
        {
            if (_taskToEdit != null)
            {
                TitleTextBox.Text = _taskToEdit.Title;
                DueDatePicker.SelectedDate = _taskToEdit.DueDate;

                // Set the selected status
                foreach (ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Content.ToString() == _taskToEdit.Status)
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }

                AttachmentTextBox.Text = _taskToEdit.DocumentLink;
            }
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

            // Update the task properties
            _taskToEdit.Title = TitleTextBox.Text;
            _taskToEdit.DueDate = DueDatePicker.SelectedDate.Value;
            _taskToEdit.Status = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();

            // Handle attachment changes
            if (!string.IsNullOrEmpty(AttachmentTextBox.Text) && File.Exists(AttachmentTextBox.Text))
            {
                string fileId = await _googleDriveService.UploadFileAsync(AttachmentTextBox.Text);
                if (!string.IsNullOrEmpty(fileId))
                {
                    _taskToEdit.DocumentLink = fileId; // Store only the File ID
                }
                else
                {
                    MessageBox.Show("Failed to upload the attachment to Google Drive.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            try
            {
                // Update the task in Firebase
                await _firebaseService.UpdateTaskAsync(_taskToEdit);

                MessageBox.Show("Task updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

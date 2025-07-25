// TasksViewModel.cs
using Microsoft.Extensions.DependencyInjection;
using PedagogyPal.Models;
using PedagogyPal.Services;
using PedagogyPal.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Google.Cloud.Firestore; 

namespace PedagogyPal.ViewModels
{
    public class TasksViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly FirebaseService _firebaseService;
        private readonly GoogleDriveService _googleDriveService;
        private readonly EmailService _emailService;
        private readonly ReminderService _reminderService;

        // Holds all tasks fetched from the service
        private ObservableCollection<TaskModel> _allTasks;

        // Public collection bound to the UI (filtered tasks)
        public ObservableCollection<TaskModel> Tasks { get; set; }

        // Search query bound to the search TextBox
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                    // Restart the search timer
                    _searchTimer.Stop();
                    _searchTimer.Start();
                }
            }
        }

        // Timer for debouncing search input
        private Timer _searchTimer;

        // Commands
        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand DownloadAndOpenDocumentCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ClearFilterCommand { get; } // New Command

        private TaskModel _selectedTask;
        public TaskModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask != value)
                {
                    _selectedTask = value;
                    OnPropertyChanged(nameof(SelectedTask));
                    // Update CanExecute for commands that depend on SelectedTask
                    ((RelayCommand)EditTaskCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteTaskCommand).RaiseCanExecuteChanged();
                }
            }
        }

        #region Calendar Properties

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                    // Filter tasks based on the selected date
                    FilterTasks(SearchQuery);
                }
            }
        }

        private DateTime _displayDate;
        public DateTime DisplayDate
        {
            get => _displayDate;
            set
            {
                if (_displayDate != value)
                {
                    _displayDate = value;
                    OnPropertyChanged(nameof(DisplayDate));
                    // Optionally, add logic to load tasks for the displayed month/year
                }
            }
        }

        #endregion

        public TasksViewModel(FirebaseService firebaseService, GoogleDriveService googleDriveService, EmailService emailService)
        {
            _firebaseService = firebaseService;
            _googleDriveService = googleDriveService;
            _emailService = emailService;
            _allTasks = new ObservableCollection<TaskModel>();
            Tasks = new ObservableCollection<TaskModel>();

            AddTaskCommand = new RelayCommand(async () => await AddTaskAsync());
            EditTaskCommand = new RelayCommand(async () => await EditTaskAsync(), () => SelectedTask != null);
            DeleteTaskCommand = new RelayCommand(async () => await DeleteTaskAsync(), () => SelectedTask != null);
            DownloadAndOpenDocumentCommand = new RelayCommand<string>(
                async (documentLink) => await DownloadAndOpenDocumentAsync(documentLink),
                (documentLink) => !string.IsNullOrEmpty(documentLink));
            ClearSearchCommand = new RelayCommand(ClearSearch);
            ClearFilterCommand = new RelayCommand(ClearFilter); // Initialize new Command

            // Initialize and start the reminder service
            _reminderService = new ReminderService(_firebaseService, _emailService);
            _reminderService.Start();

            // Initialize the search timer
            _searchTimer = new Timer(300); // 300 milliseconds
            _searchTimer.AutoReset = false;
            _searchTimer.Elapsed += (s, e) =>
            {
                // Ensure UI updates occur on the UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FilterTasks(SearchQuery);
                    Console.WriteLine($"Search Timer Elapsed: Filtered Tasks Count: {Tasks.Count}");
                });
            };

            // Initialize Calendar Properties
            InitializeCalendarProperties();

            // Load tasks initially (fire and forget)
            var _ = LoadTasksAsync(); // Start the async task without awaiting
            Console.WriteLine("TasksViewModel initialized and LoadTasksAsync called.");

            // Subscribe to CurrentUserId changes to initialize Firestore Listener
            _firebaseService.UserLoggedIn += FirebaseService_UserLoggedIn;
            _firebaseService.UserLoggedOut += FirebaseService_UserLoggedOut;

            // If user is already logged in (auto-login), initialize listener
            if (!string.IsNullOrEmpty(_firebaseService.CurrentUserId))
            {
                Console.WriteLine("TasksViewModel detected already logged-in user during initialization.");
                InitializeFirestoreListener();
            }
        }

        private void FirebaseService_UserLoggedIn(object sender, EventArgs e)
        {
            Console.WriteLine("TasksViewModel received UserLoggedIn event.");
            InitializeFirestoreListener();
            LoadTasksAsync();
        }

        private void FirebaseService_UserLoggedOut(object sender, EventArgs e)
        {
            Console.WriteLine("TasksViewModel received UserLoggedOut event.");
            _allTasks.Clear();
            Tasks.Clear();
        }

        private void InitializeFirestoreListener()
        {
            if (string.IsNullOrEmpty(_firebaseService.CurrentUserId))
            {
                Console.WriteLine("Firestore Listener not initialized: UserId is null or empty.");
                return;
            }

            Console.WriteLine($"Initializing Firestore Listener for UserId: {_firebaseService.CurrentUserId}");

            var tasksQuery = _firebaseService.GetFirestoreDb().Collection("tasks")
                .WhereEqualTo("UserId", _firebaseService.CurrentUserId);

            tasksQuery.Listen(snapshot =>
            {
                Console.WriteLine("Firestore Listener received a snapshot.");

                foreach (var change in snapshot.Changes)
                {
                    switch (change.ChangeType)
                    {
                        case DocumentChange.Type.Added:
                            var task = change.Document.ConvertTo<TaskModel>();
                            task.Id = change.Document.Id; // Set the Id property
                            _allTasks.Add(task);
                            Console.WriteLine($"Real-Time Added Task: {task.Title}");
                            break;
                        case DocumentChange.Type.Modified:
                            var modifiedTask = change.Document.ConvertTo<TaskModel>();
                            modifiedTask.Id = change.Document.Id; // Set the Id property
                            var existingTask = _allTasks.FirstOrDefault(t => t.Id == modifiedTask.Id);
                            if (existingTask != null)
                            {
                                existingTask.Title = modifiedTask.Title;
                                existingTask.Description = modifiedTask.Description;
                                existingTask.DueDate = modifiedTask.DueDate;
                                existingTask.Status = modifiedTask.Status;
                                existingTask.DocumentLink = modifiedTask.DocumentLink;
                                Console.WriteLine($"Real-Time Modified Task: {existingTask.Title}");
                            }
                            break;
                        case DocumentChange.Type.Removed:
                            var removedTask = change.Document.ConvertTo<TaskModel>();
                            removedTask.Id = change.Document.Id; // Set the Id property
                            var taskToRemove = _allTasks.FirstOrDefault(t => t.Id == removedTask.Id);
                            if (taskToRemove != null)
                            {
                                _allTasks.Remove(taskToRemove);
                                Console.WriteLine($"Real-Time Removed Task: {taskToRemove.Title}");
                            }
                            break;
                    }
                }

                // Refresh the UI
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FilterTasks(SearchQuery);
                    Console.WriteLine($"Filtered Tasks Count after snapshot: {Tasks.Count}");
                });
            });

            Console.WriteLine("Firestore Listener initialized successfully.");
        }

        #region Calendar Initialization

        /// <summary>
        /// Initializes the SelectedDate and DisplayDate properties.
        /// </summary>
        private void InitializeCalendarProperties()
        {
            SelectedDate = null; 
            DisplayDate = DateTime.UtcNow.Date;
        }

        #endregion

        /// <summary>
        /// Loads tasks from Firebase and initializes the AllTasks and Tasks collections.
        /// </summary>
        public async Task LoadTasksAsync()
        {
            try
            {
                Console.WriteLine("Loading tasks from Firestore...");
                var tasks = await _firebaseService.GetTasksAsync();
                _allTasks.Clear();

                foreach (var task in tasks)
                {
                    // Ensure DueDate is UTC
                    if (task.DueDate.Kind != DateTimeKind.Utc)
                    {
                        task.DueDate = DateTime.SpecifyKind(task.DueDate, DateTimeKind.Utc);
                    }

                    _allTasks.Add(task);
                    Console.WriteLine($"Loaded Task: {task.Title}, DueDate: {task.DueDate}, UserId: {task.UserId}");
                }

                // Initialize the Tasks collection with all tasks or filter based on SelectedDate
                FilterTasks(SearchQuery);
                Console.WriteLine($"Total Tasks after filtering: {Tasks.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Exception in LoadTasksAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// Filters the Tasks collection based on the search query and selected date.
        /// </summary>
        /// <param name="query">The search query string.</param>
        private void FilterTasks(string query)
        {
            Console.WriteLine($"FilterTasks called with query: '{query}' and SelectedDate: {SelectedDate}");

            if (string.IsNullOrWhiteSpace(query) && SelectedDate == null)
            {
                // If search query is empty and no date is selected, display all tasks
                Tasks.Clear();
                foreach (var task in _allTasks)
                {
                    Tasks.Add(task);
                    Console.WriteLine($"Added Task to UI: {task.Title}");
                }
            }
            else
            {
                // Filter tasks based on the query and/or selected date
                var filtered = _allTasks.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    filtered = filtered.Where(t => t.Title != null && t.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
                    Console.WriteLine($"Filtered by query: {query}, remaining tasks: {filtered.Count()}");
                }

                if (SelectedDate.HasValue)
                {
                    filtered = filtered.Where(t => t.DueDate.Date == SelectedDate.Value.Date);
                    Console.WriteLine($"Filtered by SelectedDate: {SelectedDate.Value.Date}, remaining tasks: {filtered.Count()}");
                }

                Tasks.Clear();
                foreach (var task in filtered)
                {
                    Tasks.Add(task);
                    Console.WriteLine($"Filtered and Added Task to UI: {task.Title}");
                }
            }

            Console.WriteLine($"Filtered Tasks Count: {Tasks.Count}");
        }

        /// <summary>
        /// Clears the search query.
        /// </summary>
        private void ClearSearch()
        {
            SearchQuery = string.Empty;
            Console.WriteLine("Search query cleared.");
        }

        /// <summary>
        /// Clears all filters, including search query and selected date.
        /// </summary>
        private void ClearFilter()
        {
            SearchQuery = string.Empty;
            SelectedDate = null;
            Console.WriteLine("Filters cleared. Displaying all tasks.");
        }

        /// <summary>
        /// Adds a new task by opening the AddTaskWindow.
        /// </summary>
        public async Task AddTaskAsync()
        {
            var addTaskWindow = App.ServiceProvider.GetRequiredService<AddTaskWindow>();
            addTaskWindow.Owner = Application.Current.MainWindow;
            bool? result = addTaskWindow.ShowDialog();
            if (result == true)
            {
                // Retrieve the new task from the AddTaskWindow
                if (addTaskWindow.NewTask != null)
                {
                    try
                    {
                        var addedTask = await _firebaseService.AddTaskAsync(addTaskWindow.NewTask);
                        _allTasks.Add(addedTask); // Add to AllTasks collection
                        FilterTasks(SearchQuery); // Refresh the Tasks collection based on current search query and selected date
                        OnPropertyChanged(nameof(Tasks)); // Notify the UI
                        Console.WriteLine($"Task '{addedTask.Title}' added and Tasks collection refreshed. Total tasks: {Tasks.Count}");

                        // Show a message to inform that the task was created
                        MessageBox.Show($"Task '{addedTask.Title}' was created successfully.", "Task Created", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to add task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"Exception in AddTaskAsync: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Edits the selected task by opening the EditTaskWindow.
        /// </summary>
        private async Task EditTaskAsync()
        {
            if (SelectedTask != null)
            {
                var editTaskWindow = App.ServiceProvider.GetRequiredService<EditTaskWindow>();
                editTaskWindow.Owner = Application.Current.MainWindow;
                editTaskWindow.SetTask(SelectedTask); // Pass the selected task to the window
                bool? result = editTaskWindow.ShowDialog();
                if (result == true)
                {
                    await LoadTasksAsync(); // Reload tasks to reflect updates
                    Console.WriteLine($"Task '{SelectedTask.Title}' edited and Tasks collection refreshed.");
                }
            }
        }

        /// <summary>
        /// Deletes the selected task after confirming with the user.
        /// Also deletes the associated document from Google Drive if it exists.
        /// </summary>
        private async Task DeleteTaskAsync()
        {
            if (SelectedTask != null)
            {
                var result = MessageBox.Show("Are you sure you want to delete this task?", "Delete Task", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // First, delete the associated file from Google Drive if it exists
                        if (!string.IsNullOrEmpty(SelectedTask.DocumentLink))
                        {
                            bool fileDeleted = await _googleDriveService.DeleteFileAsync(SelectedTask.DocumentLink);
                            if (!fileDeleted)
                            {
                                MessageBox.Show("Failed to delete the associated file from Google Drive.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                // Optionally, decide whether to proceed with task deletion or abort
                                // For this example, we'll abort the deletion
                                return;
                            }
                        }

                        // Now, delete the task from Firestore
                        await _firebaseService.DeleteTaskAsync(SelectedTask);
                        _allTasks.Remove(SelectedTask); 
                        FilterTasks(SearchQuery); 
                        MessageBox.Show("Task deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                        Console.WriteLine($"Task '{SelectedTask.Title}' deleted and Tasks collection refreshed.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"Exception in DeleteTaskAsync: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Downloads and opens the document associated with a task.
        /// </summary>
        /// <param name="documentLink">The link to the document.</param>
        private async Task DownloadAndOpenDocumentAsync(string documentLink)
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
                string tempPath = Path.Combine(Path.GetTempPath(), fileName);

                // Download the file using GoogleDriveService
                var resultPath = await _googleDriveService.DownloadFileAsync(fileId, tempPath);
                if (!string.IsNullOrEmpty(resultPath))
                {
                    // Open the file with the default application
                    try
                    {
                        var process = new System.Diagnostics.Process
                        {
                            StartInfo = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = resultPath,
                                UseShellExecute = true
                            }
                        };
                        process.Start();
                        Console.WriteLine($"Opened document: {fileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to open the document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"Exception in Opening Document: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to download the document.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine("Failed to download the document.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download and open document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Exception in DownloadAndOpenDocumentAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// Extracts the file ID from a Google Drive URL.
        /// </summary>
        /// <param name="url">The Google Drive URL.</param>
        /// <returns>The file ID or null if extraction fails.</returns>
        private string ExtractFileId(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.Segments;
                // Example Google Drive URL: https://drive.google.com/file/d/FILE_ID/view?usp=sharing
                // or https://drive.google.com/open?id=FILE_ID
                string fileId = null;

                // Attempt to extract from '/d/FILE_ID/' pattern
                for (int i = 0; i < segments.Length; i++)
                {
                    if (segments[i].Equals("d/", StringComparison.OrdinalIgnoreCase) && i + 1 < segments.Length)
                    {
                        fileId = segments[i + 1].TrimEnd('/');
                        break;
                    }
                }

                // If not found, check query parameters
                if (string.IsNullOrEmpty(fileId))
                {
                    var query = uri.Query.TrimStart('?');
                    var queryParams = query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries); // Corrected Split

                    foreach (var param in queryParams)
                    {
                        var parts = param.Split(new char[] { '=' }, 2, StringSplitOptions.None); // Corrected Split
                        if (parts.Length == 2 && parts[0].Equals("id", StringComparison.OrdinalIgnoreCase))
                        {
                            fileId = parts[1];
                            break;
                        }
                    }
                }

                return fileId;
            }
            catch
            {
                // Handle invalid URLs gracefully
                return null;
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event for INotifyPropertyChanged.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

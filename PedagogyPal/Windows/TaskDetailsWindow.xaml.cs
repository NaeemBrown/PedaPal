using PedagogyPal.Models;
using PedagogyPal.Services;
using PedagogyPal.ViewModels;
using System.Windows;

namespace PedagogyPal.Windows
{
    public partial class TaskDetailsWindow : Window
    {
        public TaskDetailsWindow(TaskModel task, GoogleDriveService googleDriveService)
        {
            InitializeComponent();
            DataContext = new TaskDetailsViewModel(task, googleDriveService);
        }
    }
}

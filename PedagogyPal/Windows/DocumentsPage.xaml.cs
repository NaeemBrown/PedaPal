// DocumentsPage.xaml.cs
using System.Windows;

namespace PedagogyPal.Windows
{
    public partial class DocumentsPage : Window
    {
        public DocumentsPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the HomePage window if it's hidden
            if (this.Owner != null)
            {
                this.Owner.Show();
            }

            // Close the DocumentsPage
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Show the HomePage window if it's hidden and not already visible
            if (this.Owner != null && !this.Owner.IsVisible)
            {
                this.Owner.Show();
            }
        }
    }
}

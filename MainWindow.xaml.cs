using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticSearchApp
{
    public partial class MainWindow : Window
    {
        private readonly DocumentStore _documentStore;

        public MainWindow()
        {
            InitializeComponent();
            var dataLoader = new DataLoader();
            _documentStore = new DocumentStore(dataLoader);
            _documentStore.ProcessingStatusChanged += DocumentStore_ProcessingStatusChanged;
            InitializeDocumentStoreAsync();
        }

        private void DocumentStore_ProcessingStatusChanged(object? sender, string status)
        {
            // Ensure UI updates happen on the UI thread
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = status;
            });
        }

        private async void InitializeDocumentStoreAsync()
        {
            try
            {
                await _documentStore.InitializeAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading documents: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Please enter a search query.", "Search", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SearchButton.IsEnabled = false;
            try
            {
                var results = await _documentStore.SearchDocumentsAsync(query);
                ResultsList.ItemsSource = results;

                if (!results.Any())
                {
                    MessageBox.Show("No results found.", "Search Results", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SearchButton.IsEnabled = true;
            }
        }
    }
}
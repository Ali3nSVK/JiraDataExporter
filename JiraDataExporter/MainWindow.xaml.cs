using System.Windows;

namespace JiraDataExporter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void GetButton_Click(object sender, RoutedEventArgs e)
    {
        GetButton.IsEnabled = false;
        GetButton.Content = "Working ...";

        try
        {
            var jiraService = new JiraService();

            var issueKeys = JiraListBox.Text
                .Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim());

            var results = await jiraService.GetIssuesStatusAsync(issueKeys);
            var writer = new ExcelWriter();
            writer.WriteWorkBook(results);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        GetButton.IsEnabled = true;
        GetButton.Content = "Get";
    }
}
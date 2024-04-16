using System.Windows;
using System.Windows.Controls;

public class ConnectionWindow
{
    public static string ConnectionUri { get; set; }
    public static async void ShowConnectionDialog()
    {
        Window dialog = new()
        {
            Title = "http://localhost:8000",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize
        };

        StackPanel stackPanel = new StackPanel();

        TextBox textBox = new()
        {
            Width = 200,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Center
        };

        Button connectButton = new()
        {
            Content = "Сonnect to",
            Width = 100,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        connectButton.Click += (s, args) =>
        {
            string rawUrl = textBox.Text;
            ConnectionUri = Uri.EscapeUriString(rawUrl);

            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                MessageBox.Show(@"Input Url (like 'http://example.com/')");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to connect to {rawUrl}?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"You have connected to: {ConnectionUri}");
                    dialog.Close(); // Close the window after connecting
                }
            }
        };

        stackPanel.Children.Add(textBox);
        stackPanel.Children.Add(connectButton);
        dialog.Content = stackPanel;

        dialog.ShowDialog();
    }
}

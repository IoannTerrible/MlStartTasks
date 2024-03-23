using System.Windows;
using System.Windows.Controls;

public class ConnectionWindow
{
    public static async void ShowConnectionDialog()
    {
        Window dialog = new Window
        {
            Title = "Введите HTTP адрес",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize
        };

        StackPanel stackPanel = new StackPanel();

        TextBox textBox = new TextBox
        {
            Width = 200,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Center
        };

        Button connectButton = new Button
        {
            Content = "Подключиться",
            Width = 100,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        connectButton.Click += (s, args) =>
        {
            // Обработка нажатия кнопки подключения
            string url = textBox.Text;
            // Выполните здесь код для подключения по HTTP адресу
            MessageBox.Show($"Вы подключились по адресу: {url}");
            dialog.Close(); // Закрыть окно после подключения
        };

        stackPanel.Children.Add(textBox);
        stackPanel.Children.Add(connectButton);
        dialog.Content = stackPanel;

        dialog.ShowDialog(); // Показать окно для ввода адреса и подключения
    }
}

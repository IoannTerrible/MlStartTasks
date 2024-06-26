﻿using ClassLibrary;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для ConfigPage.xaml
    /// </summary>
    public partial class ConfigPage : Page
    {
        private MainWindow _window;
        private DataTable _configTable; 
        public ConfigPage(MainWindow window)
        {
            _window = window;
            InitializeComponent();

            dictionaryDataGrid.AutoGenerateColumns = false;

            _configTable = new DataTable();
            DataColumn keyColumn = _configTable.Columns.Add("Key");
            DataColumn valueColumn = _configTable.Columns.Add("Value");
            keyColumn.ReadOnly = true;

            foreach (var item in App.ContentFromConfig)
            {
                _configTable.Rows.Add(item.Key, item.Value);
            }

            dictionaryDataGrid.ItemsSource = _configTable.DefaultView;
            
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            App.ContentFromConfig.Clear();
            foreach (DataRowView row in _configTable.DefaultView)
            {
                App.ContentFromConfig.Add(row["Key"].ToString(), row["Value"].ToString());
            }
            ConfigReader.UpdateConfigValues(App.PathToConfig, App.ContentFromConfig);
        }
    }
}



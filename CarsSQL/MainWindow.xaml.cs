using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CarsSQL
{
    public partial class MainWindow : Window
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public MainWindow()
        {
            InitializeComponent();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
            _connectionString = _configuration.GetConnectionString("CarDatabase");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchColumn = rbMarka.IsChecked == true ? "Marka" : "Model";
            string searchValue = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                MessageBox.Show("Please enter a search term.");
                return;
            }

            string query = $"SELECT * FROM Car WHERE {searchColumn} LIKE @searchValue";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@searchValue", "%" + searchValue + "%");

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    lstResults.Items.Clear();

                    while (reader.Read())
                    {
                        string result = $"Id: {reader["Id"]}, Marka: {reader["Marka"]}, Model: {reader["Model"]}";
                        lstResults.Items.Add(result);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
    }
}

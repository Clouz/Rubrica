using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace rubrica
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ICollectionView view;
        List<User> users;
        string filename = "rubrica.json";
        string json;

        public MainWindow()
        {
            if (File.Exists(filename))
            {
                json = File.ReadAllText(filename);
                users = Deserialize(json);
            }
            else
            {
                File.Create(filename);
                users = new List<User> { new User { Name = "", Department = "", Phone = "" } };
            }
            


            InitializeComponent();
            SearchBar.Focus();
            
            view = CollectionViewSource.GetDefaultView(users);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            DataGrid.ItemsSource = view;
        }

        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            string json = Serialize(users);
            File.WriteAllText(filename, json);
            this.Close();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            StringComparison comp = StringComparison.OrdinalIgnoreCase;
            view.Filter = o =>
            {
                User u = o as User;
                return u.Name.Contains(SearchBar.Text, comp) || u.Phone.Contains(SearchBar.Text, comp) || u.Department.Contains(SearchBar.Text, comp);
            };
        }

        public class User
        {
            public string Name { get; set; }
            public string Department { get; set; }
            public string Phone { get; set; }
        }

        List<User> Deserialize(string json)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };

            try
            {
                return JsonSerializer.Deserialize<List<User>>(json, options);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to read rubrica.json", "Error",MessageBoxButton.OK,MessageBoxImage.Error);
                throw;
            }
        }

        string Serialize(List<User> u)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                WriteIndented = true
            };

            return JsonSerializer.Serialize<List<User>>(u, options);
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {

        }
    }
}

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

namespace Agenda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ICollectionView view;

        public MainWindow()
        {
           string json = File.ReadAllText("rubrica.json");

            
            List<User> users = Deserialize(json);

            InitializeComponent();
            SearchBar.Focus();
            
            view = CollectionViewSource.GetDefaultView(users);

            DataGrid.ItemsSource = view;
        }

        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
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

        return JsonSerializer.Deserialize<List<User>>(json, options);
    }

    }
}

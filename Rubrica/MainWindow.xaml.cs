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
            flag.Visibility = Visibility.Collapsed;
            SearchBar.Focus();
            
            view = CollectionViewSource.GetDefaultView(users);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            DataGrid.ItemsSource = view;
        }

        private void ExitFuncion()
        {
            view.Refresh();
            string json = Serialize(users);
            File.WriteAllText(filename, json);
            this.Close();
        }

        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ExitFuncion();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            StringComparison comp;
            switch (true)
            {
                case true when SearchBar.Text.ToLower() == "name: " || SearchBar.Text.ToLower() == "dep: " || SearchBar.Text.ToLower() == "phone: ":
                    flag.Visibility = Visibility.Visible;
                    flag.Content = SearchBar.Text.ToUpper().Substring(0, SearchBar.Text.Length - 2);
                    SearchBar.Text = "";
                    break;
                case true when flag.Visibility == Visibility.Visible:
                    comp = StringComparison.OrdinalIgnoreCase;
                    view.Filter = o =>
                    {
                        User u = o as User;
                        switch (flag.Content.ToString())
                        {
                            case "NAME":
                                return u.Name.Contains(SearchBar.Text, comp);
                            case "DEP":
                                return u.Department.Contains(SearchBar.Text, comp);
                            case "PHONE":
                                return u.Phone.Contains(SearchBar.Text, comp);
                            default:
                                return true;
                        }
                    };
                    break;
                default:
                    comp = StringComparison.OrdinalIgnoreCase;
                    view.Filter = o =>
                    {
                        User u = o as User;
                        return u.Name.Contains(SearchBar.Text, comp) || u.Phone.Contains(SearchBar.Text, comp) || u.Department.Contains(SearchBar.Text, comp);
                    };
                    break;
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && flag.Visibility == Visibility.Visible && SearchBar.Text == "")
            {
                e.Handled = true;
                flag.Visibility = Visibility.Collapsed;
            }
        }

        public class User : IDataErrorInfo
        {

            public string Name { get; set; }
            public string Department { get; set; }
            public string Phone { get; set; }

            public string Error
            {
                get
                {
                    return String.Concat(this[Name], " ", this[Department], " ", this[Phone]);
                }
            }

            public string this[string columnName]
            {
                get
                {
                    string errorMessage = null;
                    switch (columnName)
                    {
                        case "Name":
                            if (Name == null)
                            {
                                Name = "";
                                errorMessage = "Name is required.";
                            }
                            break;
                        case "Department":
                            if (Department == null)
                            {
                                Department = "";
                                errorMessage = "Department is required.";
                            }
                            break;
                        case "Phone":
                            if (Phone == null)
                            {
                                Phone = "";
                                errorMessage = "Phone is required.";
                            }
                            break;
                    }
                    return errorMessage;
                }
            }
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



        private void Window_Closed(object sender, EventArgs e)
        {
            ExitFuncion();
        }

        private void menu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("click!!!");
        }

        private void OnFocusExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBar.Focus();
        }
    }
}

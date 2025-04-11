using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ConsoleGyakorlasWpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    List<DataItem> dataList = [];
    public MainWindow()
    {
        InitializeComponent();

        cbFilter.Items.Add("öszes");
        cbFilter.Items.Add("belépés");
        cbFilter.Items.Add("kilépés");
        cbFilter.Items.Add("menza");
        cbFilter.Items.Add("könyvtár");

        cbFilter.SelectedIndex = 0;

        dataList = new List<DataItem>();

        cbFilter.SelectionChanged += (s, e) => 
        {
            if (cbFilter.SelectedItem.ToString() == "öszes")
                dgTabla.ItemsSource = dataList;
            else dgTabla.ItemsSource = dataList.Where(x => x.Fajta == cbFilter.SelectedItem.ToString());
        };
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Szöveges file (*.txt)|*.txt|Összes file (*.*)|*.*",
            Title = "File kiválasztása"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                var lines = File.ReadAllLines(openFileDialog.FileName);
                //var dataList = new List<DataItem>();

                foreach (var line in lines)
                {
                    var parts = line.Split(' ');

                    if (parts.Length == 3)
                    {
                        string kód = parts[0];
                        DateTime idő = DateTime.Parse(parts[1]);
                        int fajtaInt = int.Parse(parts[2]);
                        string fajta = fajtaInt switch
                        {
                            1 => "belépés",
                            2 => "kilépés",
                            3 => "menza",
                            4 => "könyvtár",
                            _ => "ismeretlen"
                        };
                        dataList.Add(new DataItem { Kód = kód, Idő = idő, Fajta = fajta });
                    }
                }
                dgTabla.ItemsSource = dataList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a file betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class DataItem
    {
        public string Kód { get; set; }
        public DateTime Idő { get; set; }
        public string Fajta { get; set; }
    }

    private void btnClear_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Biztos vagy benne hogy szeretnéd kiüríteni a táblát?",
            "Kiürítés",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
            dgTabla.ItemsSource = null;
        }
    }

    private void btnSaveTo_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Szöveges file (*.txt)|*.txt|All Files (*.*)|*.*",
            Title = "Táblázat tartalmának mentése"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                var dataItems = dgTabla.ItemsSource as IEnumerable<DataItem>;

                if (dataItems != null)
                {
                    StringBuilder fileContent = new StringBuilder();

                    foreach (var item in dataItems)
                    {
                        int fajtaCode = item.Fajta switch
                        {
                            "belépés" => 1,
                            "kilépés" => 2,
                            "menza" => 3,
                            "könyvtár" => 4
                        };

                        fileContent.AppendLine($"{item.Kód} {item.Idő:yyyy-MM-dd HH:mm:ss} {fajtaCode}");
                    }

                    File.WriteAllText(saveFileDialog.FileName, fileContent.ToString());
                    MessageBox.Show("Sikeres mentés!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nincs menthető adat.", "Figyelem", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a file mentésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = dgTabla.SelectedItem as DataItem;
        if (selectedItem != null)
        {
            var dataItems = dgTabla.ItemsSource as List<DataItem>;
            if (dataItems != null)
            {
                dataItems.Remove(selectedItem);
                dgTabla.ItemsSource = null;
                dgTabla.ItemsSource = dataItems;
            }
        }
        else
        {
            MessageBox.Show("Nincs kijelölt sor.", "Figyelem", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
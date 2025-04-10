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
    public MainWindow()
    {
        InitializeComponent();

        cbDelete.Items.Add("öszes");
        cbDelete.Items.Add("belépés");
        cbDelete.Items.Add("kilépés");
        cbDelete.Items.Add("menza");
        cbDelete.Items.Add("könyvtár");

        cbDelete.SelectedIndex = 0;
        var originalData = dgTabla.ItemsSource as IEnumerable<DataItem>;
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
                var dataList = new List<DataItem>();

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

    }

    private void cbDelete_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbDelete.SelectedItem is string selectedFajta)
        {
            

            if (originalData != null)
            {
                if (selectedFajta == "All")
                {
                    dgTabla.ItemsSource = originalData.ToList();
                }
                else
                {
                    var filteredData = originalData.Where(item => item.Fajta == selectedFajta).ToList();
                    dgTabla.ItemsSource = filteredData;
                }
            }
        }
    }
}
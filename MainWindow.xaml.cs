using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
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
using Path = System.IO.Path;

namespace SaltItemDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window // TODO: Implement ability to read in item file.
    {
        public MainWindow()
        {
            InitializeComponent();
            ObservableCollection<ItemRarity> rarityObjs = new ()
            {
                new ItemRarity("#FFFFFF", "Common"),
                new ItemRarity("#5DA546", "Uncommon"),
                new ItemRarity("#336F9A", "Rare"),
                new ItemRarity("#6A3173", "Epic"),
                new ItemRarity("#D77700", "Legendary")
            };
            ItemRarityComboBox.ItemsSource = rarityObjs;
            ItemRarityComboBox.SelectedIndex = 1;
            rarityObjs.CollectionChanged += RarityObjs_Changed;
        }

        private void RarityObjs_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null) // Items have been added to the combobox
            {
                Trace.Write("items added");
            }
            else if (e.OldItems != null) // Items have been removed from the combobox
            {
                Trace.Write("Items removed");
            }
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All Files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == true)
            {
                var selectedFileName = dlg.FileName;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                ItemIcon.Source = bitmap;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", $"{_curDir}Items\\");
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            string itemsDir = $"{_curDir}Items\\";
            if (!Directory.Exists(itemsDir)) Directory.CreateDirectory(itemsDir);

            if (!string.IsNullOrEmpty(TitleTextBox.Text) && !string.IsNullOrEmpty(FlareTextBox.Text))
            { 
                var itemJson = JsonSerializer.Serialize(FetchItemValues());
                Trace.WriteLine($"{_curDir}Items\\");
                string[] folders = Directory.GetDirectories(itemsDir, "", SearchOption.TopDirectoryOnly);
                Trace.WriteLine("folder length: " + folders.Length);

                var dupeNamesFound = 0;
                foreach (var dirPath in folders)
                {
                    Trace.WriteLine("checking: " + Path.GetFileName(dirPath));
                    if (Path.GetFileName(dirPath) == TitleTextBox.Text)
                    {
                        // Duplicate item found, increasing folder suffix
                        dupeNamesFound++;
                    }
                }
                Trace.WriteLine(itemsDir);
                if (dupeNamesFound > 0)
                {
                    Directory.CreateDirectory($"{itemsDir}{TitleTextBox.Text} ({dupeNamesFound})");
                    File.WriteAllText($"{itemsDir}{TitleTextBox.Text} ({dupeNamesFound})\\{TitleTextBox.Text}.json", itemJson);
                    System.Windows.MessageBox.Show("Item exported successfully.", "Export Success", MessageBoxButton.OK);
                    return;
                }
                Directory.CreateDirectory($"{itemsDir}{TitleTextBox.Text}");
                File.WriteAllText($"{itemsDir}{TitleTextBox.Text}\\{TitleTextBox.Text}.json", itemJson);
                System.Windows.MessageBox.Show("Item exported successfully.", "Export Success", MessageBoxButton.OK);
            }
            else
            {
                System.Windows.MessageBox.Show("Unable to export item. Please check your fields and try again.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ItemRarityComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                var colorSelected = ((ItemRarity)e.AddedItems[0]).RarityName;
                var colorBrushSelected = ((ItemRarity) e.AddedItems[0]).RarityColor;
                TitleTextBox.Foreground = colorBrushSelected;
                Trace.WriteLine($"Color selected: {colorSelected}.");
            }
        }

        private Dictionary<string, string> FetchItemValues()
        {
            return new Dictionary<string, string>
            {
                ["ItemTitle"] = TitleTextBox.Text,
                ["ItemFlareText"] = FlareTextBox.Text,
                ["ItemPrice"] = GoldAmountBox.Text,
                ["ItemRarity"] = ((ItemRarity)ItemRarityComboBox.SelectedItem).RarityName,
                ["ItemID"] = "10000" // TODO: Implement this.
            };
        }
        private readonly string _curDir = AppDomain.CurrentDomain.BaseDirectory;

        private void GoldUp_OnClick(object sender, RoutedEventArgs e)
        {
            GoldAmountBox.Text = ModifyGoldAmount(GoldAmountBox.Text, true);
        }

        private void GoldDown_OnClick(object sender, RoutedEventArgs e)
        {
            GoldAmountBox.Text = ModifyGoldAmount(GoldAmountBox.Text, false);
        }

        private string ModifyGoldAmount(string amount, bool upOrDown)
        {
            bool isSuccessful = int.TryParse(amount, out var amountNum);

            if (isSuccessful) return upOrDown ? (amountNum + 1).ToString() : (amountNum - 1).ToString();
            System.Windows.MessageBox.Show("Invalid gold amount. Please try entering a smaller number or a numeric value.", "Invalid Amount Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return "0";
        }

        private void GoldAmountBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var isSuccessful = int.TryParse(GoldAmountBox.Text, out var num);
            if (isSuccessful) return;
            System.Windows.MessageBox.Show("Invalid gold amount. Please try entering a smaller number or a numeric value.", "Invalid Amount Error", MessageBoxButton.OK, MessageBoxImage.Error);
            GoldAmountBox.Text = "0";
        }
    }
}

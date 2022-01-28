using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Text.Json;

namespace SaltItemDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window // TODO: Reorganize file structure.
    {
        public MainWindow()
        {
            InitializeComponent();

            ObservableCollection<ItemRarity> rarityObjs = new()
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

            StatusEffect[] effectObjs =
            {
                new() { EffectType = "CritChance" },
                new() { EffectType = "Strength" }
            };

            ItemStatusEffectsComboBox.ItemsSource = effectObjs;
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
                Filter = "PNG Image Files (*.png)|*.png",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == true)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dlg.FileName);
                bitmap.EndInit();
                ItemIcon.Source = bitmap;
                _curItemIcon = bitmap;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", $"{_curDir}Items\\");
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            string itemsDir = $"{_curDir}Items\\";
            if (!Directory.Exists(itemsDir)) Directory.CreateDirectory(itemsDir);

            var dlg = new OpenFileDialog
            {
                InitialDirectory = itemsDir,
                Filter = "Item Files (*.item)|*.item",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == true)
            {
                ImportItemData(dlg.FileName);
            }
        }

        private void ImportItemData(string filename)
        {
            string jsonEncoded = File.ReadAllText(filename);
            ItemSaveable itemSaved = JsonSerializer.Deserialize<ItemSaveable>(jsonEncoded);
            ItemRarity itemSavedRarity = new(itemSaved.ItemRarityColorHex, itemSaved.ItemRarityName);

            TitleTextBox.Text = itemSaved.ItemTitle;
            TitleTextBox.Foreground = itemSavedRarity.RarityColor;
            FlareTextBox.Text = itemSaved.ItemFlareText;
            GoldAmountBox.Text = itemSaved.ItemPrice;
            ItemIcon.Source = itemSaved.ItemIconBitmapImage;
            _curItemIcon = itemSaved.ItemIconBitmapImage;

            Trace.WriteLine("Checking itemrarity");
            for (var i = 0; i < ItemRarityComboBox.Items.Count; i++)
            {
                ItemRarity itemrarity = (ItemRarity)ItemRarityComboBox.Items[i];
                if (itemrarity.RarityName == itemSavedRarity.RarityName && itemrarity.RarityColorHex == itemSavedRarity.RarityColorHex)
                {
                    Trace.WriteLine("itemraritycombox contains rarity object");
                    Trace.WriteLine("index of item: " + ItemRarityComboBox.Items.IndexOf(itemSavedRarity)); // TODO: Figure out why index of/contains don't work.
                    ItemRarityComboBox.SelectedIndex = i; // TODO: Implement custom rarity support.
                }
                
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var itemsDir = $"{_curDir}Items\\";
            Trace.WriteLine(itemsDir);
            if (!Directory.Exists(itemsDir)) Directory.CreateDirectory(itemsDir);

            if (!string.IsNullOrEmpty(TitleTextBox.Text) && !string.IsNullOrEmpty(FlareTextBox.Text) && _curItemIcon != null)
            {
                var itemJson = JsonSerializer.Serialize(FetchItemValues());

                var dialog = new SaveFileDialog
                {
                    AddExtension = true,
                    OverwritePrompt = true,
                    CheckPathExists = true,
                    DefaultExt = ".item",
                    Filter = "Item files|*.item",
                    InitialDirectory = itemsDir
                };
                var saveSuccess = dialog.ShowDialog();

                if (saveSuccess == true)
                {
                    Trace.WriteLine("Save success: " + dialog.FileName);
                    File.WriteAllText(dialog.FileName, itemJson);
                    MessageBox.Show("Item exported successfully.", "Export Success", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Unable to export item. Please check your fields and or select an item icon before trying again.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ItemRarityComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                Trace.WriteLine("raritycombobox selection changed.");
                Trace.WriteLine("e length: " + e.AddedItems.Count);

                var selectedItemRarity = (ItemRarity)e.AddedItems[0];
                var colorSelected = selectedItemRarity.RarityName;
                var colorBrushSelected = selectedItemRarity.RarityColor;
                TitleTextBox.Foreground = colorBrushSelected;
                Trace.WriteLine($"Color selected: {colorSelected}.");

                if (ItemRarityComboBox.Items.Contains(selectedItemRarity))
                {
                    // Rarity value already exists in the combobox, assuming not custom. Setting it directly in case this was imported. 
                    Trace.WriteLine("selectionchanged index: " + ItemRarityComboBox.Items.IndexOf(selectedItemRarity));
                    ItemRarityComboBox.SelectedIndex = ItemRarityComboBox.Items.IndexOf(selectedItemRarity);
                }
                //if (ItemRarityComboBox.Items[ItemRarityComboBox.SelectedIndex].
            }
        }

        private Dictionary<string, string> FetchItemValues()
        {
            return new Dictionary<string, string>
            {
                ["ItemTitle"] = TitleTextBox.Text,
                ["ItemFlareText"] = FlareTextBox.Text,
                ["ItemPrice"] = GoldAmountBox.Text,
                ["ItemRarityName"] = ((ItemRarity)ItemRarityComboBox.SelectedItem).RarityName,
                ["ItemRarityColorHex"] = ((ItemRarity)ItemRarityComboBox.SelectedItem).RarityColorHex,
                ["ItemIcon"] = GetIconBase64()
            };
        }

        private string GetIconBase64()
        {
            var pngEncoder = new PngBitmapEncoder();

            pngEncoder.Frames.Add(BitmapFrame.Create(_curItemIcon));
            using (var imgStream = new MemoryStream())
            {
                pngEncoder.Save(imgStream);
                imgStream.Seek(0, SeekOrigin.Begin);
                return Convert.ToBase64String(imgStream.ToArray());
            }
        }

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
            MessageBox.Show("Invalid gold amount. Please try entering a smaller number or a numeric value.", "Invalid Amount Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return "0";
        }

        private void GoldAmountBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var isSuccessful = int.TryParse(GoldAmountBox.Text, out var num);
            if (isSuccessful) return;
            MessageBox.Show("Invalid gold amount. Please try entering a smaller number or a numeric value.", "Invalid Amount Error", MessageBoxButton.OK, MessageBoxImage.Error);
            GoldAmountBox.Text = "0";
        }

        private void ItemTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedType = (ComboBoxItem)e.AddedItems[0];

            if (selectedType.Content != null && selectedType.Content.ToString() != "N/A")
            {
                ItemStatusEffectsComboBox.IsEnabled = true;
                return;
            }
            ItemStatusEffectsComboBox.IsEnabled = false;
        }

        private void ItemStatusEffects_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private readonly string _curDir = AppDomain.CurrentDomain.BaseDirectory;

        private BitmapImage _curItemIcon = null;
    }
}
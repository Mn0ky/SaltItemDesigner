using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SaltItemDesigner;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window // TODO: Reorganize file structure.
{
    private readonly string _curDir = AppDomain.CurrentDomain.BaseDirectory;
    private static CustomItem _currentItem = new();
    private static readonly string[] ItemEffects = 
    {
        "Strength",
        "MaxHealth",
        "Health",
        "Weight",
        "MaxWeightSpeedReduction",
        "MaxFood",
        "CombatStaminaRegenRate",
        "CombatStaminaRecoveryDuration",
        "Armor",
        "SlashArmor",
        "PierceArmor",
        "PickArmor",
        "SpecialStamina",
        "BluntArmor",
        "HeatResist",
        "ColdResist",
        "CombatStamina",
        "MaxCombatStamina",
        "CritChance",
        "CritMultiplier",
        "ExplosionArmor",
        "Stunned",
        "Fishing",
        "PoisonResistance",
        "Adornment",
        "WalkVolume",
        "Concealment",
        "TeleportCooldown"
    };

    public MainWindow()
    {
        InitializeComponent();

        ObservableCollection<ItemRarity> rarityObjs = new()
        {
            new ItemRarity("FFFFFF", "Common"),
            new ItemRarity("5DA546", "Uncommon"),
            new ItemRarity("336F9A", "Rare"),
            new ItemRarity("6A3173", "Epic"),
            new ItemRarity("D77700", "Legendary")
        };

        ItemRarityComboBox.ItemsSource = rarityObjs;
        ItemRarityComboBox.SelectedIndex = 1;
        rarityObjs.CollectionChanged += RarityObjs_Changed;

        ItemStatusEffectsComboBox.ItemsSource = ItemEffects;
        ItemStatusEffectsComboBox.SelectedIndex = 19;
    }

    private void RarityObjs_Changed(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null) // Items have been added to the combobox
            Trace.Write("items added");

        else if (e.OldItems != null) // Items have been removed from the combobox
            Trace.Write("Items removed");
    }

    private void IconButton_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            InitialDirectory = "c:\\",
            Filter = "PNG Image Files (*.png)|*.png",
            RestoreDirectory = true
        };

        if (dlg.ShowDialog() != true) return;

        _currentItem.SetItemIcon(new Uri(dlg.FileName));
        ItemIcon.Source = _currentItem.ItemIcon;
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
        => Process.Start("explorer.exe", $"{_curDir}Items\\");

    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        var itemsDir = $"{_curDir}Items\\";
        if (!Directory.Exists(itemsDir)) 
            Directory.CreateDirectory(itemsDir);

        var dlg = new OpenFileDialog
        {
            InitialDirectory = itemsDir,
            Filter = "Item Files (*.item)|*.item",
            RestoreDirectory = true
        };

        if (dlg.ShowDialog() == true) ImportItemData(dlg.FileName);
    }

    private void ImportItemData(string filename)
    {
        _currentItem = CustomItem.DeserializeItem(filename);

        TitleTextBox.Text = _currentItem.ItemTitle;
        TitleTextBox.Foreground = _currentItem.ItemRarity.RarityColorBrush;
        FlareTextBox.Text = _currentItem.ItemFlareText;
        GoldAmountBox.Text = _currentItem.ItemPrice;
            
        ItemIcon.Source = _currentItem.ItemIcon;
        Trace.WriteLine("Deserialized item type: " + _currentItem.ItemType);
        Trace.WriteLine("Deserialized item effect: " + _currentItem.ItemStatusEffect);
        Trace.WriteLine("Deserialized item price: " + _currentItem.ItemPrice);

        switch (_currentItem.ItemType)
        {
            case "Consumable":
                ItemTypeComboBox.SelectedIndex = 1;
                ItemStatusEffectsComboBox.SelectedIndex = ItemStatusEffectsComboBox.Items.IndexOf(_currentItem.ItemStatusEffect);
                EffectAmountBox.Text = _currentItem.ItemEffectAmount;
                break;
            case "Wearable":
                Trace.WriteLine("wear");
                break;
            default:
                ItemTypeComboBox.SelectedIndex = 0;
                ItemStatusEffectsComboBox.SelectedIndex = 19;
                EffectAmountBox.Text = "00";
                break;
        }

        var importedItemRarity = _currentItem.ItemRarity;

        // TODO: Implement custom rarity support.
        for (var i = 0; i < ItemRarityComboBox.Items.Count; i++)
        {
            var rarity = (ItemRarity) ItemRarityComboBox.Items[i];

            if (rarity.Equals(importedItemRarity)) 
                ItemRarityComboBox.SelectedIndex = i;
        }
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        var itemsDir = $"{_curDir}Items\\";
        Trace.WriteLine(itemsDir);
            
        if (!Directory.Exists(itemsDir)) Directory.CreateDirectory(itemsDir);

        if (!string.IsNullOrEmpty(_currentItem.ItemTitle) && !string.IsNullOrEmpty(_currentItem.ItemFlareText) &&
            _currentItem.ItemIcon != null)
        {
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
                _currentItem.SerializeItem(dialog.FileName);
                Trace.WriteLine("Save success: " + dialog.FileName);
                MessageBox.Show("Item exported successfully.", "Export Success", MessageBoxButton.OK);
            }
        }
        else
        {
            MessageBox.Show(
                "Unable to export item. Please check your fields and or select an item icon before trying again.",
                "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ItemRarityComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems != null)
        {
            Trace.WriteLine("rarity combobox selection changed.");
            Trace.WriteLine("e length: " + e.AddedItems.Count);

            if (e.AddedItems.Count == 0) return;

            _currentItem.ItemRarity = (ItemRarity) e.AddedItems[0];

            TitleTextBox.Foreground = _currentItem.ItemRarity!.RarityColorBrush;
            Trace.WriteLine($"Color selected: {_currentItem.ItemRarity.RarityColorBrush}.");

            if (ItemRarityComboBox.Items.Contains(_currentItem.ItemRarity))
            {
                // Rarity value already exists in the combobox, assuming not custom. Setting it directly in case this was imported. 
                Trace.WriteLine("selectionchanged index: " + ItemRarityComboBox.Items.IndexOf(_currentItem.ItemRarity));
                ItemRarityComboBox.SelectedIndex = ItemRarityComboBox.Items.IndexOf(_currentItem.ItemRarity);
            }
            //if (ItemRarityComboBox.Items[ItemRarityComboBox.SelectedIndex].
        }
    }

    private void ItemTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItemType = (ComboBoxItem) e.AddedItems[0];

        if (selectedItemType.Content != null)
        {
            Trace.WriteLine("selected item type is: " + selectedItemType.Content);
            ItemStatusEffectsComboBox.IsEnabled = selectedItemType.Content.ToString() != "N/A";
            _currentItem.ItemType = selectedItemType.Content.ToString();
            Trace.WriteLine("new item type is: " + _currentItem.ItemType);
            return;
        }

        Trace.WriteLine("selected ItemType was null, nothing was changed...");
    }

    private void GoldUp_OnClick(object sender, RoutedEventArgs e)
    {
        _currentItem.ItemPrice = ModifyGoldAmount(GoldAmountBox.Text, true);
        GoldAmountBox.Text = _currentItem.ItemPrice;
    }

    private void GoldDown_OnClick(object sender, RoutedEventArgs e)
    {
        _currentItem.ItemPrice = ModifyGoldAmount(GoldAmountBox.Text, false);
        GoldAmountBox.Text = _currentItem.ItemPrice;
    }

    private static string ModifyGoldAmount(string amount, bool upOrDown)
    {
        var isSuccessful = int.TryParse(amount, out var amountNum);
        if (isSuccessful) return upOrDown ? (amountNum + 1).ToString() : (amountNum - 1).ToString();

        MessageBox.Show("Invalid gold amount. Please try entering a smaller number or a numeric value.",
            "Invalid Amount Error", MessageBoxButton.OK, MessageBoxImage.Error);

        return "0";
    }

    private void EffectAmountBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var isSuccessful = int.TryParse(EffectAmountBox.Text, out var num);
        if (isSuccessful)
        {
            _currentItem.ItemEffectAmount = EffectAmountBox.Text;
            return;
        }

        MessageBox.Show("Invalid effect amount. Please try entering a smaller number or a numeric value.",
            "Invalid Amount Error", MessageBoxButton.OK, MessageBoxImage.Error);

        EffectAmountBox.Text = "0";
        _currentItem.ItemEffectAmount = EffectAmountBox.Text;
    }

    private void GoldAmountBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var isSuccessful = int.TryParse(GoldAmountBox.Text, out _);
        if (isSuccessful) return;

        MessageBox.Show("Invalid gold amount. Please try entering a smaller number or a numeric value.",
            "Invalid Amount Error", MessageBoxButton.OK, MessageBoxImage.Error);

        GoldAmountBox.Text = "0";
    }

    private void ItemStatusEffects_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Trace.WriteLine("Itemstatuseff box selected");
        string selectedItemEffect = (string) e.AddedItems[0];

        if (selectedItemEffect != null)
        {
            Trace.WriteLine($"Changed item effect: {selectedItemEffect}");
            _currentItem.ItemStatusEffect = selectedItemEffect;
        }
    }

    private void TitleTextBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) 
        => _currentItem.ItemTitle = TitleTextBox.Text;

    private void FlareTextBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        => _currentItem.ItemFlareText = FlareTextBox.Text;
}
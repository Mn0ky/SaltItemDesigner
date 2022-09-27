using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;

namespace SaltItemDesigner;

[Serializable]
public class CustomItem
{
    private byte[] _itemIconBytes;
    
    [field: NonSerialized] public BitmapImage ItemIcon { get; private set; }
    public string ItemTitle { get; set; } = "";
    public string ItemFlareText { get; set; } = "";
    public string ItemPrice { get; set; } = "";
    public string ItemType { get; set; } = "";
    public string ItemStatusEffect { get; set; } = "";
    public string ItemEffectAmount { get; set; } = "";
    public ItemRarity ItemRarity = null;

    // Used when importing an item file
    public void SetItemIconFromBytes()
    {
        ItemIcon = new BitmapImage();

        ItemIcon.BeginInit();
        ItemIcon.StreamSource = new MemoryStream(_itemIconBytes);

        ItemIcon.DecodePixelHeight = 64;
        ItemIcon.DecodePixelWidth = 64;

        ItemIcon.EndInit();
        ItemIcon.Freeze();
    }

    // Used when creating a new item
    public void SetItemIcon(Uri imageUri)
    {
        ItemIcon = new BitmapImage();

        ItemIcon.BeginInit();
        ItemIcon.UriSource = imageUri;

        ItemIcon.DecodePixelWidth = 64;
        ItemIcon.DecodePixelHeight = 64;

        ItemIcon.EndInit();
        ItemIcon.Freeze();
    }

    public void SerializeItem(string filePath)
    {
        PngBitmapEncoder pngEncoder = new();
        pngEncoder.Frames.Add(BitmapFrame.Create(ItemIcon));

        using (MemoryStream imageMs = new())
        {
            pngEncoder.Save(imageMs);
            _itemIconBytes = imageMs.ToArray();
        }

        using Stream fileStream = File.OpenWrite(filePath);
        var formatter = new BinaryFormatter();

#pragma warning disable SYSLIB0011
        formatter.Serialize(fileStream, this);
#pragma warning restore SYSLIB0011

        fileStream.Flush();
        fileStream.Dispose();
    }

    public static CustomItem DeserializeItem(string filePath)
    {
        using Stream fileStream = File.OpenRead(filePath);
        var formatter = new BinaryFormatter();

#pragma warning disable SYSLIB0011
        var importedItem = (CustomItem) formatter.Deserialize(fileStream);
#pragma warning restore SYSLIB0011
        
        importedItem.ItemRarity.GenerateColorBrush();
        importedItem.SetItemIconFromBytes();

        fileStream.Flush();
        fileStream.Dispose();

        return importedItem;
    }
}
using System;
using System.Windows.Media;

namespace SaltItemDesigner;

[Serializable]
public class ItemRarity
{
    [field: NonSerialized] public SolidColorBrush RarityColorBrush { get; private set; }
    public string RarityName { get; }
    public string RarityColorHex { get; }

    public ItemRarity(string rarityColorHex, string rarityName)
    {
        var rarityColorHexBytes = Convert.FromHexString(rarityColorHex);

        RarityColorBrush = new SolidColorBrush(Color.FromRgb(rarityColorHexBytes[0],
            rarityColorHexBytes[1],
            rarityColorHexBytes[2]));
        
        RarityName = rarityName;
        RarityColorHex = rarityColorHex;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (!(GetType() == obj.GetType())) return false;
        
        var comparedRarity = obj as ItemRarity;
        
        return comparedRarity!.RarityName == RarityName &&
               comparedRarity.RarityColorHex == RarityColorHex &&
               comparedRarity.RarityColorBrush.Color == RarityColorBrush.Color;
    }   

    public void GenerateColorBrush()
    {
        var rarityColorHexBytes = Convert.FromHexString(RarityColorHex);
        
        RarityColorBrush = new SolidColorBrush(Color.FromRgb(rarityColorHexBytes[0],
            rarityColorHexBytes[1],
            rarityColorHexBytes[2]));
    }
}
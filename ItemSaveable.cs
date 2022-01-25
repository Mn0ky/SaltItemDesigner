using System;
using System.Windows.Media;

namespace SaltItemDesigner
{
    class ItemSaveable
    {
        public string ItemTitle { get; set; } = "";

        public string ItemFlareText { get; set; } = "";

        public string ItemPrice { get; set; } = "";

        public string ItemIcon { get; set; } = "";

        public string ItemRarityColorHex { get; set; } = "";

        public string ItemRarityName { get; set; } = "";

        // public ItemRarity ItemRarityInfo{ get; set; }

        // public ItemRarity(string rarityColorHex, string rarityName)
        // {
        //     var rarityColorHexBytes = Convert.FromHexString(rarityColorHex.Remove(0, 1));
        //     RarityColor = new SolidColorBrush(Color.FromRgb(rarityColorHexBytes[0], rarityColorHexBytes[1], rarityColorHexBytes[2]));
        //     RarityName = rarityName;
        // }
    }
}

using System;
using System.Windows.Media;

namespace SaltItemDesigner
{
    class ItemRarity
    {
        public SolidColorBrush RarityColor { get; set; }

        public string RarityName { get; set; }

        public ItemRarity(string rarityColorHex, string rarityName)
        {
            var rarityColorHexBytes = Convert.FromHexString(rarityColorHex.Remove(0, 1));
            RarityColor = new SolidColorBrush(Color.FromRgb(rarityColorHexBytes[0], rarityColorHexBytes[1], rarityColorHexBytes[2]));
            RarityName = rarityName;
        }
    }
}

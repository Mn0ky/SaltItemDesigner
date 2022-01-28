using System;
using System.Windows.Media;

namespace SaltItemDesigner
{
    class StatusEffect
    {
        //public SolidColorBrush RarityColor { get; set; }

        public string EffectType { get; set; } = "";

        public string EffectImage { get; set; } = "";

        //public ItemRarity(string rarityColorHex, string rarityName)
        //{
        //    var rarityColorHexBytes = Convert.FromHexString(rarityColorHex.Remove(0, 1));
        //    RarityColor = new SolidColorBrush(Color.FromRgb(rarityColorHexBytes[0], rarityColorHexBytes[1], rarityColorHexBytes[2]));
        //    RarityName = rarityName;
        //    RarityColorHex = rarityColorHex;
        //}
    }
}
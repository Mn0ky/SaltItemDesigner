using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SaltItemDesigner
{
    class ItemSaveable
    {
        private BitmapImage _itemIcon;

        public string ItemTitle { get; set; } = "";

        public string ItemFlareText { get; set; } = "";

        public string ItemPrice { get; set; } = "";

        public string ItemType { get; set; } = "";

        public string ItemStatusEffect { get; set; } = "";

        public string ItemEffectAmount { get; set; } = "";

        public string ItemIcon
        {
            set
            {
                byte[] imgData = Convert.FromBase64String(value);

                _itemIcon = new BitmapImage();
                _itemIcon.BeginInit();
                _itemIcon.StreamSource = new MemoryStream(imgData);
                _itemIcon.DecodePixelHeight = 64;
                _itemIcon.DecodePixelWidth = 64;
                _itemIcon.EndInit();
                _itemIcon.Freeze();

                //var scale = 64d / imgBitMap.PixelWidth;
                //var imgBitMapResized = new WriteableBitmap(new TransformedBitmap(imgBitMap, new ScaleTransform(scale, scale)));
                //var imgBitMapResized = new WriteableBitmap(imgBitMap);
            }
        }

        public BitmapImage ItemIconBitmapImage => _itemIcon;

        public string ItemRarityColorHex { get; set; } = "";

        public string ItemRarityName { get; set; } = "";

    }
}

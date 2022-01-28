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

        public string ItemIcon
        {
            set
            {
                byte[] imgData = Convert.FromBase64String(value);

                BitmapImage imgBitMap = new BitmapImage();
                imgBitMap.BeginInit();
                imgBitMap.StreamSource = new MemoryStream(imgData);
                imgBitMap.CacheOption = BitmapCacheOption.OnLoad;
                imgBitMap.EndInit();

                _itemIcon = imgBitMap;
            }
        }

        public BitmapImage ItemIconBitmapImage
        {
            get
            {
                return _itemIcon;
            }
        }

        public string ItemRarityColorHex { get; set; } = "";

        public string ItemRarityName { get; set; } = "";

    }
}
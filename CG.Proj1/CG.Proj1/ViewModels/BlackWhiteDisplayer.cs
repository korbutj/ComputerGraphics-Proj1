using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Mvvm;

namespace CG.Proj1.ViewModels
{
    public class BlackWhiteDisplayer : BindableBase
    {
        private Uri imgPathUri;
        public Uri ImgPathUri
        {
            get { return imgPathUri; }
            set
            {
                SetProperty(ref imgPathUri, value);
                unsafe
                {
                    Image = new WriteableBitmap(new BitmapImage(ImgPathUri));
                    var formatted = new FormatConvertedBitmap();
                    formatted.BeginInit();
                    formatted.Source = Image;
                    formatted.DestinationFormat = PixelFormats.Gray8;
                    formatted.DestinationPalette = BitmapPalettes.Gray256;
                    formatted.EndInit();
                    ConvertedImageSource = new WriteableBitmap(formatted);
                }
            }
        }


        private WriteableBitmap image;
        public WriteableBitmap Image
        {
            get { return image; }
            set { SetProperty(ref image, value); }
        }

        private WriteableBitmap convertedImageSource;
        public WriteableBitmap ConvertedImageSource
        {
            get { return convertedImageSource; }
            set { SetProperty(ref convertedImageSource, value); }
        }

        private bool ImageValid()
        {
            return ConvertedImageSource != null;
        }
    }
}
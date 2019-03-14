using System;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Commands;
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

        public ICommand ThresholdCommand { get; }

        public BlackWhiteDisplayer()
        {
            ThresholdCommand = new DelegateCommand(() => Thresholding(126), () => Image != null)
                .ObservesProperty(() => Image);
        }

        public void Thresholding(int value)
        {
            unsafe
            {
                var copy = new WriteableBitmap(ConvertedImageSource);
                copy.Lock();
                ConvertedImageSource.Lock();
                var copyPtr = (byte*)copy.BackBuffer;
                var truPtr = (byte*)ConvertedImageSource.BackBuffer;
                for (int y = 0; y < ConvertedImageSource.PixelHeight; y++)
                {
                    for (int x = 0; x < ConvertedImageSource.PixelWidth; x++)
                    {
                        var byteOffset = (x * 3) + (y * ConvertedImageSource.BackBufferStride);
                        copyPtr[byteOffset] = truPtr[byteOffset] > value ? (byte)255 : (byte)0;
                        copyPtr[byteOffset + 1] = truPtr[byteOffset + 1] > value ? (byte)255 : (byte)0;
                        copyPtr[byteOffset + 2] = truPtr[byteOffset + 2] > value ? (byte)255 : (byte)0;
                    }
                }
                ConvertedImageSource.Unlock();
                ConvertedImageSource = copy;
                ConvertedImageSource.Unlock();
            }
        }
    }
}
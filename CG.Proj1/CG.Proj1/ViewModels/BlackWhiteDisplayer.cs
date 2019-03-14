using System;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Commands;
using Prism.Mvvm;

namespace CG.Proj1.ViewModels
{
    //TODO - Project assignment - average dithering + octree q
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
                    //formatted.DestinationFormat = PixelFormats.Gray8;
                    switch (GrayColors)
                    {
                        case 2:
                            formatted.DestinationFormat = PixelFormats.BlackWhite;
                            formatted.DestinationPalette = BitmapPalettes.BlackAndWhite;
                            break;

                        case 4:
                            formatted.DestinationFormat = PixelFormats.Gray2;
                            formatted.DestinationPalette = BitmapPalettes.Gray4;
                            break;

                        case 16:
                            formatted.DestinationFormat = PixelFormats.Gray4;
                            formatted.DestinationPalette = BitmapPalettes.Gray16;
                            break;

                    }
                    formatted.EndInit();
                    ConvertedImageSource = new WriteableBitmap(formatted);
                }
            }
        }

        private int grayColors;
        public int GrayColors
        {
            get
            {
                return grayColors;
            }
            set
            {
                SetProperty(ref grayColors, value);
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
            GrayColors = 2;
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
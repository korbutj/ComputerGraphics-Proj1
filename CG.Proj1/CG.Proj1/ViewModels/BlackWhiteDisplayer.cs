using System;
using System.Drawing;
using System.Linq;
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
                    ConvertImage();
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
                if (Image != null)
                {
                    ConvertImage();
                }
            }
        }

        private double tParameter;
        public double TParameter
        {
            get { return tParameter; }
            set { SetProperty(ref tParameter, value); }
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
            ThresholdCommand = new DelegateCommand(Thresholding, () => Image != null)
                .ObservesProperty(() => Image);
            GrayColors = 2;
            TParameter = 1;
        }

        private void ConvertImage()
        {
            var formatted = new FormatConvertedBitmap();
            formatted.BeginInit();
            formatted.Source = Image;
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

        public void Thresholding()
        {
            unsafe
            {
                var borders = new byte[GrayColors];
                for (int i = 1; i < GrayColors + 1; i++)
                {
                    borders[i - 1] = (byte)(255 * i / GrayColors - 1);
                }
                var copy = new WriteableBitmap(ConvertedImageSource);
                copy.Lock();
                ConvertedImageSource.Lock();
                var copyPtr = (byte*)copy.BackBuffer;
                var truPtr = (byte*)ConvertedImageSource.BackBuffer;
                for (int y = 0; y < copy.PixelHeight; y++)
                {
                    for (int x = 0; x < copy.PixelWidth; x++)
                    {
                        var byteOffset = (x * 3) + (y * copy.BackBufferStride);

                        if (copyPtr[byteOffset] == 255)
                            copyPtr[byteOffset] = borders[borders.Length - 1];
                        var index = (int)((copyPtr[byteOffset] * (GrayColors - 1))/ 255);
                        if (copyPtr[byteOffset] < (borders[index] + ((borders[index + 1] - borders[index]) * TParameter)))
                        {
                            copyPtr[byteOffset] = borders[index];
                        }
                        else
                        {
                            copyPtr[byteOffset] = borders[index + 1];
                        }

                        if (copyPtr[byteOffset + 1] == 255)
                            copyPtr[byteOffset + 1] = borders[borders.Length - 1];
                        var index1 = (int)((copyPtr[byteOffset + 1] * (GrayColors - 1) )/ 255);
                        if (copyPtr[byteOffset+1] < (borders[index1] + ((borders[index1 + 1] - borders[index1]) * TParameter)))
                        {
                            copyPtr[byteOffset+1] = borders[index1];
                        }
                        else
                        {
                            copyPtr[byteOffset+1] = borders[index1 + 1];
                        }

                        if (copyPtr[byteOffset+2] == 255)
                            copyPtr[byteOffset+2] = borders[borders.Length - 1];
                        var index2 = (int)((copyPtr[byteOffset + 2] * (GrayColors - 1)) / 255);
                        if (copyPtr[byteOffset+2] < (borders[index2] + ((borders[index2 + 1] - borders[index2]) * TParameter)))
                        {
                            copyPtr[byteOffset+2] = borders[index2];
                        }
                        else
                        {
                            copyPtr[byteOffset+2] = borders[index2 + 1];
                        }
                        //copyPtr[byteOffset] = copyPtr[byteOffset] > 127 ? (byte)255 : (byte)0;
                        //copyPtr[byteOffset+1] = (byte) copyPtr[byteOffset+1] > 127 ? (byte)255 : (byte)0;
                        //copyPtr[byteOffset+2] = (byte) copyPtr[byteOffset+2] > 127 ? (byte)255 : (byte)0;
                        //ThresholdPixel(ref copyPtr[byteOffset], borders);
                        //ThresholdPixel(ref copyPtr[byteOffset + 1], borders);
                        //ThresholdPixel(ref copyPtr[byteOffset + 2], borders);
                    }
                }
                ConvertedImageSource.Unlock();
                ConvertedImageSource = copy;
                ConvertedImageSource.Unlock();
            }
        }

        private void ThresholdPixel(ref byte pixel, byte[] borders)
        {
            if (pixel == 255)
                pixel = borders[borders.Length - 1];
            var index = (int)(pixel * (GrayColors - 1) / 255);
            if(pixel < (borders[index] + ((borders[index+1] - borders[index])*TParameter)))
            {
                pixel = borders[index];
            }
            else
            {
                pixel = borders[index+1];
            }
        }
    }
}
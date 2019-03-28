using System;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CG.Proj1.BaseClass;
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
                    Image = new WriteableBitmap(new BitmapImage(imgPathUri));
                    var copy = new WriteableBitmap(Image);
                    copy.Lock();
                    var copyPtr = (byte*) copy.BackBuffer;
//                    for (int y = 0; y < copy.PixelHeight; y++)
//                    {
//                        for (int x = 0; x < copy.PixelWidth; x++)
//                        {
//                            PixelToGrayscale(x, y, Image, copyPtr);
//                        }
//                    }

                    copy.Unlock();
                    Image = new WriteableBitmap(copy);
                    ConvertedImageSource = new WriteableBitmap(copy);
                }
            }
        }

        private static unsafe void PixelToGrayscale(int x, int y, WriteableBitmap copy, byte* copyPtr)
        {
            var bytesPerPixel = copy.Format.BitsPerPixel / 8;
            var byteOffset = (x * bytesPerPixel) + (y * copy.BackBufferStride);
            var pixel = (byte) (0.299 * copyPtr[byteOffset] + 0.587 * copyPtr[byteOffset + 1] +
                                0.114 * copyPtr[byteOffset + 2]);
            for (int i = 0; i < bytesPerPixel; i++)
            {
                copyPtr[byteOffset + i] = pixel;
            }

//            copyPtr[byteOffset + 1] = pixel;
//            copyPtr[byteOffset + 2] = pixel;
        }

        private int grayColors;

        public int GrayColors
        {
            get { return grayColors; }
            set { SetProperty(ref grayColors, value); }
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
        public ICommand OcTreeCommand { get; }

        public BlackWhiteDisplayer()
        {
            ThresholdCommand = new DelegateCommand(() => Thresholding(126), () => Image != null)
                .ObservesProperty(() => Image);
            OcTreeCommand = new DelegateCommand(() => Octree(2), () => Image != null)
                .ObservesProperty(() => Image);
            GrayColors = 2;
        }

        private void Octree(int level)
        {
            var OcTree = new OctoNode(0);
            unsafe
            {
                var copy = new WriteableBitmap(Image);
                copy.Lock();
                ConvertedImageSource.Lock();
                var copyPtr = (byte*) copy.BackBuffer;
                var truPtr = (byte*) Image.BackBuffer;
                for (int y = 0; y < Image.PixelHeight; y++)
                {
                    for (int x = 0; x < Image.PixelWidth; x++)
                    {
                        var bytesPerPixel = Image.Format.BitsPerPixel / 8;
                        var byteOffset = (x * bytesPerPixel) + (y * Image.BackBufferStride);
                        OcTree.InsertColor(truPtr[byteOffset], truPtr[byteOffset + 1], truPtr[byteOffset + 2]);
                    }
                }

                OcTree.Reduce(level);

                for (int y = 0; y < Image.PixelHeight; y++)
                {
                    for (int x = 0; x < Image.PixelWidth; x++)
                    {
                        var bytesPerPixel = Image.Format.BitsPerPixel / 8;
                        var byteOffset = (x * bytesPerPixel) + (y * Image.BackBufferStride);
                        (copyPtr[byteOffset], copyPtr[byteOffset + 1], copyPtr[byteOffset + 2]) =
                            OcTree.GetColor(truPtr[byteOffset], truPtr[byteOffset + 1], truPtr[byteOffset + 2]);
                    }
                }


                ConvertedImageSource.Unlock();
                ConvertedImageSource = copy;
                ConvertedImageSource.Unlock();
            }
        }

        public void Thresholding(int value)
        {
            var copy = new WriteableBitmap(Image);
            unsafe
            {
                var copyPtr = (byte*) copy.BackBuffer;
                for (int y = 0; y < copy.PixelHeight; y++)
                {
                    for (int x = 0; x < copy.PixelWidth; x++)
                    {
                        PixelToGrayscale(x, y, Image, copyPtr);
                    }
                }
                copy.Unlock();
                var borders = new byte[GrayColors];
                for (int i = 0; i < GrayColors; i++)
                {
                    borders[i] = (byte) ((255 * i) / (GrayColors - 1));
                }

                ConvertedImageSource.Lock();
                var truPtr = (byte*) Image.BackBuffer;
                for (int y = 0; y < Image.PixelHeight; y++)
                {
                    for (int x = 0; x < Image.PixelWidth; x++)
                    {
                        Threshold(x, y, copyPtr, truPtr, borders, 127);
                    }
                }

                ConvertedImageSource.Unlock();
                ConvertedImageSource = copy;
                ConvertedImageSource.Unlock();
            }
        }

        private unsafe void Threshold(int x, int y, byte* copyPtr, byte* truPtr, byte[] borders, byte borderValue)
        {
            var bytesPerPixel = Image.Format.BitsPerPixel / 8;
            var byteOffset = (x * bytesPerPixel) + (y * Image.BackBufferStride);
            for (int i = 0; i < bytesPerPixel; i++)
            {
                copyPtr[byteOffset + i] = (byte) (truPtr[byteOffset + i] > borderValue ? 255 : 0);
                ;
            }
        }

        private byte ThresholdPixel(byte pixelValue, byte[] borders)
        {
            if (pixelValue == 255)
                return borders.Last();
            var index = (int) ((pixelValue * (GrayColors - 1)) / 255);
            return borders[index];
        }
    }
}
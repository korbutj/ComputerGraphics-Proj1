using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;

namespace CG.Proj1.ViewModels
{
    public class ImageDisplayerViewModel : BindableBase
    {
        public BitmapImage Image { get; set; }
        private byte[] rawImage;
        public byte[] RawImage
        {
            get => rawImage;
            set => SetProperty(ref rawImage, value);
        }

        private WriteableBitmap convertedImageSource;
        public WriteableBitmap ConvertedImageSource
        {
            get => convertedImageSource;
            set => SetProperty(ref convertedImageSource, value);
        }

        public ICommand InverseCommand { get; set; }
        public ICommand BrightnessMinusCommand { get; set; }
        public ICommand BrightnessPlusCommand { get; set; }
        public ICommand ContrastCommand { get; set; }


        public ImageDisplayerViewModel(Uri imgPath)
        {
            Image = new BitmapImage(imgPath);
            ConvertedImageSource = new WriteableBitmap(Image);
            RawImage = ImageSourceToBytes(new PngBitmapEncoder(), Image);
            InverseCommand = new DelegateCommand(InverseBytes, ImageValid)
                .ObservesProperty(() => RawImage);
            BrightnessMinusCommand = new DelegateCommand(() => BrightnessCorrection(-30), ImageValid)
                .ObservesProperty(() => RawImage);
            BrightnessPlusCommand = new DelegateCommand(() => BrightnessCorrection(+30), ImageValid)
                .ObservesProperty(() => RawImage);
            ContrastCommand = new DelegateCommand(() => ContrastEnhc(1.3), ImageValid)
                .ObservesProperty(() => RawImage);
        }

        public byte[] ImageSourceToBytes(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            if (imageSource is BitmapSource bitmapSource)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

        private void InverseBytes()
        {
            ConvertedImageSource = new WriteableBitmap(Image);
            unsafe
            {
                ConvertedImageSource.Lock();
                var buffer = ConvertedImageSource.BackBuffer;
                int pixelHeight = ConvertedImageSource.PixelHeight;
                int pixelWidth = ConvertedImageSource.PixelWidth;
                int channels = 4;
                unsafe
                {
                    for (int r = 0; r < pixelHeight; r++)
                    {
                        byte* pixels = (byte*)buffer.ToPointer() + r * pixelWidth * channels; 
                        for (int c = 0; c < pixelWidth; c++)
                        {
                            pixels[c * channels] = (byte)(byte.MaxValue - pixels[c * channels]);
                            pixels[c * channels + 1] = (byte) (byte.MaxValue - pixels[c * channels + 1]);
                            pixels[c * channels + 2] = (byte)(byte.MaxValue - pixels[c * channels + 2]);
                            pixels[c * channels + 3] = 255;

                        }
                    }
                }
                ConvertedImageSource.AddDirtyRect(new Int32Rect(0,0,ConvertedImageSource.PixelWidth,ConvertedImageSource.PixelHeight));
                ConvertedImageSource.Unlock();
            }
            RaisePropertyChanged(nameof(ConvertedImageSource));
        }

        private void BrightnessCorrection(int correction)
        {
            ConvertedImageSource = new WriteableBitmap(Image);
            unsafe
            {
                ConvertedImageSource.Lock();
                var buffer = ConvertedImageSource.BackBuffer;
                int pixelHeight = ConvertedImageSource.PixelHeight;
                int pixelWidth = ConvertedImageSource.PixelWidth;
                int channels = 4;
                unsafe
                {
                    for (int r = 0; r < pixelHeight; r++)
                    {
                        byte* pixels = (byte*)buffer.ToPointer() + r * pixelWidth * channels;
                        for (int c = 0; c < pixelWidth; c++)
                        {
                            pixels[c * channels] = (byte)(pixels[c * channels] + correction > 0 ? (pixels[c * channels] + correction < 255 ? pixels[c * channels] + correction : 255) : 0);
                            pixels[c * channels + 1] = (byte)(pixels[c * channels + 1] + correction > 0 ? (pixels[c * channels + 1] + correction < 255 ? pixels[c * channels + 1] + correction : 255) : 0);
                            pixels[c * channels + 2] = (byte)(pixels[c * channels + 2] + correction > 0 ? (pixels[c * channels + 2] + correction < 255 ? pixels[c * channels + 2] + correction : 255) : 0);
                            pixels[c * channels + 3] = 255;

                        }
                    }
                }
                ConvertedImageSource.AddDirtyRect(new Int32Rect(0, 0, ConvertedImageSource.PixelWidth, ConvertedImageSource.PixelHeight));
                ConvertedImageSource.Unlock();
            }
            RaisePropertyChanged(nameof(ConvertedImageSource));
        }
        private void ContrastEnhc(double slope)
        {
            ConvertedImageSource = new WriteableBitmap(Image);
            unsafe
            {
                ConvertedImageSource.Lock();
                var buffer = ConvertedImageSource.BackBuffer;
                int pixelHeight = ConvertedImageSource.PixelHeight;
                int pixelWidth = ConvertedImageSource.PixelWidth;
                int channels = 4;
                unsafe
                {
                    for (int r = 0; r < pixelHeight; r++)
                    {
                        byte* pixels = (byte*)buffer.ToPointer() + r * pixelWidth * channels;
                        for (int c = 0; c < pixelWidth; c++)
                        {
                            pixels[c * channels] = (byte)(pixels[c * channels]*slope > 0 ? (pixels[c * channels] * slope < 255 ? pixels[c * channels] * slope : 255) : 0);
                            pixels[c * channels + 1] = (byte)(pixels[c * channels + 1]*slope > 0 ? (pixels[c * channels + 1] * slope < 255 ? pixels[c * channels + 1] * slope : 255) : 0);
                            pixels[c * channels + 2] = (byte)(pixels[c * channels + 2]*slope > 0 ? (pixels[c * channels + 2] * slope < 255 ? pixels[c * channels + 2] * slope : 255) : 0);
                            pixels[c * channels + 3] = 255;

                        }
                    }
                }
                ConvertedImageSource.AddDirtyRect(new Int32Rect(0, 0, ConvertedImageSource.PixelWidth, ConvertedImageSource.PixelHeight));
                ConvertedImageSource.Unlock();
            }
            RaisePropertyChanged(nameof(ConvertedImageSource));
        }

        private bool ImageValid()
        {
            return RawImage != null;
        }


        private BitmapSource newImage(byte[] pixels)
        {
            int width = Image.PixelWidth;
            int height = Image.PixelHeight;
            var stride = Image.PixelWidth * 4;
            //var mod = stride % 4;
            //if (mod != 0) stride += 4 - mod;

            // Define the image palette
            BitmapPalette myPalette = BitmapPalettes.Halftone256Transparent;

            // Creates a new empty image with the pre-defined palette

            BitmapSource image = BitmapSource.Create(
                width,
                height,
                Image.DpiX,
                Image.DpiY,
                Image.Format,
                BitmapPalettes.Halftone256,
                pixels,
                stride);

            return image;
        }
    }
}
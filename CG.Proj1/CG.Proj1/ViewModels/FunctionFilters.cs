using System.Windows;
using System.Windows.Media.Imaging;

namespace CG.Proj1.ViewModels
{
    public partial class ImageDisplayerViewModel
    {
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
                            pixels[c * channels + 1] = (byte)(byte.MaxValue - pixels[c * channels + 1]);
                            pixels[c * channels + 2] = (byte)(byte.MaxValue - pixels[c * channels + 2]);
                            pixels[c * channels + 3] = 255;

                        }
                    }
                }
                ConvertedImageSource.AddDirtyRect(new Int32Rect(0, 0, ConvertedImageSource.PixelWidth, ConvertedImageSource.PixelHeight));
                ConvertedImageSource.Unlock();
            }
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

        private void ContrastEnhancment(double slope)
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
                            pixels[c * channels] = (byte)(pixels[c * channels] * slope > 0 ? (pixels[c * channels] * slope < 255 ? pixels[c * channels] * slope : 255) : 0);
                            pixels[c * channels + 1] = (byte)(pixels[c * channels + 1] * slope > 0 ? (pixels[c * channels + 1] * slope < 255 ? pixels[c * channels + 1] * slope : 255) : 0);
                            pixels[c * channels + 2] = (byte)(pixels[c * channels + 2] * slope > 0 ? (pixels[c * channels + 2] * slope < 255 ? pixels[c * channels + 2] * slope : 255) : 0);
                            pixels[c * channels + 3] = 255;

                        }
                    }
                }
                ConvertedImageSource.AddDirtyRect(new Int32Rect(0, 0, ConvertedImageSource.PixelWidth, ConvertedImageSource.PixelHeight));
                ConvertedImageSource.Unlock();
            }
            RaisePropertyChanged(nameof(ConvertedImageSource));
        }
    }
}
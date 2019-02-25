using System.Windows;
using System.Windows.Media.Imaging;

namespace CG.Proj1.ViewModels
{
    public partial class ImageDisplayerViewModel
    {
        private void ConvolutionBlur()
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
                    var offsetX = 1;
                    var offsetY = 1;
                    for (int row = 0; row < pixelHeight; row++)
                    {
                        byte* pixels = (byte*)buffer.ToPointer() + row * pixelWidth * channels;
                        for (int c = 0; c < pixelWidth; c++)
                        {
                            pixels[c * channels] = 0;
                            pixels[c * channels + 1] = 0;
                            pixels[c * channels + 2] = 0;
                            pixels[c * channels + 3] = 255;

                            for (int i = -offsetX; i < offsetX; i++)
                            {

                            }
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
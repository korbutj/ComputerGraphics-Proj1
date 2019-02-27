using System.Windows.Media.Imaging;

namespace CG.Proj1.BaseClass
{
    public abstract class ConvolutionFilterBase
    {
        public abstract string FilterName
        {
            get;
        }


        public abstract double Factor
        {
            get;
        }


        public abstract double Bias
        {
            get;
        }


        public abstract double[,] FilterMatrix
        {
            get;
        }


    }

    public static class ExtensionsWriteableBitmap
    {
        public static WriteableBitmap ConvolutionFilter<T>(this WriteableBitmap sourceBitmap, T filter)
                                where T : ConvolutionFilterBase
        {
            unsafe
            {
                var copy = new WriteableBitmap(sourceBitmap);
                sourceBitmap.Lock();
                copy.Lock();
                var pixelBuffer = (byte*)sourceBitmap.BackBuffer;
                var resultBuffer = (byte*)copy.BackBuffer;

                var blue = 0.0;
                var green = 0.0;
                var red = 0.0;

                int filterWidth = filter.FilterMatrix.GetLength(1);
                int filterHeight = filter.FilterMatrix.GetLength(0);


                int filterOffset = (filterWidth - 1) / 2;
                int calcOffset = 0;


                var byteOffset = 0;


                for (var offsetY = filterOffset; offsetY <
                                                 sourceBitmap.Height - filterOffset; offsetY++)
                {
                    for (var offsetX = filterOffset; offsetX <
                                                     sourceBitmap.Width - filterOffset; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;


                        for (var filterY = -filterOffset;
                            filterY <= filterOffset; filterY++)
                        {
                            for (var filterX = -filterOffset;
                                filterX <= filterOffset; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterOffset,
                                            filterX + filterOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterOffset,
                                             filterX + filterOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterOffset,
                                           filterX + filterOffset];
                            }
                        }


                        blue = filter.Factor * blue + filter.Bias;
                        green = filter.Factor * green + filter.Bias;
                        red = filter.Factor * red + filter.Bias;


                        if (blue > 255)
                        { blue = 255; }
                        else if (blue < 0)
                        { blue = 0; }


                        if (green > 255)
                        { green = 255; }
                        else if (green < 0)
                        { green = 0; }


                        if (red > 255)
                        { red = 255; }
                        else if (red < 0)
                        { red = 0; }


                        resultBuffer[byteOffset] = (byte)(blue);
                        resultBuffer[byteOffset + 1] = (byte)(green);
                        resultBuffer[byteOffset + 2] = (byte)(red);
                        resultBuffer[byteOffset + 3] = 255;
                    }
                }
                sourceBitmap.Unlock();
                copy.Unlock();
                return copy;
            }
        }
    }
}
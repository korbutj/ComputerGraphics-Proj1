using System;
using System.Windows.Media.Imaging;

namespace CG.Proj1.BaseClass
{
    public abstract class ConvolutionFilterBase
    {
        public abstract string FilterName
        {
            get;
        }

        protected double factor;
        public abstract double Factor { get; set; }

        protected double bias;

        public abstract double Bias { get; set; }

        protected double[,] filterMatrix;
        public abstract double[,] FilterMatrix { get; set; }
    }

    public static class ExtensionsWriteableBitmap
    {
        private static WriteableBitmap CreateCopyWithBorder<T>(WriteableBitmap sourceBitmap, T filter) where T : ConvolutionFilterBase
        {
            unsafe
            {
                int filterWidth = filter.FilterMatrix.GetLength(1);
                int filterHeight = filter.FilterMatrix.GetLength(0);


                int filterWidthOffset = (filterWidth - 1) / 2;
                int filterHeightOffset = (filterHeight - 1) / 2;
                var copyAdd = new WriteableBitmap(sourceBitmap.PixelWidth + filterWidthOffset * 2,
                    sourceBitmap.PixelHeight + filterHeightOffset * 2, sourceBitmap.DpiX, sourceBitmap.DpiY,
                    sourceBitmap.Format, sourceBitmap.Palette);

                var addPtr = (byte*) copyAdd.BackBuffer;
                var truPtr = (byte*) sourceBitmap.BackBuffer;
                copyAdd.Lock();

                //LeftUpperCorner
                for (int y = 0; y < filterHeightOffset; y++)
                {
                    for (int x = 0; x < filterWidthOffset; x++)
                    {
                        var offsetCopy = y *
                                         copyAdd.BackBufferStride +
                                         x * 4;

                        addPtr[offsetCopy] = truPtr[0];
                        addPtr[offsetCopy+1] = truPtr[1];
                        addPtr[offsetCopy+2] = truPtr[2];
                        addPtr[offsetCopy+3] = truPtr[3];

                    }
                }
                //UpperBorder
                for (int y = 0; y < filterHeightOffset; y++)
                {
                    for (int x = filterWidthOffset; x < copyAdd.PixelWidth; x++)
                    {
                        var offsetTru = 0 *
                                     sourceBitmap.BackBufferStride +
                                     x * 4;
                        var offsetCopy = y *
                                        copyAdd.BackBufferStride +
                                        x * 4;
                        addPtr[offsetCopy] = truPtr[offsetTru];
                        addPtr[offsetCopy+1] = truPtr[offsetTru+1];
                        addPtr[offsetCopy+2] = truPtr[offsetTru+2];
                        addPtr[offsetCopy+3] = truPtr[offsetTru+3];

                    }
                }
                //RightUpperCorner
                var offsetRightUpperCorner = 0 *
                                sourceBitmap.BackBufferStride +
                                sourceBitmap.PixelWidth * 4;
                for (int y = 0; y < filterHeightOffset; y++)
                {
                    for (int x = copyAdd.PixelWidth- filterWidthOffset; x < copyAdd.PixelWidth; x++)
                    {

                        var offsetCopy = y *
                                         copyAdd.BackBufferStride +
                                         x * 4;

                        addPtr[offsetCopy] = truPtr[offsetRightUpperCorner];
                        addPtr[offsetCopy + 1] = truPtr[offsetRightUpperCorner+1];
                        addPtr[offsetCopy + 2] = truPtr[offsetRightUpperCorner+2];
                        addPtr[offsetCopy + 3] = truPtr[offsetRightUpperCorner+3];

                    }
                }
                //LeftLowerCorner
                var offsetLeftLowerCorner = sourceBitmap.PixelHeight *
                                sourceBitmap.BackBufferStride +
                                0 * 4;
                for (int y = copyAdd.PixelHeight- filterHeightOffset; y < copyAdd.PixelHeight; y++)
                {
                    for (int x = 0; x < filterWidthOffset; x++)
                    {
                        var offsetCopy = y *
                                         copyAdd.BackBufferStride +
                                         x * 4;

                        addPtr[offsetCopy] = truPtr[offsetLeftLowerCorner];
                        addPtr[offsetCopy + 1] = truPtr[offsetLeftLowerCorner+1];
                        addPtr[offsetCopy + 2] = truPtr[offsetLeftLowerCorner+2];
                        addPtr[offsetCopy + 3] = truPtr[offsetLeftLowerCorner+3];

                    }
                }
                //LowerBorder
                for (int y = copyAdd.PixelHeight - filterHeightOffset; y < copyAdd.PixelHeight; y++)
                {
                    for (int x = filterWidthOffset; x < copyAdd.PixelWidth; x++)
                    {
                        var offsetTru = 0 *
                                        sourceBitmap.BackBufferStride +
                                        x * 4;
                        var offsetCopy = y *
                                         copyAdd.BackBufferStride +
                                         x * 4;
                        addPtr[offsetCopy] = truPtr[offsetTru];
                        addPtr[offsetCopy + 1] = truPtr[offsetTru + 1];
                        addPtr[offsetCopy + 2] = truPtr[offsetTru + 2];
                        addPtr[offsetCopy + 3] = truPtr[offsetTru + 3];

                    }
                }
                //RightLowerCorner
                var offsetRightLowerCorner = sourceBitmap.PixelHeight *
                                             sourceBitmap.BackBufferStride +
                                             sourceBitmap.PixelWidth * 4;
                for (int y = copyAdd.PixelHeight - filterHeightOffset; y < copyAdd.PixelHeight; y++)
                {
                    for (int x = copyAdd.PixelWidth - filterWidthOffset; x < copyAdd.PixelWidth; x++)
                    {

                        var offsetCopy = y *
                                         copyAdd.BackBufferStride +
                                         x * 4;

                        addPtr[offsetCopy] = truPtr[offsetRightLowerCorner];
                        addPtr[offsetCopy + 1] = truPtr[offsetRightLowerCorner + 1];
                        addPtr[offsetCopy + 2] = truPtr[offsetRightLowerCorner + 2];
                        addPtr[offsetCopy + 3] = truPtr[offsetRightLowerCorner + 3];
                    }
                }
                return copyAdd;
            }
        }


        public static WriteableBitmap ConvolutionFilter<T>(this WriteableBitmap sourceBitmap, T filter)
                                where T : ConvolutionFilterBase
        {
            unsafe
            {
                int filterWidth = filter.FilterMatrix.GetLength(1);
                int filterHeight = filter.FilterMatrix.GetLength(0);


                int filterWidthOffset = (filterWidth - 1) / 2;
                int filterHeightOffset = (filterHeight - 1) / 2;
                int calcOffset = 0;

                var copy = new WriteableBitmap(sourceBitmap);
                //var copy = CreateCopyWithBorder(sourceBitmap, filter);

                sourceBitmap.Lock();
                copy.Lock();
                var pixelBuffer = (byte*)sourceBitmap.BackBuffer;
                var resultBuffer = (byte*)copy.BackBuffer;

                var blue = 0.0;
                var green = 0.0;
                var red = 0.0;


                var byteOffset = 0;


                //UpperLeftCorner
                for (int offsetY = 0; offsetY < filterHeightOffset; offsetY++)
                {
                    for (var offsetX = 0; offsetX < filterWidthOffset; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;

                        for (var filterY = -filterHeightOffset;
                            filterY < filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset;
                                filterX++)
                            {
                                calcOffset = byteOffset +
                                             (Math.Abs(filterX)*4) +
                                             (Math.Abs(filterY)* sourceBitmap.BackBufferStride);
                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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

                //Upper line
                for (var offsetY = 0; offsetY < filterHeightOffset; offsetY++)
                {
                    for (var offsetX = filterWidthOffset; offsetX < sourceBitmap.Width - filterWidthOffset; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;

                        for (var filterY = -filterHeightOffset;
                            filterY < 0; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset;
                                filterX++)
                            {
                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (0 * sourceBitmap.BackBufferStride);
                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
                            }
                        }


                        for (var filterY = 0;
                            filterY <= filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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

                //UpperRightCorner
                for (int offsetY = 0; offsetY < filterHeightOffset; offsetY++)
                {
                    for (var offsetX = sourceBitmap.PixelWidth - filterWidthOffset; offsetX < sourceBitmap.PixelWidth; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;

                        for (var filterY = -filterHeightOffset;
                            filterY < filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset;
                                filterX++)
                            {
                                calcOffset = byteOffset +
                                             (Math.Abs(filterX) * 4) +
                                             (Math.Abs(filterY) * sourceBitmap.BackBufferStride);
                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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

                //Left line
                for (var offsetY = filterHeightOffset; offsetY < sourceBitmap.PixelHeight - filterHeightOffset; offsetY++)
                {
                    for (var offsetX = 0; offsetX < filterWidthOffset; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;


                        for (var filterY = -filterHeightOffset;
                            filterY <= filterHeightOffset; filterY++)
                        {
                            for (var filterX = 0;
                                filterX <= filterWidthOffset; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
                            }
                        }
                        for (var filterY = -filterHeightOffset;
                            filterY <= filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX < 0; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (filterY * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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

                //Main area
                for (var offsetY = filterHeightOffset; offsetY < sourceBitmap.PixelHeight - filterHeightOffset; offsetY++)
                {
                    for (var offsetX = filterWidthOffset; offsetX < sourceBitmap.PixelWidth - filterWidthOffset; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;


                        for (var filterY = -filterHeightOffset;
                            filterY <= filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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

                //LeftLowerCorner
                for (int offsetY = sourceBitmap.PixelHeight - filterHeightOffset; offsetY < sourceBitmap.PixelHeight; offsetY++)
                {
                    for (var offsetX = 0; offsetX < filterWidthOffset; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;

                        for (var filterY = -filterHeightOffset;
                            filterY < filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset;
                                filterX++)
                            {
                                calcOffset = byteOffset +
                                             (Math.Abs(filterX) * 4) +
                                             (Math.Abs(filterY) * sourceBitmap.BackBufferStride);
                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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

                //Right line
                for (var offsetY = filterHeightOffset; offsetY < sourceBitmap.PixelHeight - filterHeightOffset; offsetY++)
                {
                    for (var offsetX = sourceBitmap.PixelWidth - filterWidthOffset; offsetX < sourceBitmap.PixelWidth; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;


                        for (var filterY = -filterHeightOffset;
                            filterY <= filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= 0; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
                            }
                        }
                        for (var filterY = -filterHeightOffset;
                            filterY <= filterHeightOffset; filterY++)
                        {
                            for (var filterX = 1;
                                filterX <= filterWidthOffset; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (sourceBitmap.PixelWidth * 4) +
                                             (filterY * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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

                //Lower line
                for (var offsetY = sourceBitmap.PixelHeight - filterHeightOffset; offsetY < sourceBitmap.PixelHeight; offsetY++)
                {
                    for (var offsetX = filterWidthOffset; offsetX < sourceBitmap.PixelWidth - filterWidthOffset; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;


                        for (var filterY = -filterHeightOffset;
                            filterY <= 0; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
                            }
                        }
                        for (var filterY = 1;
                            filterY <= filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset; filterX++)
                            {


                                calcOffset = byteOffset +
                                             (sourceBitmap.PixelHeight * 4) +
                                             (filterX * sourceBitmap.BackBufferStride);


                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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

                //RightLowerCorner
                for (int offsetY = sourceBitmap.PixelHeight - filterHeightOffset; offsetY < sourceBitmap.PixelHeight; offsetY++)
                {
                    for (var offsetX = sourceBitmap.PixelWidth - filterWidthOffset; offsetX < sourceBitmap.PixelWidth; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;


                        byteOffset = offsetY *
                                     sourceBitmap.BackBufferStride +
                                     offsetX * 4;

                        for (var filterY = -filterHeightOffset;
                            filterY < filterHeightOffset; filterY++)
                        {
                            for (var filterX = -filterWidthOffset;
                                filterX <= filterWidthOffset;
                                filterX++)
                            {
                                calcOffset = byteOffset +
                                             (Math.Abs(filterX) * 4) +
                                             (Math.Abs(filterY) * sourceBitmap.BackBufferStride);
                                blue += (double)(pixelBuffer[calcOffset]) *
                                        filter.FilterMatrix[filterY + filterHeightOffset,
                                            filterX + filterWidthOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                         filter.FilterMatrix[filterY + filterHeightOffset,
                                             filterX + filterWidthOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                       filter.FilterMatrix[filterY + filterHeightOffset,
                                           filterX + filterWidthOffset];
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
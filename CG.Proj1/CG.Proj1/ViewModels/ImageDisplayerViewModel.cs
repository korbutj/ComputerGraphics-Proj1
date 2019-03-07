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
using CG.Proj1.BaseClass;
using CG.Proj1.ConvolutionFilters;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;

namespace CG.Proj1.ViewModels
{
    public partial class ImageDisplayerViewModel : BindableBase
    {
        private Uri imgPathUri;
        public Uri ImgPathUri
        {
            get { return imgPathUri; }
            set
            {
                SetProperty(ref imgPathUri, value);
                Image = new BitmapImage(ImgPathUri);
                var clone = new BitmapImage(ImgPathUri);
                ConvertedImageSource = new WriteableBitmap(clone);
            }
        }


        private BitmapImage image;
        public BitmapImage Image
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

        public ICommand InverseCommand { get; set; }
        public ICommand BrightnessMinusCommand { get; set; }
        public ICommand BrightnessPlusCommand { get; set; }
        public ICommand ContrastCommand { get; set; }
        public ICommand BlurCommand { get; set; }
        public ICommand GaussSmoothingCommand { get; set; }
        public ICommand SharpenCommand { get; set; }
        public ICommand EmbossCommand { get; set; }
        public ICommand EdgeYCommand { get; set; }
        public ICommand EdgeXCommand { get; set; }

        public ImageDisplayerViewModel()
        {
            InverseCommand = new DelegateCommand(InverseBytes, ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
            BrightnessMinusCommand = new DelegateCommand(() => BrightnessCorrection(-30), ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
            BrightnessPlusCommand = new DelegateCommand(() => BrightnessCorrection(+30), ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
            ContrastCommand = new DelegateCommand(() => ContrastEnhancment(1.3), ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
            BlurCommand = new DelegateCommand(() => ConvolutionFilter(new PredefinedBlurConvolution()), ImageValid)
                .ObservesProperty(() => Image);
            GaussSmoothingCommand = new DelegateCommand(() => ConvolutionFilter(new PredefinedGaussianSmoothing()), ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
            SharpenCommand = new DelegateCommand(() => ConvolutionFilter(new PredefinedSharpen()), ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
            EmbossCommand = new DelegateCommand(() => ConvolutionFilter(new PredefinedEmboss()), ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
            EdgeXCommand = new DelegateCommand(() => ConvolutionFilter(new PredefinedEdgeDetectionX()), ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
            EdgeYCommand = new DelegateCommand(() => ConvolutionFilter(new PredefinedEdgeDetectionY()), ImageValid)
                .ObservesProperty(() => ConvertedImageSource);
        }


        private bool ImageValid()
        {
            return ConvertedImageSource != null;
        }

        public void ConvolutionFilter(ConvolutionFilterBase filter)
        {
            var copy = new WriteableBitmap(Image);
            ConvertedImageSource = copy.ConvolutionFilter(filter);
        }
    }
}
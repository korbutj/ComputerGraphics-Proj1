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
        public BitmapImage Image { get; set; }

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

        public ImageDisplayerViewModel(Uri imgPath)
        {
            Image = new BitmapImage(imgPath);
            var clone = new BitmapImage(imgPath);
            ConvertedImageSource = new WriteableBitmap(clone);
            InverseCommand = new DelegateCommand(InverseBytes, ImageValid)
                .ObservesProperty(() => Image);
            BrightnessMinusCommand = new DelegateCommand(() => BrightnessCorrection(-30), ImageValid)
                .ObservesProperty(() => Image);
            BrightnessPlusCommand = new DelegateCommand(() => BrightnessCorrection(+30), ImageValid)
                .ObservesProperty(() => Image);
            ContrastCommand = new DelegateCommand(() => ContrastEnhancment(1.3), ImageValid)
                .ObservesProperty(() => Image);
            BlurCommand = new DelegateCommand(() =>
                {
                    var copy = new WriteableBitmap(Image);
                    ConvertedImageSource = copy.ConvolutionFilter(new PredefinedBlurConvolution());
                }, ImageValid)
                .ObservesProperty(() => Image);
            GaussSmoothingCommand = new DelegateCommand(() =>
                {
                    var copy = new WriteableBitmap(Image);
                    ConvertedImageSource = copy.ConvolutionFilter(new PredefinedGaussianSmoothing());
                }, ImageValid)
                .ObservesProperty(() => Image);
            SharpenCommand = new DelegateCommand(() =>
                {
                    var copy = new WriteableBitmap(Image);
                    ConvertedImageSource = copy.ConvolutionFilter(new PredefinedSharpen());
                }, ImageValid)
                .ObservesProperty(() => Image);
            EmbossCommand = new DelegateCommand(() =>
                {
                    var copy = new WriteableBitmap(Image);
                    ConvertedImageSource = copy.ConvolutionFilter(new PredefinedEmboss());
                }, ImageValid)
                .ObservesProperty(() => Image);
        }
        

        private bool ImageValid()
        {
            return Image != null;
        }
    }
}
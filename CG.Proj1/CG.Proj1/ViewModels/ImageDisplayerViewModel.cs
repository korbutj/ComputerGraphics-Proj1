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
    public partial class ImageDisplayerViewModel : BindableBase
    {
        public BitmapImage Image { get; set; }

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
            InverseCommand = new DelegateCommand(InverseBytes, ImageValid)
                .ObservesProperty(() => Image);
            BrightnessMinusCommand = new DelegateCommand(() => BrightnessCorrection(-30), ImageValid)
                .ObservesProperty(() => Image);
            BrightnessPlusCommand = new DelegateCommand(() => BrightnessCorrection(+30), ImageValid)
                .ObservesProperty(() => Image);
            ContrastCommand = new DelegateCommand(() => ContrastEnhancment(1.3), ImageValid)
                .ObservesProperty(() => Image);
        }
        

        private bool ImageValid()
        {
            return Image != null;
        }
    }
}
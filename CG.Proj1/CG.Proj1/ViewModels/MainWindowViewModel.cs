using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CG.Proj1.BaseClass;
using CG.Proj1.ConvolutionFilters;
using CG.Proj1.Views;
using Prism.Commands;
using Prism.Mvvm;

namespace CG.Proj1.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ICommand OpenFileCommand { get; set; }


        private ImageDisplayerViewModel imgDisplayer;

        public ImageDisplayerViewModel ImgDisplayer
        {
            get { return imgDisplayer; }
            set { SetProperty(ref imgDisplayer, value); }
        }

        public ICommand ConvolutionEditorCommand { get; set; }

        public MainWindowViewModel()
        {
            OpenFileCommand = new DelegateCommand(OpenFileDialog);
            ImgDisplayer = new ImageDisplayerViewModel();
            ConvolutionEditorCommand = new DelegateCommand(ConvolutionEditor, () => ImgDisplayer?.ConvertedImageSource != null)
                .ObservesProperty(() => ImgDisplayer.ConvertedImageSource);
        }

        private void ConvolutionEditor()
        {
            var dialog = new PickSizes();
            if (dialog.ShowDialog() == true)
            {
                ImgDisplayer.ConvolutionFilter(dialog.Convolution);   
            }
        }

        private void OpenFileDialog()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "Images |*.jpg ; *.png"
            };
            if (dialog.ShowDialog() == true)
            {
                ImgDisplayer.ImgPathUri = new Uri(dialog.FileName);
            }

            ;
        }
    }
}
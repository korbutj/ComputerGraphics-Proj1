using System;
using System.Windows;
using System.Windows.Input;
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
            get => imgDisplayer;
            set => SetProperty(ref imgDisplayer, value);
        }

        public ICommand ConvolutionEditorCommand { get; set; }

        public MainWindowViewModel()
        {
            OpenFileCommand = new DelegateCommand(OpenFileDialog);
            ConvolutionEditorCommand = new DelegateCommand(ConvolutionEditor);
        }

        private void ConvolutionEditor()
        {
            var dialog = new PickSizes();
            if (dialog.ShowDialog() == true)
            {
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
                ImgDisplayer = new ImageDisplayerViewModel(new Uri(dialog.FileName));
            }
;
        }
    }
}
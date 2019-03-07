using System;
using System.Windows;
using System.Windows.Input;
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

        public MainWindowViewModel()
        {
            OpenFileCommand = new DelegateCommand(OpenFileDialog);
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
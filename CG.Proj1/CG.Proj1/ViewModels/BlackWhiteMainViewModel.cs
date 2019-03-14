using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace CG.Proj1.ViewModels
{
    public class BlackWhiteMainViewModel : BindableBase
    {
        private BlackWhiteDisplayer imgDisplayer;
        public BlackWhiteDisplayer ImgDisplayer
        {
            get { return imgDisplayer; }
            set { SetProperty(ref imgDisplayer, value); }
        }

        public ICommand OpenFileCommand { get; set; }

        public BlackWhiteMainViewModel()
        {
            OpenFileCommand = new DelegateCommand(OpenFileDialog);
            ImgDisplayer = new BlackWhiteDisplayer();
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
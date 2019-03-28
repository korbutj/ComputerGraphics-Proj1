using System;
using System.Collections.Generic;
using System.Windows.Input;
using CG.Proj1.BaseClass;
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

        public List<int> GrayScales => new List<int>() { 2, 4, 16 };
        public List<int> OctreeLevels => new List<int>() { 1,2,3,4,5,6 };

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
            };
            if (dialog.ShowDialog() == true)
            {
                ImgDisplayer.ImgPathUri = new Uri(dialog.FileName);
            }

            ;
        }
    }
}
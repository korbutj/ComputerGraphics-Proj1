using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CG.Proj1.ViewModels;

namespace CG.Proj1
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => (MainWindowViewModel) this.DataContext;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new BlackWhiteMainViewModel();
        }
    }
}

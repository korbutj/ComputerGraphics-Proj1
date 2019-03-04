using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CG.Proj1.Views
{
    public partial class PickSizes : Window
    {
        public PickSizes()
        {
            InitializeComponent();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ToMatrix_OnClick(object sender, RoutedEventArgs e)
        {
            var regex = new Regex("[^1]*[^3]*[^5]*[^7]*[^9]*");
            var rowsMatch = regex.Match(RowsBox.Text);
            var columnsMatch = regex.Match(ColumnsBox.Text);
        }
    }
}

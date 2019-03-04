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
using System.Windows.Shapes;

namespace CG.Proj1.Views
{


    public partial class ConvolutionMatrixEditor : Window
    {
        public ConvolutionMatrixEditor(int rows, int columns)
        {
            InitializeComponent();
            for (int i = 0; i < columns; i++)
            {
                GridLayout.ColumnDefinitions.Add(new ColumnDefinition(){Width = GridLength.Auto});
            }
            for (int j = 0; j < rows; j++)
            {
                GridLayout.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    var textBox = new TextBox();
                    Grid.SetColumn(textBox,i);
                    Grid.SetRow(textBox,j);
                }
            }
        }
    }
}

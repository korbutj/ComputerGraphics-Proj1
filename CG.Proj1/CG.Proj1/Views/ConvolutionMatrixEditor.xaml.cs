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
using CG.Proj1.ConvolutionFilters;
using CG.Proj1.ViewModels;

namespace CG.Proj1.Views
{


    public partial class ConvolutionMatrixEditor : Window
    {
        public CustomConvolution Convolution { get; set; }
        private TextBox[,] textBoxes;
        private int rows;
        private int columns;

        public ConvolutionMatrixEditor(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            InitializeComponent();
            textBoxes = new TextBox[columns,rows];
            for (int i = 0; i < columns; i++)
            {
                GridLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Pixel) });
            }
            for (int j = 0; j < rows; j++)
            {
                GridLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Pixel) });
            }

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    var textBox = new TextBox()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(3,3,3,3)
                    };
                    Grid.SetColumn(textBox, i);
                    Grid.SetRow(textBox, j);
                    GridLayout.Children.Add(textBox);
                    textBoxes[i, j] = textBox;
                }
            }

            FactorBox.Text = "1";
            BiasBox.Text = "0";
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Convolution = new CustomConvolution()
            {
                Factor = double.Parse(FactorBox.Text),
                Bias = double.Parse(BiasBox.Text),
                FilterMatrix = new double[columns,rows]
            };
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Convolution.FilterMatrix[i, j] = string.IsNullOrEmpty(textBoxes[i, j].Text) ? 0 : double.Parse(textBoxes[i, j].Text);
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Predefined_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var box in textBoxes)
            {
                box.Text = "1";
            }
        }
    }
}

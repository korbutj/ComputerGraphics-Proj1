using CG.Proj1.BaseClass;
using Prism.Mvvm;

namespace CG.Proj1.ConvolutionFilters
{
    public class CustomConvolution : ConvolutionFilterBase
    {
        public override string FilterName { get; }
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix
        {
            get;
            set;
        }
    }
}
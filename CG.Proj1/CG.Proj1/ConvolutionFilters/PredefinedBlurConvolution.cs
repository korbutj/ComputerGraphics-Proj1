using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedBlurConvolution : ConvolutionFilterBase
    {
        public override string FilterName => "Blur";

        public override double Factor
        {
            get { return 0.3; }
            set { factor = value; }
        }

        public override double Bias
        {
            get { return 0.0; }
            set { bias = value; }
        }

        public override double[,] FilterMatrix
        {
            get
            {
                return new double[,] {{1, 1, 1}, {1, 1, 1}, {1, 1, 1}};
            }
            set { filterMatrix = value; }
        }
    }
}
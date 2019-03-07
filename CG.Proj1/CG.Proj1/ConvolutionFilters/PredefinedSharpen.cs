using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedSharpen : ConvolutionFilterBase
    {
        public override string FilterName => "PredefinedSharpen";
        public override double Factor
        {
            get { return 1.0; }
            set { factor = value; }
        }

        public override double Bias
        {
            get { return 0.0; }
            set { bias = value; }
        }

        public override double[,] FilterMatrix
        {
            get { return new double[,] {{-1, -1, -1}, {-1, 9, -1}, {-1, -1, -1}}; }
            set { filterMatrix = value; }
        }
    }
}
using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedGaussianSmoothing : ConvolutionFilterBase
    {
        public override string FilterName => "Gaussian Smoothing";
        public override double Factor
        {
            get => 1.0;
            set => factor = value;
        }

        public override double Bias
        {
            get => 0.0;
            set => bias = value;
        }

        public override double[,] FilterMatrix
        {
            get => new double[,] {{0, 1, 0}, {1, 4, 1}, {0, 1, 0}};
            set => filterMatrix = value;
        }
    }
}
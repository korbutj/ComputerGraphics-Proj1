using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedGaussianSmoothing : ConvolutionFilterBase
    {
        public override string FilterName => "Gaussian Smoothing";
        public override double Factor
        {
            get { return 0.3; }
            set {
                    factor = value;
                }
            }

        public override double Bias
        {
            get { return 0.0; }
            set {
                    bias = value;
                }
            }

        public override double[,] FilterMatrix
        {
            get { return new double[,] {{0, 1, 0}, {1, 4, 1}, {0, 1, 0}}; }
            set {
                    filterMatrix = value;
                }
            }
    }
}
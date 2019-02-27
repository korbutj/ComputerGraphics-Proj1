using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedGaussianSmoothing : ConvolutionFilterBase
    {
        public override string FilterName => "Gaussian Smoothing";
        public override double Factor => 1.0;
        public override double Bias => 0.0;
        public override double[,] FilterMatrix => new double[,] { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
    }
}
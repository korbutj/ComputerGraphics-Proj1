using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedEdgeDetectionX : ConvolutionFilterBase
    {
        public override string FilterName => "EdgeDetectionX";
        public override double Factor => 1;
        public override double Bias => 0;
        public override double[,] FilterMatrix => new double[,] { { 0, -1, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };

    }
}
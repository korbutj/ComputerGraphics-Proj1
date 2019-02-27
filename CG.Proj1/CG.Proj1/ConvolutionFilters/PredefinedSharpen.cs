using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedSharpen : ConvolutionFilterBase
    {
        public override string FilterName => "PredefinedSharpen";
        public override double Factor => 1.0;
        public override double Bias => 0.0;
        public override double[,] FilterMatrix => new double[,] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
    }
}
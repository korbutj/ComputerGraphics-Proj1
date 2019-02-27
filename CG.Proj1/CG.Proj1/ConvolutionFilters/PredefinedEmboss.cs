using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedEmboss : ConvolutionFilterBase
    {
        public override string FilterName => "Emboss";
        public override double Factor => 1;
        public override double Bias => 0;
        public override double[,] FilterMatrix => new double[,] { { -1, -1, -1 }, { 0, 1, 0 }, { 1, 1, 1 } };
    }
}
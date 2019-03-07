using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedEdgeDetectionY : ConvolutionFilterBase
    {
        public override string FilterName => "EdgeDetectionY";
        public override double Factor
        {
            get => 1;
            set => factor = value;
        }

        public override double Bias
        {
            get => 0;
            set => bias = value;
        }

        public override double[,] FilterMatrix
        {
            get => new double[,] {{0, 0, 0}, {-1, 1, 0}, {0, 0, 0}};
            set => filterMatrix = value;
        }
    }
}
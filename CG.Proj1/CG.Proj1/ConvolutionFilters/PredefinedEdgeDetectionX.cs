using CG.Proj1.BaseClass;

namespace CG.Proj1.ConvolutionFilters
{
    public class PredefinedEdgeDetectionX : ConvolutionFilterBase
    {
        public override string FilterName => "EdgeDetectionX";
        public override double Factor
        {
            get { return 1; }
            set {
                    factor = value;
                }
            }

        public override double Bias
        {
            get { return 0; }
            set {
                    bias = value;
                }
            }

        public override double[,] FilterMatrix
        {
            get { return new double[,] {{0, -1, 0}, {0, 1, 0}, {0, 0, 0}}; }
            set {
                    filterMatrix = value;
                }
            }
    }
}
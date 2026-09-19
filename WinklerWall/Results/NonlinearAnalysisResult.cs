using System.Collections.Generic;
using WinklerWall.Elements;

namespace WinklerWall.Results
{
    public class NonlinearAnalysisResult
    {
        public double[] Displacements
        {
            get;
            private set;
        }


        public List<TwoSidedElastoPlasticSpring> Springs
        {
            get;
            private set;
        }


        public int Iterations
        {
            get;
            private set;
        }


        public bool Converged
        {
            get;
            private set;
        }


        public double MaximumResidual
        {
            get;
            private set;
        }


        public NonlinearAnalysisResult(
            double[] displacements,
            List<TwoSidedElastoPlasticSpring> springs,
            int iterations,
            bool converged,
            double maximumResidual)
        {
            Displacements =
                displacements;

            Springs =
                springs;

            Iterations =
                iterations;

            Converged =
                converged;

            MaximumResidual =
                maximumResidual;
        }
    }
}
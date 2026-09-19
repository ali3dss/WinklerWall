namespace WinklerWall.Results
{
    /// <summary>
    /// Resultados de esfuerzos en los extremos
    /// de un elemento de viga.
    /// </summary>
    public class BeamElementResult
    {
        public int ElementId { get; private set; }

        public double Zi { get; private set; }

        public double Zj { get; private set; }

        public double ShearI { get; private set; }

        public double MomentI { get; private set; }

        public double ShearJ { get; private set; }

        public double MomentJ { get; private set; }

        public BeamElementResult(
            int elementId,
            double zi,
            double zj,
            double shearI,
            double momentI,
            double shearJ,
            double momentJ)
        {
            ElementId = elementId;

            Zi = zi;
            Zj = zj;

            ShearI = shearI;
            MomentI = momentI;

            ShearJ = shearJ;
            MomentJ = momentJ;
        }
    }
}
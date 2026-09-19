using WinklerWall.Models;

namespace WinklerWall.Elements
{
    /// <summary>
    /// Representa un resorte horizontal de Winkler
    /// asociado a un nodo de la pantalla.
    /// </summary>
    public class WinklerSpring
    {
        /// <summary>
        /// Nodo al que está asociado el resorte.
        /// </summary>
        public Node Node { get; private set; }

        /// <summary>
        /// Coeficiente de balasto horizontal KH [kN/m3].
        /// </summary>
        public double SubgradeModulus { get; private set; }

        /// <summary>
        /// Longitud tributaria del nodo [m].
        /// </summary>
        public double TributaryLength { get; private set; }

        /// <summary>
        /// Ancho de cálculo de la pantalla [m].
        /// Normalmente 1.0 m.
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// Rigidez nodal equivalente del resorte [kN/m].
        ///
        /// k = KH * b * Ltributaria
        /// </summary>
        public double Stiffness
        {
            get
            {
                return SubgradeModulus
                       * Width
                       * TributaryLength;
            }
        }

        public WinklerSpring(
            Node node,
            double subgradeModulus,
            double tributaryLength,
            double width = 1.0)
        {
            Node = node;
            SubgradeModulus = subgradeModulus;
            TributaryLength = tributaryLength;
            Width = width;
        }
    }
}
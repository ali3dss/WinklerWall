using System;
using WinklerWall.Models;

namespace WinklerWall.Elements
{
    /// <summary>
    /// Elemento de viga Euler-Bernoulli de dos nodos.
    ///
    /// Cada nodo posee dos grados de libertad:
    /// - desplazamiento horizontal u
    /// - rotación theta
    ///
    /// Orden de los grados de libertad locales:
    /// { u_i, theta_i, u_j, theta_j }
    /// </summary>
    public class BeamElement
    {
        public int Id { get; private set; }

        public Node NodeI { get; private set; }

        public Node NodeJ { get; private set; }

        /// <summary>
        /// Rigidez a flexión EI [kN·m²].
        /// </summary>
        public double EI { get; private set; }

        /// <summary>
        /// Longitud del elemento [m].
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Abs(NodeJ.Z - NodeI.Z);
            }
        }

        public BeamElement(
            int id,
            Node nodeI,
            Node nodeJ,
            double ei)
        {
            if (nodeI == null)
                throw new ArgumentNullException(nameof(nodeI));

            if (nodeJ == null)
                throw new ArgumentNullException(nameof(nodeJ));

            if (ei <= 0.0)
                throw new ArgumentException(
                    "La rigidez EI debe ser mayor que cero.");

            if (Math.Abs(nodeJ.Z - nodeI.Z) <= 0.0)
                throw new ArgumentException(
                    "La longitud del elemento debe ser mayor que cero.");

            Id = id;
            NodeI = nodeI;
            NodeJ = nodeJ;
            EI = ei;
        }

        /// <summary>
        /// Devuelve los grados de libertad globales
        /// asociados al elemento.
        /// </summary>
        public int[] GetGlobalDofs()
        {
            return new int[]
            {
                NodeI.HorizontalDof,
                NodeI.RotationDof,
                NodeJ.HorizontalDof,
                NodeJ.RotationDof
            };
        }

        /// <summary>
        /// Calcula la matriz de rigidez del elemento
        /// de viga Euler-Bernoulli.
        /// </summary>
        public double[,] GetStiffnessMatrix()
        {
            double L = Length;

            double factor = EI / Math.Pow(L, 3);

            double[,] k = new double[4, 4];

            k[0, 0] = 12.0;
            k[0, 1] = 6.0 * L;
            k[0, 2] = -12.0;
            k[0, 3] = 6.0 * L;

            k[1, 0] = 6.0 * L;
            k[1, 1] = 4.0 * L * L;
            k[1, 2] = -6.0 * L;
            k[1, 3] = 2.0 * L * L;

            k[2, 0] = -12.0;
            k[2, 1] = -6.0 * L;
            k[2, 2] = 12.0;
            k[2, 3] = -6.0 * L;

            k[3, 0] = 6.0 * L;
            k[3, 1] = 2.0 * L * L;
            k[3, 2] = -6.0 * L;
            k[3, 3] = 4.0 * L * L;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    k[i, j] *= factor;
                }
            }

            return k;
        }
    }
}
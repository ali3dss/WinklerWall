using System;
using System.Collections.Generic;
using WinklerWall.Models;
using WinklerWall.Elements;

namespace WinklerWall.Solver
{
    public static class LoadVectorAssembler
    {
        /// <summary>
        /// Calcula el vector de cargas nodales equivalentes
        /// de un elemento sometido a empuje activo lineal:
        ///
        /// p(z) = Ka * gamma * z
        ///
        /// Devuelve:
        /// { Fi, Mi, Fj, Mj }
        /// </summary>
        public static double[] GetActiveEarthPressureLocalVector(
            BeamElement element,
            double excavationDepth,
            double activeEarthPressureCoefficient,
            double unitWeight,
            double width = 1.0)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            double zi = element.NodeI.Z;
            double zj = element.NodeJ.Z;

            double[] localF = new double[4];

            // En esta primera versión únicamente se cargan
            // elementos completamente situados sobre
            // el fondo de excavación.
            if (zj > excavationDepth)
                return localF;

            double L = element.Length;

            // Presiones activas [kN/m²]
            double pi =
                activeEarthPressureCoefficient
                * unitWeight
                * zi;

            double pj =
                activeEarthPressureCoefficient
                * unitWeight
                * zj;

            // Cargas lineales [kN/m]
            double qi = pi * width;
            double qj = pj * width;

            // Vector de cargas nodales consistentes
            localF[0] =
                L / 20.0
                * (7.0 * qi + 3.0 * qj);

            localF[1] =
                L * L / 60.0
                * (3.0 * qi + 2.0 * qj);

            localF[2] =
                L / 20.0
                * (3.0 * qi + 7.0 * qj);

            localF[3] =
                -L * L / 60.0
                * (2.0 * qi + 3.0 * qj);

            return localF;
        }

        /// <summary>
        /// Ensambla el vector global de cargas.
        /// </summary>
        public static double[] AssembleActiveEarthPressure(
            List<Node> nodes,
            List<BeamElement> elements,
            double excavationDepth,
            double activeEarthPressureCoefficient,
            double unitWeight,
            double width = 1.0)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            int numberOfDofs =
                nodes.Count * 2;

            double[] globalF =
                new double[numberOfDofs];

            foreach (BeamElement element in elements)
            {
                double[] localF =
                    GetActiveEarthPressureLocalVector(
                        element,
                        excavationDepth,
                        activeEarthPressureCoefficient,
                        unitWeight,
                        width);

                int[] dofs =
                    element.GetGlobalDofs();

                for (int i = 0; i < 4; i++)
                {
                    globalF[dofs[i]] += localF[i];
                }
            }

            return globalF;
        }
    }
}
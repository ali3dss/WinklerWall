using System;
using System.Collections.Generic;
using WinklerWall.Models;
using WinklerWall.Elements;

namespace WinklerWall.Solver
{
    /// <summary>
    /// Ensambla la matriz global de rigidez del sistema:
    ///
    /// Kglobal = Kviga + Ksuelo
    /// </summary>
    public static class MatrixAssembler
    {
        /// <summary>
        /// Construye la matriz global de rigidez.
        /// </summary>
        /// <param name="nodes">Nodos de la pantalla.</param>
        /// <param name="elements">Elementos de viga.</param>
        /// <param name="springs">Resortes Winkler.</param>
        /// <returns>Matriz global K.</returns>
        public static double[,] AssembleGlobalStiffness(
            List<Node> nodes,
            List<BeamElement> elements,
            List<WinklerSpring> springs)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            if (springs == null)
                throw new ArgumentNullException(nameof(springs));

            if (nodes.Count == 0)
                throw new ArgumentException(
                    "La lista de nodos no puede estar vacía.");

            // Cada nodo tiene dos grados de libertad:
            //
            // u     = desplazamiento horizontal
            // theta = rotación
            //
            // Por tanto:
            //
            // NDOF = 2 * número de nodos

            int numberOfDofs = nodes.Count * 2;

            double[,] globalK =
                new double[numberOfDofs, numberOfDofs];

            // =====================================================
            // 1. ENSAMBLAJE DE LOS ELEMENTOS DE VIGA
            // =====================================================

            foreach (BeamElement element in elements)
            {
                double[,] localK =
                    element.GetStiffnessMatrix();

                int[] dofs =
                    element.GetGlobalDofs();

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int globalRow = dofs[i];
                        int globalColumn = dofs[j];

                        globalK[globalRow, globalColumn]
                            += localK[i, j];
                    }
                }
            }

            // =====================================================
            // 2. ENSAMBLAJE DE LOS RESORTES WINKLER
            // =====================================================

            foreach (WinklerSpring spring in springs)
            {
                int dof =
                    spring.Node.HorizontalDof;

                globalK[dof, dof]
                    += spring.Stiffness;
            }

            return globalK;
        }
    }
}
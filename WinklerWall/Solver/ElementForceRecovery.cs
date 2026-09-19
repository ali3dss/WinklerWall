using System;
using System.Collections.Generic;
using WinklerWall.Models;
using WinklerWall.Elements;
using WinklerWall.Results;

namespace WinklerWall.Solver
{
    /// <summary>
    /// Recupera los esfuerzos internos de los
    /// elementos una vez resuelto K * U = F.
    /// </summary>
    public static class ElementForceRecovery
    {
        public static List<BeamElementResult> Recover(
            AnalysisModel model,
            double[] globalU,
            double activeEarthPressureCoefficient,
            double unitWeight,
            double width = 1.0)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (globalU == null)
                throw new ArgumentNullException(nameof(globalU));

            List<BeamElementResult> results =
                new List<BeamElementResult>();

            foreach (BeamElement element in model.Elements)
            {
                int[] dofs =
                    element.GetGlobalDofs();

                // ==========================================
                // VECTOR LOCAL DE DESPLAZAMIENTOS
                // ==========================================

                double[] localU = new double[4];

                for (int i = 0; i < 4; i++)
                {
                    localU[i] =
                        globalU[dofs[i]];
                }

                // ==========================================
                // MATRIZ DEL ELEMENTO
                // ==========================================

                double[,] localK =
                    element.GetStiffnessMatrix();

                // ==========================================
                // CARGAS DISTRIBUIDAS EQUIVALENTES
                // ==========================================

                double[] localF =
                    LoadVectorAssembler
                    .GetActiveEarthPressureLocalVector(
                        element,
                        model.ExcavationDepth,
                        activeEarthPressureCoefficient,
                        unitWeight,
                        width);

                // ==========================================
                // R = K * U - F
                // ==========================================

                double[] endForces =
                    new double[4];

                for (int i = 0; i < 4; i++)
                {
                    double value = 0.0;

                    for (int j = 0; j < 4; j++)
                    {
                        value +=
                            localK[i, j]
                            * localU[j];
                    }

                    endForces[i] =
                        value - localF[i];
                }

                BeamElementResult result =
                    new BeamElementResult(
                        element.Id,
                        element.NodeI.Z,
                        element.NodeJ.Z,
                        endForces[0],
                        endForces[1],
                        endForces[2],
                        endForces[3]);

                results.Add(result);
            }

            return results;
        }
    }
}
using System;
using System.Collections.Generic;

using WinklerWall.Models;
using WinklerWall.Elements;
using WinklerWall.Results;

namespace WinklerWall.Solver
{
    public static class NonlinearSolver
    {
        public static NonlinearAnalysisResult Solve(
            AnalysisModel model,
            double[] globalLoadVector,
            double phiDegrees,
            double gamma,
            int numberOfLoadSteps = 20,
            int maximumIterationsPerStep = 50,
            double tolerance = 1e-6)
        {
            if (model == null)
            {
                throw new ArgumentNullException(
                    nameof(model));
            }


            if (globalLoadVector == null)
            {
                throw new ArgumentNullException(
                    nameof(globalLoadVector));
            }


            if (numberOfLoadSteps <= 0)
            {
                throw new ArgumentException(
                    "El número de incrementos debe ser mayor que cero.");
            }


            if (maximumIterationsPerStep <= 0)
            {
                throw new ArgumentException(
                    "El número máximo de iteraciones debe ser mayor que cero.");
            }


            if (tolerance <= 0.0)
            {
                throw new ArgumentException(
                    "La tolerancia debe ser mayor que cero.");
            }


            // ========================================================
            // 1. COEFICIENTES GEOTÉCNICOS
            // ========================================================

            double K0 =
                EarthPressureCalculator.CalculateK0(
                    phiDegrees);


            double Ka =
                EarthPressureCalculator.CalculateKa(
                    phiDegrees);


            double Kp =
                EarthPressureCalculator.CalculateKp(
                    phiDegrees);


            // ========================================================
            // 2. RESORTES DE DOS CARAS
            // ========================================================

            List<TwoSidedElastoPlasticSpring> springs =
                new List<TwoSidedElastoPlasticSpring>();


            foreach (WinklerSpring linearSpring
                     in model.Springs)
            {
                double z =
                    linearSpring.Node.Z;


                double pa =
                    EarthPressureCalculator.ActivePressure(
                        Ka,
                        gamma,
                        z);


                double p0 =
                    EarthPressureCalculator.InitialPressure(
                        K0,
                        gamma,
                        z);


                double pp =
                    EarthPressureCalculator.PassivePressure(
                        Kp,
                        gamma,
                        z);


                TwoSidedElastoPlasticSpring spring =
                    new TwoSidedElastoPlasticSpring(
                        linearSpring.Node,
                        model.SubgradeModulus,
                        linearSpring.TributaryLength,
                        model.Wall.Width,
                        p0,
                        pa,
                        pp);


                springs.Add(
                    spring);
            }


            // ========================================================
            // 3. MATRIZ DE VIGA SIN RESORTES
            // ========================================================

            double[,] beamK =
                MatrixAssembler.AssembleGlobalStiffness(
                    model.Nodes,
                    model.Elements,
                    new List<WinklerSpring>());


            // ========================================================
            // 4. DESPLAZAMIENTOS
            // ========================================================

            double[] U =
                new double[
                    globalLoadVector.Length];


            bool globalConvergence =
                true;


            int totalIterations =
                0;


            double finalMaximumResidual =
                double.PositiveInfinity;


            // ========================================================
            // 5. INCREMENTOS DE CARGA
            // ========================================================

            for (int loadStep = 1;
                 loadStep <= numberOfLoadSteps;
                 loadStep++)
            {
                double loadFactor =
                    (double)loadStep /
                    numberOfLoadSteps;


                bool stepConverged =
                    false;


                for (int iteration = 1;
                     iteration <= maximumIterationsPerStep;
                     iteration++)
                {
                    totalIterations++;


                    // ================================================
                    // VALIDAR U
                    // ================================================

                    ValidateVector(
                        U,
                        "vector U",
                        loadStep,
                        iteration);


                    // ================================================
                    // ACTUALIZAR RESORTES
                    // ================================================

                    foreach (TwoSidedElastoPlasticSpring spring
                             in springs)
                    {
                        int dof =
                            spring.Node.HorizontalDof;


                        spring.UpdateState(
                            U[dof]);
                    }


                    // ================================================
                    // RESIDUO
                    // ================================================

                    double[] residual =
                        CalculateResidual(
                            beamK,
                            U,
                            globalLoadVector,
                            loadFactor,
                            springs);


                    ValidateVector(
                        residual,
                        "vector residual",
                        loadStep,
                        iteration);


                    double maximumResidual =
                        GetMaximumAbsoluteValue(
                            residual);


                    finalMaximumResidual =
                        maximumResidual;


                    // ================================================
                    // CONVERGENCIA
                    // ================================================

                    if (maximumResidual <
                        tolerance)
                    {
                        stepConverged =
                            true;

                        break;
                    }


                    // ================================================
                    // MATRIZ TANGENTE
                    // ================================================

                    double[,] tangentK =
                        CopyMatrix(
                            beamK);


                    foreach (TwoSidedElastoPlasticSpring spring
                             in springs)
                    {
                        int dof =
                            spring.Node.HorizontalDof;


                        double tangent =
                            spring.GetTangentStiffness();


                        tangentK[dof, dof] +=
                            tangent;
                    }


                    ValidateMatrix(
                        tangentK,
                        loadStep,
                        iteration);


                    // ================================================
                    // -R
                    // ================================================

                    double[] negativeResidual =
                        new double[
                            residual.Length];


                    for (int i = 0;
                         i < residual.Length;
                         i++)
                    {
                        negativeResidual[i] =
                            -residual[i];
                    }


                    // ================================================
                    // RESOLVER:
                    //
                    // Kt DeltaU = -R
                    // ================================================

                    double[] deltaU;


                    try
                    {
                        deltaU =
                            LinearSolver.Solve(
                                tangentK,
                                negativeResidual);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            "FALLO DEL ANÁLISIS NO LINEAL\n\n" +

                            "Incremento de carga: " +
                            loadStep +
                            " de " +
                            numberOfLoadSteps +
                            "\n" +

                            "Factor de carga: " +
                            loadFactor.ToString("F3") +
                            "\n" +

                            "Carga aplicada: " +
                            (loadFactor * 100.0).ToString("F1") +
                            " %\n" +

                            "Iteración: " +
                            iteration +
                            "\n\n" +

                            "La matriz tangente es singular o está " +
                            "numéricamente mal condicionada.\n\n" +

                            "Detalle:\n" +
                            ex.Message,
                            ex);
                    }


                    ValidateVector(
                        deltaU,
                        "DeltaU",
                        loadStep,
                        iteration);


                    // ================================================
                    // ACTUALIZAR U
                    // ================================================

                    for (int i = 0;
                         i < U.Length;
                         i++)
                    {
                        U[i] +=
                            deltaU[i];
                    }


                    ValidateVector(
                        U,
                        "vector U actualizado",
                        loadStep,
                        iteration);
                }


                // ====================================================
                // ESCALÓN NO CONVERGENTE
                // ====================================================

                if (!stepConverged)
                {
                    globalConvergence =
                        false;

                    break;
                }
            }


            // ========================================================
            // 6. ACTUALIZACIÓN FINAL
            // ========================================================

            if (IsFiniteVector(U))
            {
                foreach (TwoSidedElastoPlasticSpring spring
                         in springs)
                {
                    int dof =
                        spring.Node.HorizontalDof;


                    spring.UpdateState(
                        U[dof]);
                }
            }
            else
            {
                globalConvergence =
                    false;
            }


            // ========================================================
            // 7. RESIDUO FINAL
            // ========================================================

            if (globalConvergence)
            {
                double[] finalResidual =
                    CalculateResidual(
                        beamK,
                        U,
                        globalLoadVector,
                        1.0,
                        springs);


                finalMaximumResidual =
                    GetMaximumAbsoluteValue(
                        finalResidual);


                if (finalMaximumResidual >
                    tolerance)
                {
                    globalConvergence =
                        false;
                }
            }


            // ========================================================
            // RESULTADO
            // ========================================================

            return new NonlinearAnalysisResult(
                U,
                springs,
                totalIterations,
                globalConvergence,
                finalMaximumResidual);
        }


        // ============================================================
        // RESIDUO
        //
        // R = Kbeam*U + Rs - lambda*F
        // ============================================================

        private static double[] CalculateResidual(
            double[,] beamK,
            double[] displacement,
            double[] loadVector,
            double loadFactor,
            List<TwoSidedElastoPlasticSpring> springs)
        {
            int n =
                displacement.Length;


            double[] residual =
                new double[n];


            // ========================================================
            // Kbeam * U - lambda F
            // ========================================================

            for (int i = 0;
                 i < n;
                 i++)
            {
                double value =
                    0.0;


                for (int j = 0;
                     j < n;
                     j++)
                {
                    value +=
                        beamK[i, j] *
                        displacement[j];
                }


                residual[i] =
                    value -
                    loadFactor *
                    loadVector[i];
            }


            // ========================================================
            // FUERZAS DE RESORTE
            // ========================================================

            foreach (TwoSidedElastoPlasticSpring spring
                     in springs)
            {
                int dof =
                    spring.Node.HorizontalDof;


                double reaction =
                    spring.GetInternalResistance();


                if (double.IsNaN(reaction) ||
                    double.IsInfinity(reaction))
                {
                    throw new InvalidOperationException(
                        "La reacción del resorte del nodo " +
                        spring.Node.Id +
                        " no es válida.");
                }


                residual[dof] +=
                    reaction;
            }


            return residual;
        }


        // ============================================================
        // MÁXIMO ABSOLUTO
        // ============================================================

        private static double GetMaximumAbsoluteValue(
            double[] vector)
        {
            double maximum =
                0.0;


            for (int i = 0;
                 i < vector.Length;
                 i++)
            {
                double value =
                    vector[i];


                if (double.IsNaN(value) ||
                    double.IsInfinity(value))
                {
                    return
                        double.PositiveInfinity;
                }


                double absolute =
                    Math.Abs(
                        value);


                if (absolute >
                    maximum)
                {
                    maximum =
                        absolute;
                }
            }


            return maximum;
        }


        // ============================================================
        // VALIDAR VECTOR
        // ============================================================

        private static void ValidateVector(
            double[] vector,
            string name,
            int loadStep,
            int iteration)
        {
            for (int i = 0;
                 i < vector.Length;
                 i++)
            {
                if (double.IsNaN(vector[i]) ||
                    double.IsInfinity(vector[i]))
                {
                    throw new InvalidOperationException(
                        "Valor numérico inválido en " +
                        name +
                        ", posición " +
                        i +
                        ".\n\n" +

                        "Incremento: " +
                        loadStep +
                        "\n" +

                        "Iteración: " +
                        iteration +
                        ".");
                }
            }
        }


        // ============================================================
        // VALIDAR MATRIZ
        // ============================================================

        private static void ValidateMatrix(
            double[,] matrix,
            int loadStep,
            int iteration)
        {
            int rows =
                matrix.GetLength(0);

            int columns =
                matrix.GetLength(1);


            for (int i = 0;
                 i < rows;
                 i++)
            {
                for (int j = 0;
                     j < columns;
                     j++)
                {
                    double value =
                        matrix[i, j];


                    if (double.IsNaN(value) ||
                        double.IsInfinity(value))
                    {
                        throw new InvalidOperationException(
                            "La matriz tangente contiene un valor " +
                            "inválido en [" +
                            i +
                            "," +
                            j +
                            "].\n\n" +

                            "Incremento: " +
                            loadStep +
                            "\n" +

                            "Iteración: " +
                            iteration +
                            ".");
                    }
                }
            }
        }


        // ============================================================
        // VECTOR FINITO
        // ============================================================

        private static bool IsFiniteVector(
            double[] vector)
        {
            for (int i = 0;
                 i < vector.Length;
                 i++)
            {
                if (double.IsNaN(vector[i]) ||
                    double.IsInfinity(vector[i]))
                {
                    return false;
                }
            }


            return true;
        }


        // ============================================================
        // COPIAR MATRIZ
        // ============================================================

        private static double[,] CopyMatrix(
            double[,] source)
        {
            int rows =
                source.GetLength(0);

            int columns =
                source.GetLength(1);


            double[,] result =
                new double[
                    rows,
                    columns];


            for (int i = 0;
                 i < rows;
                 i++)
            {
                for (int j = 0;
                     j < columns;
                     j++)
                {
                    result[i, j] =
                        source[i, j];
                }
            }


            return result;
        }
    }
}
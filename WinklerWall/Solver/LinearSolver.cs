using System;
using MathNet.Numerics.LinearAlgebra;

namespace WinklerWall.Solver
{
    public static class LinearSolver
    {
        // ============================================================
        // RESOLVER K * U = F
        // ============================================================

        public static double[] Solve(
            double[,] stiffnessMatrix,
            double[] loadVector)
        {
            if (stiffnessMatrix == null)
            {
                throw new ArgumentNullException(
                    nameof(stiffnessMatrix));
            }

            if (loadVector == null)
            {
                throw new ArgumentNullException(
                    nameof(loadVector));
            }


            int rows =
                stiffnessMatrix.GetLength(0);

            int columns =
                stiffnessMatrix.GetLength(1);


            if (rows != columns)
            {
                throw new ArgumentException(
                    "La matriz de rigidez debe ser cuadrada.");
            }


            if (loadVector.Length != rows)
            {
                throw new ArgumentException(
                    "El tamaño del vector de cargas no coincide con la matriz de rigidez.");
            }


            // ========================================================
            // VALIDAR ENTRADAS
            // ========================================================

            ValidateMatrix(
                stiffnessMatrix);

            ValidateVector(
                loadVector,
                "vector de cargas");


            // ========================================================
            // CONVERTIR A MATHNET
            // ========================================================

            Matrix<double> K =
                Matrix<double>.Build.DenseOfArray(
                    stiffnessMatrix);


            Vector<double> F =
                Vector<double>.Build.DenseOfArray(
                    loadVector);


            try
            {
                // ====================================================
                // FACTORIZACIÓN LU
                // ====================================================

                var lu =
                    K.LU();


                Vector<double> U =
                    lu.Solve(F);


                double[] result =
                    U.ToArray();


                // ====================================================
                // VALIDAR RESULTADO
                //
                // IMPORTANTE:
                // MathNet puede devolver NaN o Infinity si la matriz
                // está singular o muy mal condicionada.
                // ====================================================

                ValidateVector(
                    result,
                    "solución del sistema");


                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "No fue posible resolver K·U = F. " +
                    "La matriz puede ser singular o estar numéricamente mal condicionada.",
                    ex);
            }
        }


        // ============================================================
        // CALCULAR RESIDUO MÁXIMO
        //
        // R = K * U - F
        // ============================================================

        public static double GetMaximumResidual(
            double[,] stiffnessMatrix,
            double[] displacementVector,
            double[] loadVector)
        {
            if (stiffnessMatrix == null)
            {
                throw new ArgumentNullException(
                    nameof(stiffnessMatrix));
            }

            if (displacementVector == null)
            {
                throw new ArgumentNullException(
                    nameof(displacementVector));
            }

            if (loadVector == null)
            {
                throw new ArgumentNullException(
                    nameof(loadVector));
            }


            ValidateMatrix(
                stiffnessMatrix);

            ValidateVector(
                displacementVector,
                "vector de desplazamientos");

            ValidateVector(
                loadVector,
                "vector de cargas");


            Matrix<double> K =
                Matrix<double>.Build.DenseOfArray(
                    stiffnessMatrix);


            Vector<double> U =
                Vector<double>.Build.DenseOfArray(
                    displacementVector);


            Vector<double> F =
                Vector<double>.Build.DenseOfArray(
                    loadVector);


            Vector<double> residual =
                K * U - F;


            double maximum =
                0.0;


            for (int i = 0;
                 i < residual.Count;
                 i++)
            {
                double value =
                    residual[i];


                if (double.IsNaN(value) ||
                    double.IsInfinity(value))
                {
                    return
                        double.PositiveInfinity;
                }


                double absolute =
                    Math.Abs(value);


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
            string name)
        {
            if (vector == null)
            {
                throw new ArgumentNullException(
                    nameof(vector));
            }


            for (int i = 0;
                 i < vector.Length;
                 i++)
            {
                double value =
                    vector[i];


                if (double.IsNaN(value) ||
                    double.IsInfinity(value))
                {
                    throw new InvalidOperationException(
                        "Se encontró un valor NaN o infinito en " +
                        name +
                        ", posición " +
                        i +
                        ".");
                }
            }
        }


        // ============================================================
        // VALIDAR MATRIZ
        // ============================================================

        private static void ValidateMatrix(
            double[,] matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(
                    nameof(matrix));
            }


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
                            "La matriz contiene un valor NaN o infinito en [" +
                            i +
                            "," +
                            j +
                            "].");
                    }
                }
            }
        }
    }
}
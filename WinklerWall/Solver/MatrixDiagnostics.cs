using System;

namespace WinklerWall.Solver
{
    /// <summary>
    /// Herramientas de diagnóstico para matrices.
    /// Se utilizarán para validar el ensamblaje
    /// antes de resolver el sistema de ecuaciones.
    /// </summary>
    public static class MatrixDiagnostics
    {
        /// <summary>
        /// Comprueba si una matriz es cuadrada y simétrica
        /// dentro de una tolerancia determinada.
        /// </summary>
        public static bool IsSymmetric(
            double[,] matrix,
            double tolerance = 1e-8)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            if (rows != columns)
                return false;

            for (int i = 0; i < rows; i++)
            {
                for (int j = i + 1; j < columns; j++)
                {
                    double difference =
                        Math.Abs(matrix[i, j] - matrix[j, i]);

                    if (difference > tolerance)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Devuelve la mayor diferencia absoluta encontrada
        /// entre Kij y Kji.
        /// </summary>
        public static double GetMaximumAsymmetry(
            double[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            if (rows != columns)
                throw new ArgumentException(
                    "La matriz debe ser cuadrada.");

            double maximumDifference = 0.0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = i + 1; j < columns; j++)
                {
                    double difference =
                        Math.Abs(matrix[i, j] - matrix[j, i]);

                    if (difference > maximumDifference)
                    {
                        maximumDifference = difference;
                    }
                }
            }

            return maximumDifference;
        }
    }
}
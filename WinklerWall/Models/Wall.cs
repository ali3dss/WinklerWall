using System;

namespace WinklerWall.Models
{
    /// <summary>
    /// Representa la geometría y rigidez estructural
    /// de la pantalla.
    /// </summary>
    public class Wall
    {
        /// <summary>
        /// Longitud total de la pantalla [m].
        /// </summary>
        public double Length { get; private set; }

        /// <summary>
        /// Espesor de la pantalla [m].
        /// </summary>
        public double Thickness { get; private set; }

        /// <summary>
        /// Ancho de cálculo de la pantalla [m].
        /// Para cálculos bidimensionales normalmente = 1.0 m.
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// Módulo de elasticidad del material [kN/m²].
        /// </summary>
        public double ElasticModulus { get; private set; }

        /// <summary>
        /// Momento de inercia de la sección [m4].
        /// I = b * e^3 / 12
        /// </summary>
        public double Inertia
        {
            get
            {
                return Width * Math.Pow(Thickness, 3) / 12.0;
            }
        }

        /// <summary>
        /// Rigidez a flexión EI [kN·m²].
        /// </summary>
        public double FlexuralRigidity
        {
            get
            {
                return ElasticModulus * Inertia;
            }
        }

        public Wall(
            double length,
            double thickness,
            double elasticModulus,
            double width = 1.0)
        {
            Length = length;
            Thickness = thickness;
            ElasticModulus = elasticModulus;
            Width = width;
        }
    }
}
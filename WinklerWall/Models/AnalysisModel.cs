using System;
using System.Collections.Generic;
using System.Linq;

using WinklerWall.Elements;

namespace WinklerWall.Models
{
    public class AnalysisModel
    {
        // ============================================================
        // PROPIEDADES
        // ============================================================

        public Wall Wall { get; private set; }

        public double ExcavationDepth { get; private set; }

        /// <summary>
        /// Longitud objetivo de discretización.
        /// Los elementos reales pueden ser menores si es necesario
        /// insertar nodos especiales.
        /// </summary>
        public double ElementLength { get; private set; }

        public double SubgradeModulus { get; private set; }


        public List<Node> Nodes { get; private set; }

        public List<BeamElement> Elements { get; private set; }

        public List<WinklerSpring> Springs { get; private set; }


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public AnalysisModel(
            Wall wall,
            double excavationDepth,
            double elementLength,
            double subgradeModulus)
        {
            if (wall == null)
            {
                throw new ArgumentNullException(
                    nameof(wall));
            }

            if (excavationDepth <= 0.0)
            {
                throw new ArgumentException(
                    "La profundidad de excavación debe ser mayor que cero.");
            }

            if (excavationDepth >= wall.Length)
            {
                throw new ArgumentException(
                    "La profundidad de excavación debe ser menor que la longitud total de la pantalla.");
            }

            if (elementLength <= 0.0)
            {
                throw new ArgumentException(
                    "La longitud objetivo del elemento debe ser mayor que cero.");
            }

            if (subgradeModulus <= 0.0)
            {
                throw new ArgumentException(
                    "El módulo de balasto debe ser mayor que cero.");
            }


            Wall = wall;

            ExcavationDepth =
                excavationDepth;

            ElementLength =
                elementLength;

            SubgradeModulus =
                subgradeModulus;


            Nodes =
                new List<Node>();

            Elements =
                new List<BeamElement>();

            Springs =
                new List<WinklerSpring>();


            BuildModel();
        }


        // ============================================================
        // CONSTRUIR MODELO
        // ============================================================

        private void BuildModel()
        {
            Nodes.Clear();

            Elements.Clear();

            Springs.Clear();


            // ========================================================
            // 1. GENERAR PROFUNDIDADES DE LOS NODOS
            // ========================================================

            List<double> depths =
                GenerateNodeDepths();


            // ========================================================
            // 2. CREAR NODOS
            // ========================================================

            for (int i = 0;
                 i < depths.Count;
                 i++)
            {
                Node node =
                    new Node(
                        i,
                        depths[i]);

                Nodes.Add(
                    node);
            }


            // ========================================================
            // 3. CREAR ELEMENTOS DE VIGA
            // ========================================================

            for (int i = 0;
                 i < Nodes.Count - 1;
                 i++)
            {
                BeamElement element =
                    new BeamElement(
                        i,
                        Nodes[i],
                        Nodes[i + 1],
                        Wall.FlexuralRigidity);

                Elements.Add(
                    element);
            }


            // ========================================================
            // 4. CREAR RESORTES WINKLER
            // ========================================================

            BuildSprings();
        }


        // ============================================================
        // GENERAR PROFUNDIDADES DE NODOS
        // ============================================================

        private List<double> GenerateNodeDepths()
        {
            List<double> depths =
                new List<double>();


            // ========================================================
            // NODO SUPERIOR
            // ========================================================

            depths.Add(
                0.0);


            // ========================================================
            // MALLA REGULAR
            // ========================================================

            double z =
                ElementLength;


            while (z < Wall.Length)
            {
                depths.Add(
                    z);

                z +=
                    ElementLength;
            }


            // ========================================================
            // NODO EXACTO EN EL FONDO DE EXCAVACIÓN
            // ========================================================

            depths.Add(
                ExcavationDepth);


            // ========================================================
            // NODO EXACTO EN LA PUNTA
            // ========================================================

            depths.Add(
                Wall.Length);


            // ========================================================
            // ORDENAR
            // ========================================================

            depths =
                depths
                .OrderBy(value => value)
                .ToList();


            // ========================================================
            // ELIMINAR NODOS DUPLICADOS
            //
            // Es necesario porque, por ejemplo:
            //
            // H = 6.00 m
            // dz = 1.00 m
            //
            // ya produce naturalmente un nodo en z = 6.00 m.
            // ========================================================

            const double tolerance =
                1e-8;


            List<double> uniqueDepths =
                new List<double>();


            foreach (double depth in depths)
            {
                if (uniqueDepths.Count == 0)
                {
                    uniqueDepths.Add(
                        depth);

                    continue;
                }


                double last =
                    uniqueDepths[
                        uniqueDepths.Count - 1];


                if (Math.Abs(
                        depth -
                        last) >
                    tolerance)
                {
                    uniqueDepths.Add(
                        depth);
                }
            }


            return uniqueDepths;
        }


        // ============================================================
        // CREAR RESORTES
        // ============================================================

        private void BuildSprings()
        {
            const double tolerance =
                1e-8;


            // ========================================================
            // BUSCAR NODO DE EXCAVACIÓN
            // ========================================================

            int excavationNodeIndex =
                -1;


            for (int i = 0;
                 i < Nodes.Count;
                 i++)
            {
                if (Math.Abs(
                        Nodes[i].Z -
                        ExcavationDepth) <
                    tolerance)
                {
                    excavationNodeIndex =
                        i;

                    break;
                }
            }


            if (excavationNodeIndex < 0)
            {
                throw new InvalidOperationException(
                    "No se encontró el nodo correspondiente al fondo de excavación.");
            }


            // ========================================================
            // CREAR RESORTES DESDE EL FONDO HASTA LA PUNTA
            // ========================================================

            for (int i = excavationNodeIndex;
                 i < Nodes.Count;
                 i++)
            {
                Node node =
                    Nodes[i];


                double tributaryLength;


                // ====================================================
                // PRIMER NODO DE LA ZONA EMPOTRADA
                // ====================================================

                if (i == excavationNodeIndex)
                {
                    if (i == Nodes.Count - 1)
                    {
                        tributaryLength =
                            0.0;
                    }
                    else
                    {
                        double rightLength =
                            Nodes[i + 1].Z -
                            node.Z;

                        tributaryLength =
                            rightLength /
                            2.0;
                    }
                }


                // ====================================================
                // NODO DE LA PUNTA
                // ====================================================

                else if (i == Nodes.Count - 1)
                {
                    double leftLength =
                        node.Z -
                        Nodes[i - 1].Z;

                    tributaryLength =
                        leftLength /
                        2.0;
                }


                // ====================================================
                // NODO INTERIOR
                // ====================================================

                else
                {
                    double leftLength =
                        node.Z -
                        Nodes[i - 1].Z;

                    double rightLength =
                        Nodes[i + 1].Z -
                        node.Z;


                    tributaryLength =
                        leftLength /
                        2.0 +
                        rightLength /
                        2.0;
                }


                WinklerSpring spring =
                    new WinklerSpring(
                        node,
                        SubgradeModulus,
                        tributaryLength,
                        Wall.Width);


                Springs.Add(
                    spring);
            }
        }
    }
}
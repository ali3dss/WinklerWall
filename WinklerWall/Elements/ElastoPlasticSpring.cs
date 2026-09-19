using System;
using WinklerWall.Models;

namespace WinklerWall.Elements
{
    public enum SpringState
    {
        Elastic,
        Active,
        Passive
    }


    public class ElastoPlasticSpring
    {
        public Node Node { get; private set; }

        public double SubgradeModulus { get; private set; }

        public double TributaryLength { get; private set; }

        public double Width { get; private set; }

        public double InitialPressure { get; private set; }

        public double ActivePressure { get; private set; }

        public double PassivePressure { get; private set; }

        public double CurrentPressure { get; private set; }

        public SpringState State { get; private set; }


        // ============================================================
        // ÁREA TRIBUTARIA
        // ============================================================

        public double TributaryArea
        {
            get
            {
                return TributaryLength * Width;
            }
        }


        // ============================================================
        // RIGIDEZ ELÁSTICA NODAL
        // ============================================================

        public double ElasticStiffness
        {
            get
            {
                return
                    SubgradeModulus *
                    TributaryArea;
            }
        }


        // ============================================================
        // DESPLAZAMIENTO DE MOVILIZACIÓN ACTIVA
        // ============================================================

        public double ActiveDisplacement
        {
            get
            {
                return
                    (InitialPressure -
                     ActivePressure) /
                    SubgradeModulus;
            }
        }


        // ============================================================
        // DESPLAZAMIENTO DE MOVILIZACIÓN PASIVA
        // ============================================================

        public double PassiveDisplacement
        {
            get
            {
                return
                    (PassivePressure -
                     InitialPressure) /
                    SubgradeModulus;
            }
        }


        // ============================================================
        // RESISTENCIA ACTIVA INCREMENTAL
        // ============================================================

        public double ActiveResistance
        {
            get
            {
                return
                    (InitialPressure -
                     ActivePressure) *
                    TributaryArea;
            }
        }


        // ============================================================
        // RESISTENCIA PASIVA INCREMENTAL
        // ============================================================

        public double PassiveResistance
        {
            get
            {
                return
                    (PassivePressure -
                     InitialPressure) *
                    TributaryArea;
            }
        }


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public ElastoPlasticSpring(
            Node node,
            double subgradeModulus,
            double tributaryLength,
            double width,
            double initialPressure,
            double activePressure,
            double passivePressure)
        {
            if (node == null)
            {
                throw new ArgumentNullException(
                    nameof(node));
            }

            if (subgradeModulus <= 0.0)
            {
                throw new ArgumentException(
                    "KH debe ser mayor que cero.");
            }

            if (tributaryLength < 0.0)
            {
                throw new ArgumentException(
                    "La longitud tributaria no puede ser negativa.");
            }

            if (width <= 0.0)
            {
                throw new ArgumentException(
                    "El ancho debe ser mayor que cero.");
            }

            if (activePressure > initialPressure)
            {
                throw new ArgumentException(
                    "La presión activa no puede superar la presión inicial.");
            }

            if (passivePressure < initialPressure)
            {
                throw new ArgumentException(
                    "La presión pasiva no puede ser menor que la presión inicial.");
            }


            Node = node;

            SubgradeModulus =
                subgradeModulus;

            TributaryLength =
                tributaryLength;

            Width =
                width;

            InitialPressure =
                initialPressure;

            ActivePressure =
                activePressure;

            PassivePressure =
                passivePressure;

            CurrentPressure =
                initialPressure;

            State =
                SpringState.Elastic;
        }


        // ============================================================
        // ACTUALIZAR ESTADO
        // ============================================================

        public void UpdateState(
            double displacement)
        {
            // ========================================================
            // PROTECCIÓN CONTRA NaN E INFINITO
            // ========================================================

            if (double.IsNaN(displacement) ||
                double.IsInfinity(displacement))
            {
                throw new InvalidOperationException(
                    "El resorte del nodo " +
                    Node.Id +
                    " recibió un desplazamiento numéricamente inválido.");
            }


            const double tolerance =
                1e-10;


            // ========================================================
            // ESTADO ACTIVO
            // ========================================================

            if (displacement >=
                ActiveDisplacement - tolerance)
            {
                State =
                    SpringState.Active;

                CurrentPressure =
                    ActivePressure;

                return;
            }


            // ========================================================
            // ESTADO PASIVO
            // ========================================================

            if (displacement <=
                -PassiveDisplacement + tolerance)
            {
                State =
                    SpringState.Passive;

                CurrentPressure =
                    PassivePressure;

                return;
            }


            // ========================================================
            // ESTADO ELÁSTICO
            // ========================================================

            State =
                SpringState.Elastic;

            CurrentPressure =
                InitialPressure -
                SubgradeModulus *
                displacement;


            if (double.IsNaN(CurrentPressure) ||
                double.IsInfinity(CurrentPressure))
            {
                throw new InvalidOperationException(
                    "La presión calculada en el nodo " +
                    Node.Id +
                    " no es numéricamente válida.");
            }
        }


        // ============================================================
        // FUERZA INTERNA DEL RESORTE
        // ============================================================

        public double GetInternalResistance(
            double displacement)
        {
            if (double.IsNaN(displacement) ||
                double.IsInfinity(displacement))
            {
                throw new InvalidOperationException(
                    "Desplazamiento inválido en el nodo " +
                    Node.Id +
                    ".");
            }


            if (State == SpringState.Active)
            {
                return
                    ActiveResistance;
            }


            if (State == SpringState.Passive)
            {
                return
                    -PassiveResistance;
            }


            return
                ElasticStiffness *
                displacement;
        }


        // ============================================================
        // RIGIDEZ TANGENTE
        // ============================================================

        public double GetTangentStiffness()
        {
            if (State ==
                SpringState.Elastic)
            {
                return
                    ElasticStiffness;
            }


            return 0.0;
        }


        // ============================================================
        // REINICIAR
        // ============================================================

        public void Reset()
        {
            State =
                SpringState.Elastic;

            CurrentPressure =
                InitialPressure;
        }
    }
}
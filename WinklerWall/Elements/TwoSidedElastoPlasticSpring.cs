using System;
using WinklerWall.Models;

namespace WinklerWall.Elements
{
    public enum SoilSideState
    {
        Elastic,
        Active,
        Passive
    }


    public class TwoSidedElastoPlasticSpring
    {
        // ============================================================
        // PROPIEDADES BÁSICAS
        // ============================================================

        public Node Node { get; private set; }

        public double SubgradeModulus { get; private set; }

        public double TributaryLength { get; private set; }

        public double Width { get; private set; }

        public double InitialPressure { get; private set; }

        public double ActivePressure { get; private set; }

        public double PassivePressure { get; private set; }


        // ============================================================
        // PRESIONES ACTUALES
        // ============================================================

        public double BackPressure { get; private set; }

        public double FrontPressure { get; private set; }


        // ============================================================
        // ESTADOS
        // ============================================================

        public SoilSideState BackState { get; private set; }

        public SoilSideState FrontState { get; private set; }


        // ============================================================
        // ÁREA TRIBUTARIA
        // ============================================================

        public double TributaryArea
        {
            get
            {
                return
                    TributaryLength *
                    Width;
            }
        }


        // ============================================================
        // RIGIDEZ POR CARA
        //
        // Se adopta KH/2 por cara para recuperar:
        //
        // q = KH*u
        //
        // en el régimen inicial.
        // ============================================================

        public double SideSubgradeModulus
        {
            get
            {
                return
                    SubgradeModulus /
                    2.0;
            }
        }


        public double SideNodalStiffness
        {
            get
            {
                return
                    SideSubgradeModulus *
                    TributaryArea;
            }
        }


        // ============================================================
        // PRESIÓN NETA
        //
        // q = p_front - p_back
        //
        // q > 0 genera resistencia contra u > 0.
        // ============================================================

        public double NetPressure
        {
            get
            {
                return
                    FrontPressure -
                    BackPressure;
            }
        }


        // ============================================================
        // REACCIÓN NODAL NETA
        // ============================================================

        public double NetReaction
        {
            get
            {
                return
                    NetPressure *
                    TributaryArea;
            }
        }


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public TwoSidedElastoPlasticSpring(
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
                    "La presión activa no puede superar p0.");
            }

            if (passivePressure < initialPressure)
            {
                throw new ArgumentException(
                    "La presión pasiva no puede ser menor que p0.");
            }


            Node =
                node;

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


            Reset();
        }


        // ============================================================
        // ACTUALIZAR ESTADO
        //
        // Convención:
        //
        // u > 0 = pantalla hacia excavación
        //
        // Trasdós:
        // disminuye presión -> activo
        //
        // Intradós:
        // aumenta presión -> pasivo
        // ============================================================

        public void UpdateState(
            double displacement)
        {
            if (double.IsNaN(displacement) ||
                double.IsInfinity(displacement))
            {
                throw new InvalidOperationException(
                    "El resorte de dos caras del nodo " +
                    Node.Id +
                    " recibió un desplazamiento inválido.");
            }


            double kSide =
                SideSubgradeModulus;


            // ========================================================
            // TRASDÓS
            // ========================================================

            double backTrial =
                InitialPressure -
                kSide *
                displacement;


            if (backTrial <=
                ActivePressure)
            {
                BackPressure =
                    ActivePressure;

                BackState =
                    SoilSideState.Active;
            }
            else if (backTrial >=
                     PassivePressure)
            {
                BackPressure =
                    PassivePressure;

                BackState =
                    SoilSideState.Passive;
            }
            else
            {
                BackPressure =
                    backTrial;

                BackState =
                    SoilSideState.Elastic;
            }


            // ========================================================
            // INTRADÓS
            // ========================================================

            double frontTrial =
                InitialPressure +
                kSide *
                displacement;


            if (frontTrial <=
                ActivePressure)
            {
                FrontPressure =
                    ActivePressure;

                FrontState =
                    SoilSideState.Active;
            }
            else if (frontTrial >=
                     PassivePressure)
            {
                FrontPressure =
                    PassivePressure;

                FrontState =
                    SoilSideState.Passive;
            }
            else
            {
                FrontPressure =
                    frontTrial;

                FrontState =
                    SoilSideState.Elastic;
            }
        }


        // ============================================================
        // RIGIDEZ TANGENTE NETA
        //
        // Cada cara elástica aporta KH/2.
        //
        // Ambas elásticas:
        // kt = KH
        //
        // Una plastificada:
        // kt = KH/2
        //
        // Ambas plastificadas:
        // kt = 0
        // ============================================================

        public double GetTangentStiffness()
        {
            double stiffness =
                0.0;


            if (BackState ==
                SoilSideState.Elastic)
            {
                stiffness +=
                    SideNodalStiffness;
            }


            if (FrontState ==
                SoilSideState.Elastic)
            {
                stiffness +=
                    SideNodalStiffness;
            }


            return stiffness;
        }


        // ============================================================
        // FUERZA INTERNA
        //
        // Kbeam*U + Rs = F
        // ============================================================

        public double GetInternalResistance()
        {
            return
                NetReaction;
        }


        // ============================================================
        // REINICIAR
        // ============================================================

        public void Reset()
        {
            BackPressure =
                InitialPressure;

            FrontPressure =
                InitialPressure;

            BackState =
                SoilSideState.Elastic;

            FrontState =
                SoilSideState.Elastic;
        }
    }
}
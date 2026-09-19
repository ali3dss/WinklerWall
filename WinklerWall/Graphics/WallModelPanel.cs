using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using WinklerWall.Models;
using WinklerWall.Elements;

namespace WinklerWall
{
    public class WallModelPanel : Panel
    {

        private AnalysisModel model;
        private double[] globalU;

        private double deformationScale = 20.0;

        public double DeformationScale
        {
            get
            {
                return deformationScale;
            }

            set
            {
                if (value < 1.0)
                    value = 1.0;

                deformationScale = value;

                // Obliga al panel a redibujarse
                this.Invalidate();
            }
        }


        public WallModelPanel(
            AnalysisModel model,
            double[] globalU)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (globalU == null)
                throw new ArgumentNullException(nameof(globalU));

            this.model = model;
            this.globalU = globalU;

            this.DoubleBuffered = true;

            this.BackColor = Color.White;

            this.Dock = DockStyle.Fill;
        }

 

        public WallModelPanel(AnalysisModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            this.model = model;

            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            DrawModel(g);
        }


        private void DrawModel(Graphics g)
        {
            // ========================================================
            // DIMENSIONES GENERALES
            // ========================================================

            int marginTop = 75;
            int marginBottom = 65;
            int marginLeft = 70;
            int marginRight = 70;

            int wallX =
                this.ClientSize.Width / 2;

            double usableHeight =
                this.ClientSize.Height
                - marginTop
                - marginBottom;

            if (usableHeight <= 0)
                return;


            // ========================================================
            // ESCALA VERTICAL
            // ========================================================

            double scale =
                usableHeight /
                model.Wall.Length;


            float wallTopY =
                marginTop;

            float wallBottomY =
                (float)(
                    marginTop +
                    model.Wall.Length *
                    scale);


            float excavationY =
                (float)(
                    marginTop +
                    model.ExcavationDepth *
                    scale);


            // ========================================================
            // FUENTES
            // ========================================================

            using (Font titleFont =
                new Font(
                    "Segoe UI",
                    12,
                    FontStyle.Bold))
            using (Font normalFont =
                new Font(
                    "Segoe UI",
                    9))
            using (Font smallFont =
                new Font(
                    "Segoe UI",
                    8))
            using (Font zoneFont =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold))
            {

                // ====================================================
                // COLORES DEL TERRENO
                // ====================================================

                Color retainedSoilColor =
                    Color.FromArgb(
                        235,
                        222,
                        190);

                Color embeddedSoilColor =
                    Color.FromArgb(
                        225,
                        215,
                        185);

                Color excavationColor =
                    Color.FromArgb(
                        245,
                        248,
                        252);


                using (Brush retainedSoilBrush =
                    new SolidBrush(retainedSoilColor))
                using (Brush embeddedSoilBrush =
                    new SolidBrush(embeddedSoilColor))
                using (Brush excavationBrush =
                    new SolidBrush(excavationColor))
                {

                    // ================================================
                    // 1. TERRENO RETENIDO
                    // ================================================

                    RectangleF retainedSoil =
                        new RectangleF(
                            marginLeft,
                            wallTopY,
                            wallX - marginLeft,
                            excavationY - wallTopY);

                    g.FillRectangle(
                        retainedSoilBrush,
                        retainedSoil);


                    // ================================================
                    // 2. ZONA EXCAVADA
                    // ================================================

                    RectangleF excavationArea =
                        new RectangleF(
                            wallX,
                            wallTopY,
                            this.ClientSize.Width
                            - marginRight
                            - wallX,
                            excavationY - wallTopY);

                    g.FillRectangle(
                        excavationBrush,
                        excavationArea);


                    // ================================================
                    // 3. TERRENO BAJO EL FONDO DE EXCAVACIÓN
                    // ================================================

                    RectangleF embeddedSoil =
                        new RectangleF(
                            marginLeft,
                            excavationY,
                            this.ClientSize.Width
                            - marginLeft
                            - marginRight,
                            wallBottomY
                            - excavationY);

                    g.FillRectangle(
                        embeddedSoilBrush,
                        embeddedSoil);
                }


                // ====================================================
                // TÍTULO
                // ====================================================

                g.DrawString(
                    "Modelo discretizado de la pantalla",
                    titleFont,
                    Brushes.Black,
                    20,
                    15);


                // ====================================================
                // ETIQUETA: TERRENO RETENIDO
                // ====================================================

                string retainedText =
                    "TERRENO RETENIDO";

                SizeF retainedSize =
                    g.MeasureString(
                        retainedText,
                        zoneFont);

                g.DrawString(
                    retainedText,
                    zoneFont,
                    Brushes.SaddleBrown,
                    wallX / 2
                    - retainedSize.Width / 2,
                    wallTopY + 15);


                // ====================================================
                // ETIQUETA: EXCAVACIÓN
                // ====================================================

                string excavationText =
                    "EXCAVACIÓN";

                SizeF excavationSize =
                    g.MeasureString(
                        excavationText,
                        zoneFont);

                float excavationTextX =
                    wallX +
                    (
                        this.ClientSize.Width
                        - marginRight
                        - wallX
                        - excavationSize.Width
                    ) / 2;


                g.DrawString(
                    excavationText,
                    zoneFont,
                    Brushes.DarkRed,
                    excavationTextX,
                    wallTopY + 15);


                // ====================================================
                // SUPERFICIE DEL TERRENO
                // ====================================================

                using (Pen groundPen =
                    new Pen(
                        Color.SaddleBrown,
                        2.0f))
                {
                    g.DrawLine(
                        groundPen,
                        marginLeft,
                        wallTopY,
                        this.ClientSize.Width
                        - marginRight,
                        wallTopY);
                }


                // ====================================================
                // FONDO DE EXCAVACIÓN
                // ====================================================

                using (Pen excavationPen =
                    new Pen(
                        Color.DarkRed,
                        2.0f))
                {
                    excavationPen.DashStyle =
                        DashStyle.Dash;

                    g.DrawLine(
                        excavationPen,
                        wallX,
                        excavationY,
                        this.ClientSize.Width
                        - marginRight,
                        excavationY);
                }


                g.DrawString(
                    "Fondo de excavación   z = "
                    + model.ExcavationDepth.ToString("F2")
                    + " m",
                    normalFont,
                    Brushes.DarkRed,
                    wallX + 20,
                    excavationY - 25);


                // ====================================================
                // PANTALLA
                // ====================================================

                using (Pen wallPen =
                    new Pen(
                        Color.Black,
                        6.0f))
                {
                    g.DrawLine(
                        wallPen,
                        wallX,
                        wallTopY,
                        wallX,
                        wallBottomY);
                }


                // ====================================================
                // EMPUJE ACTIVO
                // ====================================================

                DrawEarthPressureArrows(
                    g,
                    wallX,
                    wallTopY,
                    excavationY,
                    scale,
                    normalFont);


                // ====================================================
                // FLECHA GENERAL DE MOVIMIENTO
                // ====================================================

                float movementY =
                    wallTopY +
                    (excavationY - wallTopY)
                    * 0.48f;


                DrawMovementArrow(
                    g,
                    wallX + 25,
                    movementY,
                    wallX + 155,
                    movementY);


                g.DrawString(
                    "Movimiento hacia\nla excavación",
                    normalFont,
                    Brushes.DarkBlue,
                    wallX + 165,
                    movementY - 18);


                // ====================================================
                // NODOS
                // ====================================================

                foreach (Node node in model.Nodes)
                {
                    float y =
                        (float)(
                            marginTop +
                            node.Z *
                            scale);


                    float radius =
                        6.0f;


                    g.FillEllipse(
                        Brushes.DarkBlue,
                        wallX - radius,
                        y - radius,
                        radius * 2,
                        radius * 2);


                    string nodeText =
                        "N" + node.Id;


                    g.DrawString(
                        nodeText,
                        normalFont,
                        Brushes.Black,
                        wallX + 15,
                        y - 11);


                    string depthText =
                        "z = "
                        + node.Z.ToString("F2")
                        + " m";


                    g.DrawString(
                        depthText,
                        smallFont,
                        Brushes.DimGray,
                        wallX + 58,
                        y - 9);
                }

                // ========================================================
                // PANTALLA DEFORMADA
                // ========================================================

                Pen deformedPen =
                    new Pen(
                        Color.Red,
                        3);

                PointF[] deformedPoints =
                    new PointF[model.Nodes.Count];


                for (int i = 0;
                     i < model.Nodes.Count;
                     i++)
                {
                    Node node =
                        model.Nodes[i];

                    double displacement =
                        globalU[
                            node.HorizontalDof];

                    float y =
                        (float)(
                            marginTop +
                            node.Z *
                            scale);


                    // ----------------------------------------------------
                    // El desplazamiento está en metros.
                    //
                    // Lo convertimos a píxeles utilizando la misma escala
                    // vertical y posteriormente aplicamos el factor gráfico.
                    // ----------------------------------------------------

                    float deformationPixels =
                        (float)(
                            displacement *
                            scale *
                            deformationScale);


                    float x =
                        wallX +
                        deformationPixels;


                    deformedPoints[i] =
                        new PointF(
                            x,
                            y);
                }


                if (deformedPoints.Length > 1)
                {
                    g.DrawLines(
                        deformedPen,
                        deformedPoints);
                }


                // ========================================================
                // NODOS SOBRE LA DEFORMADA
                // ========================================================

                Brush deformedNodeBrush =
                    Brushes.Red;


                foreach (Node node in model.Nodes)
                {
                    double displacement =
                        globalU[
                            node.HorizontalDof];

                    float y =
                        (float)(
                            marginTop +
                            node.Z *
                            scale);

                    float deformationPixels =
                        (float)(
                            displacement *
                            scale *
                            deformationScale);

                    float x =
                        wallX +
                        deformationPixels;


                    float radius =
                        4.0f;


                    g.FillEllipse(
                        deformedNodeBrush,
                        x - radius,
                        y - radius,
                        radius * 2.0f,
                        radius * 2.0f);
                }

  


                // ====================================================
                // RESORTES WINKLER
                // ====================================================

                using (Pen springPen =
                    new Pen(
                        Color.SteelBlue,
                        1.7f))
                {
                    foreach (WinklerSpring spring
                             in model.Springs)
                    {
                        float y =
                            (float)(
                                marginTop +
                                spring.Node.Z *
                                scale);

                        DrawSpring(
                            g,
                            springPen,
                            wallX,
                            y);
                    }
                }


                // ====================================================
                // ETIQUETA ZONA EMPOTRADA
                // ====================================================

                float embeddedLabelY =
                    excavationY +
                    25;


                g.DrawString(
                    "TERRENO DE EMPOTRAMIENTO",
                    normalFont,
                    Brushes.SaddleBrown,
                    marginLeft + 15,
                    embeddedLabelY);


                // ====================================================
                // LONGITUD TOTAL
                // ====================================================

                g.DrawString(
                    "L = "
                    + model.Wall.Length.ToString("F2")
                    + " m",
                    normalFont,
                    Brushes.Black,
                    wallX - 120,
                    wallBottomY - 5);


                // ====================================================
                // LEYENDA
                // ====================================================

                DrawLegend(
                    g,
                    marginLeft,
                    wallBottomY + 20,
                    smallFont);
            }
        }


        // ============================================================
        // FLECHAS DE EMPUJE
        // ============================================================

        private void DrawEarthPressureArrows(
            Graphics g,
            float wallX,
            float topY,
            float excavationY,
            double scale,
            Font font)
        {
            using (Pen arrowPen =
                new Pen(
                    Color.DarkOrange,
                    1.8f))
            {
                arrowPen.CustomEndCap =
                    new AdjustableArrowCap(
                        4,
                        5);


                int numberOfArrows = 5;

                for (int i = 1;
                     i <= numberOfArrows;
                     i++)
                {
                    double ratio =
                        (double)i /
                        numberOfArrows;

                    float y =
                        topY +
                        (float)(
                            ratio *
                            (excavationY - topY));


                    // La longitud crece con la profundidad,
                    // representando aproximadamente el empuje triangular.

                    float arrowLength =
                        25.0f +
                        75.0f *
                        (float)ratio;


                    float startX =
                        wallX -
                        arrowLength;


                    float endX =
                        wallX - 8;


                    g.DrawLine(
                        arrowPen,
                        startX,
                        y,
                        endX,
                        y);
                }
            }


            g.DrawString(
                "Empuje activo",
                font,
                Brushes.DarkOrange,
                wallX - 145,
                topY + 65);
        }


        // ============================================================
        // FLECHA DE MOVIMIENTO
        // ============================================================

        private void DrawMovementArrow(
            Graphics g,
            float startX,
            float startY,
            float endX,
            float endY)
        {
            using (Pen movementPen =
                new Pen(
                    Color.DarkBlue,
                    3.0f))
            {
                movementPen.CustomEndCap =
                    new AdjustableArrowCap(
                        6,
                        7);


                g.DrawLine(
                    movementPen,
                    startX,
                    startY,
                    endX,
                    endY);
            }
        }


        // ============================================================
        // RESORTE
        // ============================================================

        private void DrawSpring(
            Graphics g,
            Pen pen,
            float startX,
            float y)
        {
            float x0 =
                startX;

            float x1 =
                startX - 18;

            float x2 =
                startX - 32;

            float x3 =
                startX - 46;

            float x4 =
                startX - 60;

            float x5 =
                startX - 78;


            PointF[] points =
            {
                new PointF(x0, y),
                new PointF(x1, y),
                new PointF(x2, y - 7),
                new PointF(x3, y + 7),
                new PointF(x4, y - 7),
                new PointF(x5, y)
            };


            g.DrawLines(
                pen,
                points);


            // Apoyo lateral

            g.DrawLine(
                pen,
                x5,
                y - 12,
                x5,
                y + 12);

            g.DrawLine(
                pen,
                x5 - 6,
                y - 9,
                x5,
                y - 4);

            g.DrawLine(
                pen,
                x5 - 6,
                y,
                x5,
                y + 5);

            g.DrawLine(
                pen,
                x5 - 6,
                y + 9,
                x5,
                y + 14);
        }


        // ============================================================
        // LEYENDA
        // ============================================================

        private void DrawLegend(
            Graphics g,
            float x,
            float y,
            Font font)
        {
            g.FillRectangle(
                Brushes.DarkBlue,
                x,
                y,
                9,
                9);

            g.DrawString(
                "Nodo",
                font,
                Brushes.Black,
                x + 15,
                y - 4);


            using (Pen springPen =
                new Pen(
                    Color.SteelBlue,
                    2))
            {
                g.DrawLine(
                    springPen,
                    x + 80,
                    y + 4,
                    x + 110,
                    y + 4);
            }

            g.DrawString(
                "Resorte Winkler",
                font,
                Brushes.Black,
                x + 118,
                y - 4);


            using (Pen pressurePen =
                new Pen(
                    Color.DarkOrange,
                    2))
            {
                pressurePen.CustomEndCap =
                    new AdjustableArrowCap(
                        3,
                        4);

                g.DrawLine(
                    pressurePen,
                    x + 240,
                    y + 4,
                    x + 275,
                    y + 4);
            }


            g.DrawString(
                "Empuje",
                font,
                Brushes.Black,
                x + 285,
                y - 4);
        }
    }
}
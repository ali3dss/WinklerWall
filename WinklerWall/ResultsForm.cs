using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using WinklerWall.Models;
using WinklerWall.Elements;
using WinklerWall.Results;

namespace WinklerWall
{
    public partial class ResultsForm : Form
    {
        private AnalysisModel model;
        private double[] globalU;
        private List<BeamElementResult> elementResults;


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public ResultsForm(
            AnalysisModel model,
            double[] globalU,
            List<BeamElementResult> elementResults)
        {
            InitializeComponent();

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (globalU == null)
                throw new ArgumentNullException(nameof(globalU));

            if (elementResults == null)
                throw new ArgumentNullException(nameof(elementResults));

            this.model = model;
            this.globalU = globalU;
            this.elementResults = elementResults;

            ConfigureForm();
            BuildInterface();
        }


        // ============================================================
        // CONFIGURAR VENTANA
        // ============================================================

        private void ConfigureForm()
        {
            this.Text =
                "Resultados - Pantalla Winkler";

            this.Width = 1100;
            this.Height = 750;

            this.StartPosition =
                FormStartPosition.CenterScreen;
        }


        // ============================================================
        // CONSTRUIR INTERFAZ
        // ============================================================

        private void BuildInterface()
        {
            TabControl tabs =
                new TabControl();

            tabs.Dock =
                DockStyle.Fill;

            // ========================================================
            // MODELO ESTRUCTURAL
            // ========================================================

            TabPage tabModel =
                new TabPage(
                    "Modelo");


            // ========================================================
            // PANEL SUPERIOR DE CONTROLES
            // ========================================================

            Panel controlPanel =
                new Panel();

            controlPanel.Dock =
                DockStyle.Top;

            controlPanel.Height =
                75;


            // ========================================================
            // ETIQUETA
            // ========================================================

            Label lblScale =
                new Label();

            lblScale.Text =
                "Factor de amplificación de la deformada:";

            lblScale.AutoSize =
                true;

            lblScale.Left =
                20;

            lblScale.Top =
                12;


            // ========================================================
            // VALOR ACTUAL
            // ========================================================

            Label lblScaleValue =
                new Label();

            lblScaleValue.Text =
                "20x";

            lblScaleValue.AutoSize =
                true;

            lblScaleValue.Left =
                290;

            lblScaleValue.Top =
                12;

            lblScaleValue.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9,
                    System.Drawing.FontStyle.Bold);


            // ========================================================
            // TRACKBAR
            // ========================================================

            TrackBar deformationTrackBar =
                new TrackBar();

            deformationTrackBar.Minimum =
                1;

            deformationTrackBar.Maximum =
                100;

            deformationTrackBar.Value =
                20;

            deformationTrackBar.TickFrequency =
                10;

            deformationTrackBar.SmallChange =
                1;

            deformationTrackBar.LargeChange =
                10;

            deformationTrackBar.Left =
                20;

            deformationTrackBar.Top =
                32;

            deformationTrackBar.Width =
                380;


            // ========================================================
            // PANEL DE DIBUJO
            // ========================================================

            WallModelPanel wallModelPanel =
                new WallModelPanel(
                    model,
                    globalU);

            wallModelPanel.DeformationScale =
                deformationTrackBar.Value;


            // ========================================================
            // EVENTO DEL TRACKBAR
            // ========================================================

            deformationTrackBar.ValueChanged +=
                delegate
                {
                    double scale =
                        deformationTrackBar.Value;

                    lblScaleValue.Text =
                        scale.ToString("F0")
                        + "x";

                    wallModelPanel.DeformationScale =
                        scale;
                };


            // ========================================================
            // AGREGAR CONTROLES
            // ========================================================

            controlPanel.Controls.Add(
                lblScale);

            controlPanel.Controls.Add(
                lblScaleValue);

            controlPanel.Controls.Add(
                deformationTrackBar);


            tabModel.Controls.Add(
                wallModelPanel);

            tabModel.Controls.Add(
                controlPanel);


            tabs.TabPages.Add(
                tabModel);


            // ========================================================
            // DEFORMADA
            // ========================================================

            Chart chartDisplacement =
                CreateBaseChart(
                    "Desplazamiento horizontal",
                    "u [mm]");


            PlotDisplacement(
                chartDisplacement);


            TabPage tabDisplacement =
                new TabPage(
                    "Deformada");




            tabDisplacement.Controls.Add(
                chartDisplacement);


            tabs.TabPages.Add(
                tabDisplacement);


            // ========================================================
            // MOMENTO
            // ========================================================

            Chart chartMoment =
                CreateBaseChart(
                    "Momento flector",
                    "M [kN·m/m]");


            PlotMoment(
                chartMoment);


            TabPage tabMoment =
                new TabPage(
                    "Momento");


            tabMoment.Controls.Add(
                chartMoment);


            tabs.TabPages.Add(
                tabMoment);


            // ========================================================
            // CORTANTE
            // ========================================================

            Chart chartShear =
                CreateBaseChart(
                    "Fuerza cortante",
                    "Q [kN/m]");


            PlotShear(
                chartShear);


            TabPage tabShear =
                new TabPage(
                    "Cortante");


            tabShear.Controls.Add(
                chartShear);


            tabs.TabPages.Add(
                tabShear);


            // ========================================================
            // REACCIONES
            // ========================================================

            Chart chartReaction =
                CreateBaseChart(
                    "Reacción del terreno",
                    "R [kN/m]");


            PlotReactions(
                chartReaction);


            TabPage tabReaction =
                new TabPage(
                    "Reacciones");


            tabReaction.Controls.Add(
                chartReaction);


            tabs.TabPages.Add(
                tabReaction);


            // ========================================================
            // AGREGAR TODO AL FORMULARIO
            // ========================================================

            this.Controls.Add(
                tabs);
        }


        // ============================================================
        // CREAR GRÁFICA BASE
        // ============================================================

        private Chart CreateBaseChart(
            string title,
            string horizontalAxisTitle)
        {
            Chart chart =
                new Chart();

            chart.Dock =
                DockStyle.Fill;


            ChartArea area =
                new ChartArea(
                    "MainArea");


            // --------------------------------------------------------
            // EJE HORIZONTAL
            // --------------------------------------------------------

            area.AxisX.Title =
                horizontalAxisTitle;

            area.AxisX.MajorGrid.Enabled =
                true;


            // --------------------------------------------------------
            // EJE VERTICAL
            // --------------------------------------------------------

            area.AxisY.Title =
                "Profundidad z [m]";

            area.AxisY.Minimum =
                0.0;

            area.AxisY.Maximum =
                model.Wall.Length;

            area.AxisY.Interval =
                1.0;

            // Profundidad positiva hacia abajo
            area.AxisY.IsReversed =
                true;

            area.AxisY.MajorGrid.Enabled =
                true;


            chart.ChartAreas.Add(
                area);


            // --------------------------------------------------------
            // TÍTULO
            // --------------------------------------------------------

            Title chartTitle =
                new Title(title);

            chartTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    12,
                    System.Drawing.FontStyle.Bold);


            chart.Titles.Add(
                chartTitle);


            // --------------------------------------------------------
            // LEYENDA
            // --------------------------------------------------------

            Legend legend =
                new Legend();

            legend.Docking =
                Docking.Bottom;

            chart.Legends.Add(
                legend);


            return chart;
        }


        // ============================================================
        // DIBUJAR LÍNEA DE CERO
        // ============================================================

        private void AddZeroLine(
            Chart chart)
        {
            Series zero =
                new Series(
                    "Referencia 0");

            zero.ChartType =
                SeriesChartType.Line;

            zero.BorderDashStyle =
                ChartDashStyle.Dash;

            zero.BorderWidth =
                1;

            zero.IsVisibleInLegend =
                false;


            zero.Points.AddXY(
                0.0,
                0.0);

            zero.Points.AddXY(
                0.0,
                model.Wall.Length);


            chart.Series.Add(
                zero);
        }


        // ============================================================
        // DEFORMADA
        // ============================================================

        private void PlotDisplacement(
            Chart chart)
        {
            AddZeroLine(
                chart);


            Series series =
                new Series(
                    "u(z)");


            series.ChartType =
                SeriesChartType.Line;

            series.BorderWidth =
                2;

            series.MarkerStyle =
                MarkerStyle.Circle;

            series.MarkerSize =
                6;


            foreach (Node node in model.Nodes)
            {
                double displacement =
                    globalU[
                        node.HorizontalDof];

                double displacementMm =
                    displacement *
                    1000.0;


                series.Points.AddXY(
                    displacementMm,
                    node.Z);
            }


            chart.Series.Add(
                series);
        }


        // ============================================================
        // MOMENTO FLECTOR
        // ============================================================

        private void PlotMoment(
            Chart chart)
        {
            AddZeroLine(
                chart);


            Series series =
                new Series(
                    "M(z)");


            series.ChartType =
                SeriesChartType.Line;

            series.BorderWidth =
                2;

            series.MarkerStyle =
                MarkerStyle.Circle;

            series.MarkerSize =
                5;


            if (elementResults.Count == 0)
                return;


            // ========================================================
            // PRIMER EXTREMO
            //
            // Las acciones nodales de elementos contiguos aparecen
            // con signos opuestos.
            //
            // Para representar el momento físico continuo:
            //
            // Inicio del elemento = -Mi
            // Final del elemento  =  Mj
            // ========================================================

            BeamElementResult first =
                elementResults[0];


            series.Points.AddXY(
                -first.MomentI,
                first.Zi);


            foreach (BeamElementResult result
                     in elementResults)
            {
                series.Points.AddXY(
                    result.MomentJ,
                    result.Zj);
            }


            chart.Series.Add(
                series);
        }


        // ============================================================
        // CORTANTE
        // ============================================================

        private void PlotShear(
            Chart chart)
        {
            AddZeroLine(
                chart);


            Series series =
                new Series(
                    "Q(z)");


            series.ChartType =
                SeriesChartType.Line;

            series.BorderWidth =
                2;

            series.MarkerStyle =
                MarkerStyle.Circle;

            series.MarkerSize =
                5;


            // ========================================================
            // Para el cortante:
            //
            // Inicio del elemento = Qi
            // Final del elemento  = -Qj
            //
            // Cuando existe un resorte nodal aparece un salto en Q.
            // Por ello mantenemos ambos valores en una misma
            // profundidad.
            // ========================================================

            foreach (BeamElementResult result
                     in elementResults)
            {
                series.Points.AddXY(
                    result.ShearI,
                    result.Zi);

                series.Points.AddXY(
                    -result.ShearJ,
                    result.Zj);
            }


            chart.Series.Add(
                series);
        }


        // ============================================================
        // REACCIONES DEL TERRENO
        // ============================================================

        private void PlotReactions(
            Chart chart)
        {
            AddZeroLine(
                chart);


            Series series =
                new Series(
                    "R(z)");


            series.ChartType =
                SeriesChartType.Line;

            series.BorderWidth =
                2;

            series.MarkerStyle =
                MarkerStyle.Circle;

            series.MarkerSize =
                7;


            foreach (WinklerSpring spring
                     in model.Springs)
            {
                double displacement =
                    globalU[
                        spring.Node.HorizontalDof];

                double reaction =
                    spring.Stiffness *
                    displacement;


                series.Points.AddXY(
                    reaction,
                    spring.Node.Z);
            }


            chart.Series.Add(
                series);
        }
    }
}
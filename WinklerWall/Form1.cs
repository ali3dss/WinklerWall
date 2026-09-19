using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinklerWall.Models;
using WinklerWall.Solver;
using WinklerWall.Elements;
using WinklerWall.Results;



namespace WinklerWall
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void AppendMatrix(
     StringBuilder sb,
     double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            const int columnWidth = 15;

            // Encabezado
            sb.Append("Fila  |");

            for (int j = 0; j < columns; j++)
            {
                sb.Append(
                    $"{"C" + j,columnWidth}|");
            }

            sb.AppendLine();

            // Separador
            sb.Append("------|");

            for (int j = 0; j < columns; j++)
            {
                sb.Append(
                    new string('-', columnWidth));

                sb.Append("|");
            }

            sb.AppendLine();

            // Valores de la matriz
            for (int i = 0; i < rows; i++)
            {
                sb.Append($"{i,5} |");

                for (int j = 0; j < columns; j++)
                {
                    sb.Append(
                        $"{matrix[i, j],columnWidth:0.000E+00}|");
                }

                sb.AppendLine();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

             try
            {
                // ============================================================
                // 1. LEER DATOS DE ENTRADA
                // ============================================================

                double wallLength;
                double excavationDepth;
                double thickness;
                double elasticModulusGPa;
                double width;
                double phiDegrees;
                double gamma;
                double kh;
                double elementLength;


                // ============================================================
                // CONVERSIÓN SEGURA DE TEXTOS A NÚMEROS
                // ============================================================

                if (!double.TryParse(txtWallLength.Text, out wallLength))
                {
                    throw new Exception(
                        "La longitud de la pantalla no es válida.");
                }

                if (!double.TryParse(
                        txtExcavationDepth.Text,
                        out excavationDepth))
                {
                    throw new Exception(
                        "La profundidad de excavación no es válida.");
                }

                if (!double.TryParse(
                        txtThickness.Text,
                        out thickness))
                {
                    throw new Exception(
                        "El espesor de la pantalla no es válido.");
                }

                if (!double.TryParse(
                        txtElasticModulus.Text,
                        out elasticModulusGPa))
                {
                    throw new Exception(
                        "El módulo de elasticidad no es válido.");
                }

                if (!double.TryParse(
                        txtWidth.Text,
                        out width))
                {
                    throw new Exception(
                        "El ancho de cálculo no es válido.");
                }

                if (!double.TryParse(
                        txtPhi.Text,
                        out phiDegrees))
                {
                    throw new Exception(
                        "El ángulo de fricción no es válido.");
                }

                if (!double.TryParse(
                        txtGamma.Text,
                        out gamma))
                {
                    throw new Exception(
                        "El peso unitario no es válido.");
                }

                if (!double.TryParse(
                        txtKh.Text,
                        out kh))
                {
                    throw new Exception(
                        "El módulo de balasto KH no es válido.");
                }

                if (!double.TryParse(
                        txtElementLength.Text,
                        out elementLength))
                {
                    throw new Exception(
                        "La longitud de elemento no es válida.");
                }

                // ============================================================
                // 2. VALIDAR DATOS
                // ============================================================

                if (wallLength <= 0.0)
                {
                    throw new Exception(
                        "La longitud de la pantalla debe ser mayor que cero.");
                }

                if (excavationDepth <= 0.0)
                {
                    throw new Exception(
                        "La profundidad de excavación debe ser mayor que cero.");
                }

                if (excavationDepth >= wallLength)
                {
                    throw new Exception(
                        "La profundidad de excavación debe ser menor que la longitud total de la pantalla.");
                }

                if (thickness <= 0.0)
                {
                    throw new Exception(
                        "El espesor debe ser mayor que cero.");
                }

                if (elasticModulusGPa <= 0.0)
                {
                    throw new Exception(
                        "El módulo de elasticidad debe ser mayor que cero.");
                }

                if (width <= 0.0)
                {
                    throw new Exception(
                        "El ancho de cálculo debe ser mayor que cero.");
                }

                if (phiDegrees <= 0.0 ||
                    phiDegrees >= 90.0)
                {
                    throw new Exception(
                        "El ángulo de fricción debe estar entre 0 y 90 grados.");
                }

                if (gamma <= 0.0)
                {
                    throw new Exception(
                        "El peso unitario debe ser mayor que cero.");
                }

                if (kh <= 0.0)
                {
                    throw new Exception(
                        "El módulo de balasto KH debe ser mayor que cero.");
                }

                if (elementLength <= 0.0)
                {
                    throw new Exception(
                        "La longitud del elemento debe ser mayor que cero.");
                }

                double elasticModulus =
    elasticModulusGPa *
    1000000.0;

                // ============================================================
                // 3. CREAR PANTALLA
                // ============================================================

                Wall wall = new Wall(
                    length: wallLength,
                    thickness: thickness,
                    elasticModulus: elasticModulus,
                    width: width
                );

                // ============================================================
                // 4. CREAR MODELO
                // ============================================================

                AnalysisModel model =
                    new AnalysisModel(
                        wall: wall,
                        excavationDepth: excavationDepth,
                        elementLength: elementLength,
                        subgradeModulus: kh
                    );



                // ====================================================
                // 3. PARÁMETROS DEL TERRENO
                // ====================================================


                double K0 =
    EarthPressureCalculator.CalculateK0(
        phiDegrees);

                double Ka =
                    EarthPressureCalculator.CalculateKa(
                        phiDegrees);

                double Kp =
                    EarthPressureCalculator.CalculateKp(
                        phiDegrees);



                // ====================================================
                // 4. MATRIZ GLOBAL DE RIGIDEZ
                // ====================================================

                double[,] globalK =
                    MatrixAssembler.AssembleGlobalStiffness(
                        model.Nodes,
                        model.Elements,
                        model.Springs
                    );


                // ====================================================
                // 5. MATRIZ DE LA VIGA SIN RESORTES
                // ====================================================

                double[,] beamK =
                    MatrixAssembler.AssembleGlobalStiffness(
                        model.Nodes,
                        model.Elements,
                        new List<WinklerSpring>()
                    );


                // ====================================================
                // 6. VECTOR GLOBAL DE CARGAS
                // ====================================================

                double[] globalF =
                    LoadVectorAssembler.AssembleActiveEarthPressure(
                        model.Nodes,
                        model.Elements,
                        model.ExcavationDepth,
                        Ka,
                        gamma,
                        wall.Width
                    );


                // ====================================================
                // 7. RESOLVER K * U = F
                // ====================================================

                double[] globalU =
                    LinearSolver.Solve(
                        globalK,
                        globalF
                    );

                NonlinearAnalysisResult nonlinearResult =
    NonlinearSolver.Solve(
        model,
        globalF,
        phiDegrees,
        gamma);


                // ====================================================
                // 8. COMPROBAR RESIDUO NUMÉRICO
                // ====================================================

                double maximumResidual =
                    LinearSolver.GetMaximumResidual(
                        globalK,
                        globalU,
                        globalF
                    );


                // ====================================================
                // 9. RECUPERAR ESFUERZOS DE LOS ELEMENTOS
                // ====================================================

                List<BeamElementResult> elementResults =
                    ElementForceRecovery.Recover(
                        model,
                        globalU,
                        Ka,
                        gamma,
                        wall.Width
                    );


                // ====================================================
                // 10. PREPARAR SALIDA
                // ====================================================

                StringBuilder sb = new StringBuilder();


                // ====================================================
                // TÍTULO
                // ====================================================

                sb.AppendLine("MODELO DE PANTALLA WINKLER");
                sb.AppendLine("===========================");
                sb.AppendLine();


                // ====================================================
                // GEOMETRÍA
                // ====================================================

                sb.AppendLine("GEOMETRÍA");
                sb.AppendLine("---------------------------");

                sb.AppendLine(
                    $"Longitud total: {wall.Length:F2} m");

                sb.AppendLine(
                    $"Espesor: {wall.Thickness:F2} m");

                sb.AppendLine(
                    $"Ancho de cálculo: {wall.Width:F2} m");

                sb.AppendLine(
                    $"Profundidad de excavación: {model.ExcavationDepth:F2} m");

                sb.AppendLine();


                // ====================================================
                // PROPIEDADES ESTRUCTURALES
                // ====================================================

                sb.AppendLine("PROPIEDADES ESTRUCTURALES");
                sb.AppendLine("---------------------------");

                sb.AppendLine(
                    $"E = {wall.ElasticModulus:N0} kN/m²");

                sb.AppendLine(
                    $"I = {wall.Inertia:F6} m⁴");

                sb.AppendLine(
                    $"EI = {wall.FlexuralRigidity:N2} kN·m²");

                sb.AppendLine();


                // ====================================================
                // PARÁMETROS GEOTÉCNICOS
                // ====================================================

                sb.AppendLine("PARÁMETROS GEOTÉCNICOS");
                sb.AppendLine("---------------------------");

                sb.AppendLine(
                    $"Phi = {phiDegrees:F2} grados");

                sb.AppendLine(
                    $"Ka = {Ka:F6}");

                sb.AppendLine(
                    $"Gamma = {gamma:F2} kN/m³");

                sb.AppendLine(
                    $"KH = {model.SubgradeModulus:N2} kN/m³");

                sb.AppendLine(
    $"K0 = {K0:F6}");

                sb.AppendLine(
                    $"Ka = {Ka:F6}");

                sb.AppendLine(
                    $"Kp = {Kp:F6}");

                sb.AppendLine();
                sb.AppendLine("ESTADOS DE PRESIÓN DEL TERRENO");
                sb.AppendLine("==============================");

                sb.AppendLine(
                    "Nodo | z [m] | " +
                    "pa [kPa] | " +
                    "p0 [kPa] | " +
                    "pp [kPa]");

                sb.AppendLine(
                    "-----------------------------------------------");

                foreach (Node node in model.Nodes)
                {
                    if (node.Z < model.ExcavationDepth)
                        continue;

                    double pa =
                        EarthPressureCalculator.ActivePressure(
                            Ka,
                            gamma,
                            node.Z);

                    double p0 =
                        EarthPressureCalculator.InitialPressure(
                            K0,
                            gamma,
                            node.Z);

                    double pp =
                        EarthPressureCalculator.PassivePressure(
                            Kp,
                            gamma,
                            node.Z);

                    sb.AppendLine(
                        $"{node.Id,4} | " +
                        $"{node.Z,5:F2} | " +
                        $"{pa,8:F3} | " +
                        $"{p0,8:F3} | " +
                        $"{pp,8:F3}");
                }

                sb.AppendLine();


                // ====================================================
                // DISCRETIZACIÓN
                // ====================================================

                sb.AppendLine("DISCRETIZACIÓN");
                sb.AppendLine("---------------------------");

                sb.AppendLine(
                    $"Número de nodos: {model.Nodes.Count}");

                sb.AppendLine(
                    $"Número de elementos: {model.Elements.Count}");

                sb.AppendLine(
                    $"Número de resortes: {model.Springs.Count}");

                sb.AppendLine(
                    $"Longitud de elemento: {model.ElementLength:F2} m");

                sb.AppendLine();


                // ====================================================
                // NODOS
                // ====================================================

                sb.AppendLine("NODOS");
                sb.AppendLine("---------------------------");

                foreach (Node node in model.Nodes)
                {
                    sb.AppendLine(
                        $"Nodo {node.Id,2} | " +
                        $"z = {node.Z,5:F2} m | " +
                        $"DOF u = {node.HorizontalDof,2} | " +
                        $"DOF theta = {node.RotationDof,2}");
                }

                sb.AppendLine();


                // ====================================================
                // RESORTES WINKLER
                // ====================================================

                sb.AppendLine("RESORTES WINKLER");
                sb.AppendLine("---------------------------");

                foreach (WinklerSpring spring in model.Springs)
                {
                    sb.AppendLine(
                        $"Nodo {spring.Node.Id,2} | " +
                        $"z = {spring.Node.Z,5:F2} m | " +
                        $"Lt = {spring.TributaryLength:F2} m | " +
                        $"k = {spring.Stiffness:N2} kN/m");
                }

                sb.AppendLine();


                // ====================================================
                // MATRIZ GLOBAL
                // ====================================================

                sb.AppendLine("MATRIZ GLOBAL DE RIGIDEZ");
                sb.AppendLine("===========================");

                int rows =
                    globalK.GetLength(0);

                int columns =
                    globalK.GetLength(1);

                sb.AppendLine(
                    $"Dimensión: {rows} x {columns}");

                bool isSymmetric =
                    MatrixDiagnostics.IsSymmetric(globalK);

                double maxAsymmetry =
                    MatrixDiagnostics.GetMaximumAsymmetry(globalK);

                sb.AppendLine(
                    $"Simétrica: {(isSymmetric ? "SÍ" : "NO")}");

                sb.AppendLine(
                    $"Máxima asimetría: {maxAsymmetry:E6}");

                sb.AppendLine();


                // ====================================================
                // MATRIZ K
                // ====================================================

                sb.AppendLine("VALORES DE K");
                sb.AppendLine("---------------------------");

                AppendMatrix(
                    sb,
                    globalK
                );

                sb.AppendLine();


                // ====================================================
                // CONTRIBUCIÓN DE LOS RESORTES
                // ====================================================

                sb.AppendLine("CONTRIBUCIÓN DE LOS RESORTES");
                sb.AppendLine("=============================");

                for (int i = 0;
                     i < globalK.GetLength(0);
                     i++)
                {
                    double difference =
                        globalK[i, i] -
                        beamK[i, i];

                    if (Math.Abs(difference) > 1e-8)
                    {
                        sb.AppendLine(
                            $"DOF {i,2} | " +
                            $"Kviga = {beamK[i, i],14:N2} | " +
                            $"Kglobal = {globalK[i, i],14:N2} | " +
                            $"DeltaK = {difference,12:N2}");
                    }
                }

                sb.AppendLine();


                // ====================================================
                // VECTOR GLOBAL DE CARGAS
                // ====================================================

                sb.AppendLine("VECTOR GLOBAL DE CARGAS");
                sb.AppendLine("========================");

                for (int i = 0;
                     i < globalF.Length;
                     i++)
                {
                    string type;

                    if (i % 2 == 0)
                    {
                        type = "Fuerza";
                    }
                    else
                    {
                        type = "Momento";
                    }

                    sb.AppendLine(
                        $"DOF {i,2} | " +
                        $"{type,-7} | " +
                        $"{globalF[i],14:F6}");
                }

                sb.AppendLine();


                // ====================================================
                // RESULTANTE HORIZONTAL
                // ====================================================

                double totalHorizontalForce = 0.0;

                foreach (Node node in model.Nodes)
                {
                    totalHorizontalForce +=
                        globalF[node.HorizontalDof];
                }


                double analyticalActiveForce =
                    0.5 *
                    Ka *
                    gamma *
                    Math.Pow(
                        model.ExcavationDepth,
                        2) *
                    wall.Width;


                double forceDifference =
                    Math.Abs(
                        totalHorizontalForce -
                        analyticalActiveForce);


                sb.AppendLine("VERIFICACIÓN DE RESULTANTE");
                sb.AppendLine("---------------------------");

                sb.AppendLine(
                    $"Fuerza obtenida del vector F = " +
                    $"{totalHorizontalForce:F6} kN/m");

                sb.AppendLine(
                    $"Fuerza analítica Ea = " +
                    $"{analyticalActiveForce:F6} kN/m");

                sb.AppendLine(
                    $"Diferencia = {forceDifference:E6}");

                sb.AppendLine();


                // ====================================================
                // VERIFICACIÓN DEL MOMENTO
                // ====================================================

                double momentAboutExcavation = 0.0;

                foreach (Node node in model.Nodes)
                {
                    if (node.Z > model.ExcavationDepth)
                    {
                        continue;
                    }

                    double leverArm =
                        model.ExcavationDepth -
                        node.Z;

                    double nodalForce =
                        globalF[
                            node.HorizontalDof];

                    double nodalMoment =
                        globalF[
                            node.RotationDof];

                    momentAboutExcavation +=
                        nodalForce *
                        leverArm;

                    momentAboutExcavation -=
                        nodalMoment;
                }


                double analyticalMoment =
                    analyticalActiveForce *
                    model.ExcavationDepth /
                    3.0;


                double momentDifference =
                    Math.Abs(
                        momentAboutExcavation -
                        analyticalMoment);


                sb.AppendLine("VERIFICACIÓN DE MOMENTO");
                sb.AppendLine("---------------------------");

                sb.AppendLine(
                    $"Momento obtenido de F = " +
                    $"{momentAboutExcavation:F6} kN·m/m");

                sb.AppendLine(
                    $"Momento analítico = " +
                    $"{analyticalMoment:F6} kN·m/m");

                sb.AppendLine(
                    $"Diferencia = {momentDifference:E6}");

                sb.AppendLine();


                // ====================================================
                // SOLUCIÓN K * U = F
                // ====================================================

                sb.AppendLine("SOLUCIÓN DEL SISTEMA K * U = F");
                sb.AppendLine("================================");

                sb.AppendLine(
                    $"Residuo máximo K*U-F = " +
                    $"{maximumResidual:E6}");

                sb.AppendLine();

                sb.AppendLine(
                    "Nodo |   z [m] |" +
                    "       u [m] |" +
                    "      u [mm] |" +
                    "     theta [rad]");

                sb.AppendLine(
                    "------------------------------------------------------------");


                foreach (Node node in model.Nodes)
                {
                    double displacement =
                        globalU[
                            node.HorizontalDof];

                    double rotation =
                        globalU[
                            node.RotationDof];

                    double displacementMm =
                        displacement *
                        1000.0;

                    sb.AppendLine(
                        $"{node.Id,4} | " +
                        $"{node.Z,7:F2} | " +
                        $"{displacement,11:F8} | " +
                        $"{displacementMm,11:F4} | " +
                        $"{rotation,15:E6}");
                }

                sb.AppendLine();


                // ====================================================
                // REACCIONES DE LOS RESORTES
                // ====================================================

                sb.AppendLine("REACCIONES DE LOS RESORTES");
                sb.AppendLine("============================");

                double totalSpringReaction =
                    0.0;


                foreach (WinklerSpring spring in model.Springs)
                {
                    double displacement =
                        globalU[
                            spring.Node.HorizontalDof];

                    double reaction =
                        spring.Stiffness *
                        displacement;

                    totalSpringReaction +=
                        reaction;

                    sb.AppendLine(
                        $"Nodo {spring.Node.Id,2} | " +
                        $"z = {spring.Node.Z,5:F2} m | " +
                        $"u = {displacement * 1000.0,9:F4} mm | " +
                        $"k = {spring.Stiffness,10:F2} kN/m | " +
                        $"R = {reaction,10:F4} kN/m");
                }

                sb.AppendLine();


                double reactionDifference =
                    Math.Abs(
                        totalSpringReaction -
                        analyticalActiveForce);


                sb.AppendLine(
                    $"Suma de reacciones = " +
                    $"{totalSpringReaction:F6} kN/m");

                sb.AppendLine(
                    $"Empuje activo total = " +
                    $"{analyticalActiveForce:F6} kN/m");

                sb.AppendLine(
                    $"Diferencia = {reactionDifference:E6}");

                sb.AppendLine();


                // ====================================================
                // ESFUERZOS EN LOS ELEMENTOS
                // ====================================================

                sb.AppendLine("ESFUERZOS EN ELEMENTOS");
                sb.AppendLine("=======================");

                sb.AppendLine();

                sb.AppendLine(
                    "Elem |   zi |   zj |" +
                    "        Qi |" +
                    "        Mi |" +
                    "        Qj |" +
                    "        Mj");

                sb.AppendLine(
                    "---------------------------------------------------------------");


                foreach (BeamElementResult result
                         in elementResults)
                {
                    sb.AppendLine(
                        $"{result.ElementId,4} | " +
                        $"{result.Zi,4:F1} | " +
                        $"{result.Zj,4:F1} | " +
                        $"{result.ShearI,9:F3} | " +
                        $"{result.MomentI,9:F3} | " +
                        $"{result.ShearJ,9:F3} | " +
                        $"{result.MomentJ,9:F3}");
                }



                // ============================================================
                // DESPLAZAMIENTOS DE MOVILIZACIÓN POR CARA
                // ============================================================

                sb.AppendLine();
                sb.AppendLine("DESPLAZAMIENTOS DE MOVILIZACIÓN POR CARA");
                sb.AppendLine("=========================================");

                sb.AppendLine(
                    "Nodo | z [m] | " +
                    "lambda_a [mm] | " +
                    "lambda_p [mm]");

                sb.AppendLine(
                    "---------------------------------------------");


                foreach (WinklerSpring spring in model.Springs)
                {
                    double z =
                        spring.Node.Z;

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


                    double sideKh =
                        kh / 2.0;


                    double lambdaA =
                        (p0 - pa) /
                        sideKh;


                    double lambdaP =
                        (pp - p0) /
                        sideKh;


                    double lambdaAmm =
                        lambdaA *
                        1000.0;


                    double lambdaPmm =
                        lambdaP *
                        1000.0;


                    sb.AppendLine(
                        $"{spring.Node.Id,4} | " +
                        $"{z,5:F2} | " +
                        $"{lambdaAmm,13:F4} | " +
                        $"{lambdaPmm,13:F4}");
                }


                // ============================================================
                // ANÁLISIS WINKLER ELASTOPLÁSTICO
                // ============================================================

                sb.AppendLine();
                sb.AppendLine("ANÁLISIS WINKLER ELASTOPLÁSTICO.");
                sb.AppendLine("================================");


                foreach (WinklerSpring spring in model.Springs)
                {
                    double z =
                        spring.Node.Z;


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


                    ElastoPlasticSpring epSpring =
                        new ElastoPlasticSpring(
                            spring.Node,
                            kh,
                            spring.TributaryLength,
                            wall.Width,
                            p0,
                            pa,
                            pp);


                    double lambdaAmm =
                        epSpring.ActiveDisplacement *
                        1000.0;


                    double lambdaPmm =
                        epSpring.PassiveDisplacement *
                        1000.0;


                    sb.AppendLine(
                        $"{spring.Node.Id,4} | " +
                        $"{z,5:F2} | " +
                        $"{lambdaAmm,13:F4} | " +
                        $"{lambdaPmm,13:F4}");
                }


                sb.AppendLine();
                sb.AppendLine("ANÁLISIS WINKLER ELASTOPLÁSTICO");
                sb.AppendLine("================================");

                sb.AppendLine(
                    "Convergencia: " +
                    (nonlinearResult.Converged
                        ? "SÍ"
                        : "NO"));

                sb.AppendLine(
                    $"Iteraciones: {nonlinearResult.Iterations}");

                sb.AppendLine(
                    $"Residuo máximo: " +
                    $"{nonlinearResult.MaximumResidual:E6}");

                sb.AppendLine();

                sb.AppendLine("DESPLAZAMIENTOS ELASTOPLÁSTICOS");
                sb.AppendLine("--------------------------------");

                sb.AppendLine(
                    "Nodo | z [m] | u lineal [mm] | u E-P [mm]");

                sb.AppendLine(
                    "------------------------------------------------");


                foreach (Node node in model.Nodes)
                {
                    double uLinear =
                        globalU[
                            node.HorizontalDof] *
                        1000.0;


                    double uNonlinear =
                        nonlinearResult.Displacements[
                            node.HorizontalDof] *
                        1000.0;


                    sb.AppendLine(
                        $"{node.Id,4} | " +
                        $"{node.Z,5:F2} | " +
                        $"{uLinear,13:F4} | " +
                        $"{uNonlinear,10:F4}");
                }


                sb.AppendLine();

              

                sb.AppendLine();
                sb.AppendLine("ESTADO DE LOS RESORTES DE DOS CARAS");
                sb.AppendLine("====================================");

                sb.AppendLine(
                    "Nodo | z [m] | u [mm] | " +
                    "Trasdos | pT [kPa] | " +
                    "Intrados | pI [kPa] | " +
                    "q neta [kPa]");

                sb.AppendLine(
                    "--------------------------------------------------------------");


                foreach (TwoSidedElastoPlasticSpring spring
                         in nonlinearResult.Springs)
                {
                    double displacement =
                        nonlinearResult.Displacements[
                            spring.Node.HorizontalDof] *
                        1000.0;


                    sb.AppendLine(
                        $"{spring.Node.Id,4} | " +
                        $"{spring.Node.Z,5:F2} | " +
                        $"{displacement,8:F4} | " +
                        $"{spring.BackState,-7} | " +
                        $"{spring.BackPressure,8:F3} | " +
                        $"{spring.FrontState,-7} | " +
                        $"{spring.FrontPressure,8:F3} | " +
                        $"{spring.NetPressure,10:F3}");
                }


                // ====================================================
                // MOSTRAR RESULTADOS
                // ====================================================

                txtSalida.Text =
                    sb.ToString();


                ResultsForm resultsForm =
    new ResultsForm(
        model,
        globalU,
        elementResults);

                resultsForm.Show();

            }




            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );



            }



        }


    }
}

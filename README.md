# WINKLERWALL

WINKLERWALL es una aplicación de escritorio desarrollada en C# para el análisis de pantallas en voladizo mediante el modelo de coeficiente de balasto de Winkler.

El programa ha sido desarrollado como parte de un Trabajo Fin de Máster relacionado con el análisis comparativo del comportamiento de muros pantalla mediante modelos de muelles y métodos de elementos finitos.

## Descarga

La versión instalable de WINKLERWALL está disponible en la sección **Releases** del repositorio.

Descargue el archivo ZIP de la versión más reciente, extraiga su contenido y ejecute:

`setup.exe`

## Funcionalidades principales

- Definición de la geometría de la pantalla.
- Definición de la profundidad de excavación.
- Introducción de propiedades estructurales:
  - espesor de la pantalla;
  - módulo de elasticidad;
  - ancho de cálculo.
- Introducción de parámetros geotécnicos:
  - peso unitario del suelo;
  - ángulo de fricción interna;
  - módulo de balasto horizontal KH.
- Discretización automática de la pantalla mediante elementos de viga.
- Generación automática de nodos en posiciones singulares, como el fondo de excavación.
- Representación del terreno mediante resortes de Winkler.
- Ensamblaje de la matriz global de rigidez.
- Cálculo del vector global de cargas.
- Resolución del sistema:

  K · U = F

- Obtención de:
  - desplazamientos horizontales;
  - giros;
  - fuerzas cortantes;
  - momentos flectores;
  - reacciones del terreno.
- Representación gráfica de:
  - pantalla;
  - nodos;
  - resortes;
  - fondo de excavación;
  - deformada;
  - diagramas de desplazamiento;
  - momentos;
  - cortantes;
  - reacciones.
- Implementación experimental de una ley elastoplástica de interacción suelo-pantalla con estados activo, pasivo y en reposo.
- Resolución incremental no lineal mediante actualización de la rigidez tangente.

## Modelo de cálculo

La pantalla se representa mediante elementos de viga Euler-Bernoulli con dos grados de libertad por nodo:

- desplazamiento horizontal;
- giro.

La interacción terreno-estructura se representa mediante resortes horizontales cuya rigidez nodal se obtiene a partir de:

k = KH · b · Lt

donde:

- KH es el módulo de balasto horizontal;
- b es el ancho de cálculo;
- Lt es la longitud tributaria correspondiente al nodo.

Para el análisis elastoplástico se consideran estados de presión asociados a:

- K0: coeficiente de empuje al reposo;
- Ka: coeficiente de empuje activo;
- Kp: coeficiente de empuje pasivo.

La versión actual constituye una herramienta de carácter académico y de investigación.

## Requisitos

- Windows
- .NET Framework 4.8
- Visual Studio 2022 o compatible
- MathNet.Numerics

## Ejecución desde Visual Studio

1. Clonar o descargar el repositorio.
2. Abrir la solución `WinklerWall.sln`.
3. Restaurar los paquetes NuGet.
4. Compilar la solución.
5. Ejecutar el proyecto `WinklerWall`.

## Código fuente

El código se encuentra organizado en diferentes módulos:

```text
WinklerWall
├── Models
│   ├── Node.cs
│   ├── Wall.cs
│   └── AnalysisModel.cs
│
├── Elements
│   ├── BeamElement.cs
│   ├── WinklerSpring.cs
│   ├── ElastoPlasticSpring.cs
│   └── TwoSidedElastoPlasticSpring.cs
│
├── Solver
│   ├── MatrixAssembler.cs
│   ├── LoadVectorAssembler.cs
│   ├── MatrixDiagnostics.cs
│   ├── LinearSolver.cs
│   ├── NonlinearSolver.cs
│   ├── EarthPressureCalculator.cs
│   └── ElementForceRecovery.cs
│
├── Results
│   ├── BeamElementResult.cs
│   └── NonlinearAnalysisResult.cs
│
└── Forms
    ├── Form1.cs
    └── ResultsForm.cs

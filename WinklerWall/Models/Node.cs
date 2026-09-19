namespace WinklerWall.Models
{
    /// <summary>
    /// Representa un nodo de la discretización de la pantalla.
    /// 
    /// Cada nodo posee dos grados de libertad:
    /// 1. Desplazamiento horizontal u
    /// 2. Rotación theta
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Número identificador del nodo.
        /// Comienza en cero.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Profundidad del nodo medida desde
        /// la coronación de la pantalla [m].
        /// </summary>
        public double Z { get; private set; }

        /// <summary>
        /// Grado de libertad correspondiente
        /// al desplazamiento horizontal.
        /// </summary>
        public int HorizontalDof
        {
            get
            {
                return 2 * Id;
            }
        }

        /// <summary>
        /// Grado de libertad correspondiente
        /// a la rotación.
        /// </summary>
        public int RotationDof
        {
            get
            {
                return 2 * Id + 1;
            }
        }

        /// <summary>
        /// Constructor del nodo.
        /// </summary>
        public Node(int id, double z)
        {
            Id = id;
            Z = z;
        }
    }
}
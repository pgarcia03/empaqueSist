using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEmpaqueRocedes.VentanasSec
{
    public class Class1
    {
        public string nombre { get; set; }
        public Image img { get; set; }

        public Class1(string nombre, Image img)
        {
            this.nombre = nombre;
            this.img = img;
        }

    }
}

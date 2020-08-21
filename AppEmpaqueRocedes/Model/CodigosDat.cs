using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEmpaqueRocedes.Model
{
    public class CodigosDat
    {
        public int Id_Order { get; set; }
        public Nullable<int> Id_Cliente { get; set; }
        public Nullable<int> Id_Style { get; set; }
        public string POrder { get; set; }
        public int Id_Bundle { get; set; }
        public Nullable<int> NSeq { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public Nullable<int> Quantity { get; set; }
        public  string codigoBarra { get; set; }
        public string POrderGuiones{ get; set; }
        public string Estado { get; set; }
    }
}

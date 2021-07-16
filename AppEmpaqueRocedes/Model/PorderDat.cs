using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEmpaqueRocedes.Model
{
    public class PorderDat
    {
        public int? Id_Order { get; set; }
        public Nullable<int> Id_Cliente { get; set; }
        public Nullable<int> Id_Style { get; set; }
        public string POrder { get; set; }
        public string style { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> Bundles { get; set; }
        public Nullable<int> Id_Planta { get; set; }
        public Nullable<int> Id_Linea { get; set; }
        public Nullable<int> Semana { get; set; }
        public string Comments { get; set; }
        public Nullable<int> Id_Linea2 { get; set; }
        public string Describir { get; set; }
        public Nullable<int> AfterIntex { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppEmpaqueRocedes.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbProduccionTickets
    {
        public int id { get; set; }
        public string serial { get; set; }
        public string codigoOperacion { get; set; }
        public Nullable<int> operarioId { get; set; }
        public string descOperacion { get; set; }
        public string corte { get; set; }
        public Nullable<int> nSeq { get; set; }
        public string linea { get; set; }
        public Nullable<System.DateTime> fechaRegistro { get; set; }
        public string usuario { get; set; }
        public Nullable<int> BundleUni { get; set; }
        public Nullable<int> bihorario { get; set; }
        public Nullable<int> lineaConfeccion { get; set; }
    }
}

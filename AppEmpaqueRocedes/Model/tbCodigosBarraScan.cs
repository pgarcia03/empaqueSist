//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppEmpaqueRocedes.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbCodigosBarraScan
    {
        public int id { get; set; }
        public string codigoBox { get; set; }
        public string codigoBarra { get; set; }
        public Nullable<System.DateTime> fechaEscaneado { get; set; }
        public Nullable<bool> estado { get; set; }
        public string usuario { get; set; }
        public Nullable<System.DateTime> fechaAnulado { get; set; }
        public string usuarioAnulado { get; set; }
    
        public virtual tbcodigosCajas tbcodigosCajas { get; set; }
    }
}

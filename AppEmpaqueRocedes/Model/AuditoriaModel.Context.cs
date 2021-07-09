﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class AuditoriaEntities : DbContext
    {
        public AuditoriaEntities()
            : base("name=AuditoriaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Biohorario> Biohorario { get; set; }
        public virtual DbSet<Bundle> Bundle { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Linea> Linea { get; set; }
        public virtual DbSet<Operacion> Operacion { get; set; }
        public virtual DbSet<Operario> Operario { get; set; }
        public virtual DbSet<POrder> POrder { get; set; }
        public virtual DbSet<Style> Style { get; set; }
        public virtual DbSet<tbBultosCodigosBarra> tbBultosCodigosBarra { get; set; }
        public virtual DbSet<tbCodigosBarraScan> tbCodigosBarraScan { get; set; }
        public virtual DbSet<tbcodigosCajas> tbcodigosCajas { get; set; }
        public virtual DbSet<tbCorteSecuenciaCaja> tbCorteSecuenciaCaja { get; set; }
        public virtual DbSet<tblUserEnvio> tblUserEnvio { get; set; }
        public virtual DbSet<tbPorderSinGuion> tbPorderSinGuion { get; set; }
        public virtual DbSet<tbProduccionTickets> tbProduccionTickets { get; set; }
        public virtual DbSet<tbUserEmpaque> tbUserEmpaque { get; set; }
        public virtual DbSet<Planta> Planta { get; set; }
    
        public virtual ObjectResult<Nullable<System.DateTime>> ExtraefechaServidor(Nullable<int> val)
        {
            var valParameter = val.HasValue ?
                new ObjectParameter("val", val) :
                new ObjectParameter("val", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<System.DateTime>>("ExtraefechaServidor", valParameter);
        }
    
        public virtual ObjectResult<getcorteBox_Result> getcorteBox(string codigobox, string usuario, string clasificacion)
        {
            var codigoboxParameter = codigobox != null ?
                new ObjectParameter("codigobox", codigobox) :
                new ObjectParameter("codigobox", typeof(string));
    
            var usuarioParameter = usuario != null ?
                new ObjectParameter("usuario", usuario) :
                new ObjectParameter("usuario", typeof(string));
    
            var clasificacionParameter = clasificacion != null ?
                new ObjectParameter("clasificacion", clasificacion) :
                new ObjectParameter("clasificacion", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getcorteBox_Result>("getcorteBox", codigoboxParameter, usuarioParameter, clasificacionParameter);
        }
    
        public virtual ObjectResult<getInfobox_Result> getInfobox(string box)
        {
            var boxParameter = box != null ?
                new ObjectParameter("box", box) :
                new ObjectParameter("box", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getInfobox_Result>("getInfobox", boxParameter);
        }
    
        public virtual ObjectResult<spdExtraeTallasXCortecompleto_Result> spdExtraeTallasXCortecompleto(Nullable<int> idorder, string porder, Nullable<int> idestilo, string estilo, string usario)
        {
            var idorderParameter = idorder.HasValue ?
                new ObjectParameter("idorder", idorder) :
                new ObjectParameter("idorder", typeof(int));
    
            var porderParameter = porder != null ?
                new ObjectParameter("porder", porder) :
                new ObjectParameter("porder", typeof(string));
    
            var idestiloParameter = idestilo.HasValue ?
                new ObjectParameter("idestilo", idestilo) :
                new ObjectParameter("idestilo", typeof(int));
    
            var estiloParameter = estilo != null ?
                new ObjectParameter("estilo", estilo) :
                new ObjectParameter("estilo", typeof(string));
    
            var usarioParameter = usario != null ?
                new ObjectParameter("usario", usario) :
                new ObjectParameter("usario", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spdExtraeTallasXCortecompleto_Result>("spdExtraeTallasXCortecompleto", idorderParameter, porderParameter, idestiloParameter, estiloParameter, usarioParameter);
        }
    
        public virtual ObjectResult<spdProduccionOperarioDiario_Result> spdProduccionOperarioDiario(Nullable<int> idoper)
        {
            var idoperParameter = idoper.HasValue ?
                new ObjectParameter("idoper", idoper) :
                new ObjectParameter("idoper", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spdProduccionOperarioDiario_Result>("spdProduccionOperarioDiario", idoperParameter);
        }
    
        public virtual ObjectResult<spdBuscarPoCliente_Result> spdBuscarPoCliente(string porder)
        {
            var porderParameter = porder != null ?
                new ObjectParameter("porder", porder) :
                new ObjectParameter("porder", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spdBuscarPoCliente_Result>("spdBuscarPoCliente", porderParameter);
        }
    }
}

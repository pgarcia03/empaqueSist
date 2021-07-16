using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppEmpaqueRocedes.Model;

namespace AppEmpaqueRocedes.Logica
{
    public class PorderNeg
    {

        //AuditoriaEntities db = new AuditoriaEntities();

        //public void Dispose()
        //{
        //    ((IDisposable)db).Dispose();
        //}

        public static List<PorderDat> GetPordersClientAutocompletado(string pre)
        {

            using (var contex = new AuditoriaEntities())
                return contex.spdBuscarPoCliente(pre).Select(x=>new PorderDat { POrder=x.Porder,style=x.Style,Quantity=x.Quantity }).ToList();

        }

        public static List<PorderDat> GetPordersAutocompletado(string pre)
        {

            using (var contex = new AuditoriaEntities())
                return contex.POrder.Join(contex.Style, x => x.Id_Style, y => y.Id_Style, (x, y) => new { x.Id_Order, x.POrder1, x.Quantity, x.Id_Style, y.Style1 })
                                    .Where(x => x.POrder1.Contains(pre))
                                    .Take(20)
                                    .Select(x => new PorderDat { Id_Order = x.Id_Order, POrder = x.POrder1, Id_Style = x.Id_Style, Quantity = x.Quantity, style = x.Style1 })
                                    .ToList();

        }

        public static List<PorderDat> GetPordersGeneradosAutocompletado(string pre)
        {

            using (var contex = new AuditoriaEntities())
                return contex.tbPorderSinGuion.Where(x => x.corte.Contains(pre))
                                    .Take(20)
                                    .Select(x => new PorderDat { Id_Order = x.idCorte, POrder = x.corte.Trim(), Id_Style = x.idEstilo, Quantity = x.unidades, style = x.estilo })
                                    .ToList();

        }

        public static List<PorderDat> GetPordersClienteGeneradosAutocompletado(string pre)
        {

            using (var contex = new AuditoriaEntities())
                return contex.tbPorderSinGuion.Where(x => x.corte.Contains(pre))
                                    .Take(20)
                                    .Select(x => new PorderDat { POrder = x.corte.Trim(), Quantity = x.unidades, style = x.estilo })
                                    .ToList();

        }

        public static List<Totales> Totales(int idcorte)
        {
            using (var contex = new AuditoriaEntities())
            {
                var l = contex.tbcodigosCajas.Join(contex.tbCodigosBarraScan, x => x.codigoBox, y => y.codigoBox, (x, y) => new {x.idCorte, x.codigoBox, y.codigoBarra,x.numeroImpresion })
                                             .Where(x=>x.idCorte==idcorte )  
                                             .GroupBy(x=>new { x.codigoBox,x.numeroImpresion })
                                             .Select(x => new Totales
                                             {
                                               box=x.Key.codigoBox,
                                               impreso=x.Key.numeroImpresion==null ? (int)x.Key.numeroImpresion:0,
                                               unidades= x.Select(z=>z.codigoBarra).Count() //.codigoBox
                                             })
                                             .ToList();


                return l;
            }


        }

        public static List<Totales> Totales(string corte)
        {
            using (var contex = new AuditoriaEntities())
            {
                var l = contex.tbcodigosCajas.Join(contex.tbCodigosBarraScan, x => x.codigoBox, y => y.codigoBox, (x, y) => new { x.corteCompleto, x.codigoBox, y.codigoBarra, x.numeroImpresion })
                                             .Where(x => x.corteCompleto.Equals(corte))
                                             .GroupBy(x => new { x.codigoBox, x.numeroImpresion })
                                             .Select(x => new Totales
                                             {
                                                 box = x.Key.codigoBox,
                                                 impreso = x.Key.numeroImpresion == null ? 0:(int)x.Key.numeroImpresion,
                                                 unidades = x.Select(z => z.codigoBarra).Count() //.codigoBox
                                             })
                                             .ToList();


                return l;
            }


        }


        public static List<Totales> TotalesLeft(int idcorte)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    var l = contex.tbcodigosCajas.Where(x => x.idCorte == idcorte)
                                                 .GroupJoin(contex.tbCodigosBarraScan, x => x.codigoBox, y => y.codigoBox, (x, y) => new { x, y })
                                                 .SelectMany(x => x.y.DefaultIfEmpty(), (a, b) => new
                                                 {
                                                     box = a.x.codigoBox, 
                                                     unidades = b.codigoBarra
                                                 })
                                                 .GroupBy(x => new
                                                 {
                                                     x.box
                                                 })
                                                 .Select(x => new Totales {
                                                     box = x.Key.box,
                                                     unidades = x.Sum(z=>z.unidades==null?0:1)
                                                 }).
                                                 ToList();


                    return l;
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;

                return null;
               // throw;
            }
          

        }

        public static List<Totales> TotalesLeft(string corte)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    var l = contex.tbcodigosCajas.Where(x => x.corteCompleto.Equals(corte))
                                                 .GroupJoin(contex.tbCodigosBarraScan, x => x.codigoBox, y => y.codigoBox, (x, y) => new { x, y })
                                                 .SelectMany(x => x.y.DefaultIfEmpty(), (a, b) => new
                                                 {
                                                     box = a.x.codigoBox,
                                                     unidades = b.codigoBarra
                                                 })
                                                 .GroupBy(x => new
                                                 {
                                                     x.box
                                                 })
                                                 .Select(x => new Totales
                                                 {
                                                     box = x.Key.box,
                                                     unidades = x.Sum(z => z.unidades == null ? 0 : 1)
                                                 }).
                                                 ToList();


                    return l;
                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;

                return null;
                // throw;
            }


        }

    }
}

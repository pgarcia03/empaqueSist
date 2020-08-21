﻿using System;
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

        public static List<PorderDat> GetPordersAutocompletado(string pre)
        {

            using (var contex = new AuditoriaEntities())
                return contex.POrder.Join(contex.Style, x => x.Id_Style, y => y.Id_Style, (x, y) => new { x.Id_Order, x.POrder1, x.Quantity, x.Id_Style, y.Style1 }).Where(x => x.POrder1.Contains(pre)).Take(20).Select(x => new PorderDat { Id_Order = x.Id_Order, POrder = x.POrder1, Id_Style = x.Id_Style, Quantity = x.Quantity, style = x.Style1 }).ToList();
        }

        public static List<Totales> Totales(int idcorte)
        {
            using (var contex = new AuditoriaEntities())
            {
                var l = contex.tbcodigosCajas.Join(contex.tbCodigosBarraScan, x => x.codigoBox, y => y.codigoBox, (x, y) => new {x.idCorte, x.codigoBox, y.codigoBarra })
                                             .Where(x=>x.idCorte==idcorte)  
                                             .GroupBy(x=>new { x.codigoBox })
                                             .Select(x => new Totales
                                             {
                                               box=x.Key.codigoBox,
                                               unidades= x.Select(z=>z.codigoBarra).Count() //.codigoBox
                                             }).ToList();


                return l;
            }


        }

    }
}

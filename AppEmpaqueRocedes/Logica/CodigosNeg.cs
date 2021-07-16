using AppEmpaqueRocedes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEmpaqueRocedes.Logica
{
    public class CodigosNeg
    {

        public async static Task<List<CodigosDat>> GetCodigosDats(int idorder)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {

                    var tarea1 = Task.Run(() =>
                    {
                        using (var contex1 = new AuditoriaEntities())
                        {
                            var l1 = contex1.POrder.Join(contex1.Bundle, x => x.Id_Order, y => y.Id_Order, (x, y) => new { x, y })
                                               .Where(x => x.x.Id_Order.Equals(idorder))
                                               .GroupBy(x => new
                                               {
                                                   x.x.Id_Order,
                                                   x.x.POrder1,
                                                   x.y.Size
                                               }).Select(x => new CodigosDat
                                               {
                                                   Id_Order = x.Key.Id_Order,
                                                   POrder = x.Key.POrder1,
                                                   Size = x.Key.Size,
                                                   Quantity = x.Sum(z => z.y.Quantity),
                                                   Estado = "Generado"

                                               }).ToList();


                            return l1;
                        }

                    });


                    var tarea2 = Task.Run(() =>
                    {
                        using (var contex1 = new AuditoriaEntities())
                        {
                            var l2 = contex1.POrder.Join(contex1.tbBultosCodigosBarra, x => x.Id_Order, y => y.idCorte, (x, y) => new { x, y })
                                                 .Where(x => x.x.Id_Order == idorder)
                                                 .Select(
                                                  x => new CodigosDat
                                                  {
                                                      Id_Order = x.x.Id_Order,
                                                      //      POrderGuiones = x.x.POrder1,
                                                      Size = x.y.talla,
                                                      POrder = x.x.POrder1,
                                                      NSeq = x.y.secuenciaUnidades,
                                                      codigoBarra = x.y.codigoBarra,
                                                      Estado = x.y.estado
                                                  }).OrderBy(x => x.NSeq).ToList();

                            return l2;
                        }
                    });


                    await Task.WhenAll(tarea1, tarea2).ConfigureAwait(false);


                    var list = tarea1.Result;
                    var listas = tarea2.Result;



                    var listaTotal = new List<CodigosDat>();
                    int incrementable = 1;


                    foreach (var item in listas.Count == 0 ? list : listas)
                    {

                        var talla = item.Size.Replace('*', 'X');
                        for (int i = 0; i < item.Quantity; i++)
                        {

                            listaTotal.Add(new CodigosDat()
                            {
                                POrder = item.POrder,
                                Id_Order = item.Id_Order,
                                Size = talla,
                                NSeq = incrementable,
                                codigoBarra = string.Concat(item.Id_Order.ToString(), incrementable.ToString(), talla),
                                Estado = item.Estado
                            });

                            incrementable++;
                        }

                    }

                    return listas.Count == 0 ? listaTotal.OrderBy(x => x.NSeq).ToList() : listas;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async static Task<List<CodigosDat>> GetCodigosDatsXtalla(int idorder)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {

                    var tarea1 = Task.Run(() =>
                    {
                        using (var contex1 = new AuditoriaEntities())
                        {
                            var l1 = contex1.POrder.Join(contex1.Bundle, x => x.Id_Order, y => y.Id_Order, (x, y) => new { x, y })
                                               .Where(x => x.x.Id_Order.Equals(idorder))
                                               .GroupBy(x => new
                                               {
                                                   x.x.Id_Order,
                                                   x.x.POrder1,
                                                   x.y.Size
                                               }).Select(x => new CodigosDat
                                               {
                                                   Id_Order = x.Key.Id_Order,
                                                   POrder = x.Key.POrder1,
                                                   Size = x.Key.Size,
                                                   Quantity = x.Sum(z => z.y.Quantity),
                                                   Estado = "Generado",
                                               }).ToList();


                            return l1;
                        }

                    });


                    var tarea2 = Task.Run(() =>
                    {
                        using (var contex1 = new AuditoriaEntities())
                        {
                            var l2 = contex1.POrder.Join(contex1.tbBultosCodigosBarra, x => x.Id_Order, y => y.idCorte, (x, y) => new { x, y })
                                                 .Where(x => x.x.Id_Order == idorder)
                                                 .Select(
                                                  x => new CodigosDat
                                                  {
                                                      Id_Order = x.x.Id_Order,
                                                      POrder = x.x.POrder1,
                                                      Size = x.y.talla,
                                                      NSeq = x.y.secuenciaUnidades,
                                                      Cantidad = x.y.Cantidad,
                                                      Resto = x.y.Restante,
                                                      codigoBarra = x.y.codigoBarra,
                                                      Estado = x.y.estado
                                                  }).OrderBy(x => x.NSeq).ToList();

                            return l2;
                        }
                    });


                    await Task.WhenAll(tarea1, tarea2).ConfigureAwait(false);


                    var list = tarea1.Result;
                    var listas = tarea2.Result;



                    var listaTotal = new List<CodigosDat>();
                    int incrementable = 1;


                    foreach (var item in list)
                    {

                        listaTotal.Add(new CodigosDat()
                        {
                            Id_Order = item.Id_Order,
                            POrder = item.POrder,
                            Size = item.Size.Replace('*', 'X'),
                            NSeq = incrementable,
                            Cantidad = item.Quantity,
                            Resto = item.Quantity,
                            codigoBarra = string.Concat(item.Id_Order.ToString(), incrementable.ToString(), item.Size.Replace('*', 'X')),
                            Estado = item.Estado
                        });

                        incrementable++;

                    }

                    return listas.Count == 0 ? listaTotal.OrderBy(x => x.NSeq).ToList() : listas;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async static Task<List<CodigosDat>> GetCodigosDatsXtalla(int idorder, string porder,int? idestilo, string estilo,string usuario)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {

                    var tarea1 = Task.Run(() =>
                    {
                        using (var contex1 = new AuditoriaEntities())
                        {
                            var l1 = contex1.spdExtraeTallasXCortecompleto(idorder, porder ,idestilo,estilo,usuario)
                                            .Select(x => new CodigosDat
                                            {
                                                   Id_Order = 0,//idorder,
                                                   POrder = porder,
                                                   Size = x.Size,
                                                   Quantity = x.Cantidad,
                                                   Estado = "Generado",
                                            }).ToList();


                            return l1;
                        }

                    });


                    var tarea2 = Task.Run(() =>
                    {
                        using (var contex1 = new AuditoriaEntities())
                        {
                            var l2 = contex1.POrder.Join(contex1.tbBultosCodigosBarra, x => x.Id_Order, y => y.idCorte, (x, y) => new { x, y })
                                                 .Where(x => x.x.Id_Order == idorder)
                                                 .Select(
                                                  x => new CodigosDat
                                                  {
                                                      Id_Order = x.x.Id_Order,
                                                      POrder = x.x.POrder1,
                                                      Size = x.y.talla,
                                                      NSeq = x.y.secuenciaUnidades,
                                                      Cantidad = x.y.Cantidad,
                                                      Resto = x.y.Restante,
                                                      codigoBarra = x.y.codigoBarra,
                                                      Estado = x.y.estado
                                                  }).OrderBy(x => x.NSeq).ToList();

                            return l2;
                        }
                    });


                    await Task.WhenAll(tarea1, tarea2).ConfigureAwait(false);

                    var list = tarea1.Result;
                    var listas = tarea2.Result;


                    var listaTotal = new List<CodigosDat>();
                    int incrementable = 1;


                    foreach (var item in list)
                    {

                        listaTotal.Add(new CodigosDat()
                        {
                            Id_Order = item.Id_Order,
                            POrder = item.POrder,
                            Size = item.Size.Replace('*', 'X'),
                            NSeq = incrementable,
                            Cantidad = item.Quantity,
                            Resto = item.Quantity,
                            codigoBarra = string.Concat(item.Id_Order.ToString(), incrementable.ToString(), item.Size.Replace('*', 'X')),
                            Estado = item.Estado
                        });

                        incrementable++;

                    }

                    return listas.Count == 0 ? listaTotal.OrderBy(x => x.NSeq).ToList() : listas;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async static Task<List<CodigosDat>> GetCodigosDatsXtalla(string porder, string estilo, string usuario)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {


                    var tarea1 = await Task.Run(() =>
                    {
                        using (var contex1 = new AuditoriaEntities())
                        {
                            var l1 = contex1.spdExtraeTallasXCortecompleto(0, porder, 0, estilo, usuario)
                                            .Select(x => new CodigosDat
                                            {
                                                Id_Order = 0,//idorder,
                                                POrder = porder,
                                                Size = x.Size,
                                                Quantity = x.Cantidad,
                                                Estado = "Generado",
                                            }).ToList();


                            return l1;
                        }

                    }).ConfigureAwait(false);

                    
                    var listaTotal = new List<CodigosDat>();
                    int incrementable = 1;


                    foreach (var item in tarea1)
                    {

                        listaTotal.Add(new CodigosDat()
                        {
                           // Id_Order = item.Id_Order,
                            POrder = item.POrder,
                            Size = item.Size.Replace('*', 'X'),
                            NSeq = incrementable,
                            Cantidad = item.Quantity,
                            Resto = item.Quantity,
                            codigoBarra =string.Concat(item.POrder.ToString().Trim(),string.Format("{0:000000}", incrementable)),
                            Estado = item.Estado
                        });

                        incrementable++;

                    }

                    return listaTotal.OrderBy(x => x.NSeq).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static List<object> GuardarCodigos(List<CodigosDat> listaGuardar, string corte)
        {
            List<object> listObject = new List<object>();

            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    using (var transaction = contex.Database.BeginTransaction())
                    {
                        try
                        {
                            var listaguardada = new List<CodigosDat>();
                            var fecha = contex.ExtraefechaServidor(1).ToArray();
                            var fe = Convert.ToDateTime(fecha[0]);

                            var registroCodigosBarra = new List<tbBultosCodigosBarra>();

                            foreach (var item in listaGuardar)
                            {
                                var cont = contex.tbBultosCodigosBarra.Where(x => x.corteCompleto.Contains(corte.Trim()) && x.secuenciaUnidades == item.NSeq).Count();

                                if (cont == 0)
                                {
                                    contex.tbBultosCodigosBarra.Add(new tbBultosCodigosBarra
                                    {
                                       // idCorte = item.Id_Order,
                                        codigoBarra = item.codigoBarra,
                                        talla = item.Size,
                                        secuenciaUnidades = item.NSeq,
                                        fechaGenerado = fe,
                                        estado = "Generado",
                                        corteCompleto = corte,
                                        Cantidad = item.Cantidad,
                                        Restante = item.Resto

                                    });

                                    //item.POrder = corte;//opcional
                                    listaguardada.Add(item);
                                }
                                
                            }

                            contex.SaveChanges();
                            transaction.Commit();

                            listObject.Add(listaguardada);
                            listObject.Add(200);

                            return listObject;
                        }
                        catch (Exception ex)
                        {
                            string mess = ex.Message;
                            transaction.Rollback();
                            listObject.Add(ex.Message);
                            listObject.Add(500);
                            return listObject;
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                listObject.Add(ex.Message);
                listObject.Add(500);

                return listObject;
            }
        }

        public async static Task<int> EscaneoCodigo(string Codigobox, string codigoBarra, string usuario)
        {
            // var list = new List<object>();
            try
            {

                using (var contex = new AuditoriaEntities())
                {

                    var fecha = contex.ExtraefechaServidor(1).ToArray();
                    var fechaActual = fecha[0];
                    var obj = contex.tbBultosCodigosBarra.Find(codigoBarra);

                    obj.fechaEscaneado = fechaActual;
                    obj.estado = "Escaneado";
                    obj.Restante--;


                    // contex.SaveChanges();

                    var objscan = new tbCodigosBarraScan
                    {
                        codigoBox = Codigobox,
                        codigoBarra = codigoBarra,
                        fechaEscaneado = fechaActual,
                        usuario = usuario,
                        estado = true
                    };

                    contex.tbCodigosBarraScan.Add(objscan);
                    await contex.SaveChangesAsync();

                    return 1;

                }

                //return list;

            }
            catch (Exception ex)
            {
                // list.Add(0);
                string mess = ex.Message;
                return 0;
            }
        }

        public async static Task<tbCorteSecuenciaCaja> crearCaja(int idcorte, string estilo, string porder,string usuario)
        {
            try
            {
                var tarea1 = Task.Run(() =>
                {
                    using (var contex1 = new AuditoriaEntities())
                    {
                        var resp = contex1.tbCorteSecuenciaCaja.Where(x => x.idcorte == idcorte).Count();

                        return resp;
                    }

                });

                var tarea2 = Task.Run(() =>
                {
                    using (var contex1 = new AuditoriaEntities())
                    {
                        var resp = contex1.ExtraefechaServidor(1).ToArray();

                        return resp;
                    }

                });

                var tarea3 = Task.Run(() =>
                {
                    using (var contex1 = new AuditoriaEntities())
                    {
                        var resp = contex1.POrder
                                          .Join(contex1.Bundle, x => x.Id_Order, y => y.Id_Order, (x, y) => new { x.Id_Order, y.Color, x.POrder1 })
                                          .Where(x => x.Id_Order == idcorte)
                                          .Select(x => new { x.Color, Porder = x.POrder1 }).Take(1).ToArray();

                        return resp[0];
                    }

                });


                await Task.WhenAll(tarea1, tarea2, tarea3);

                using (var contex = new AuditoriaEntities())
                {

                    var cont = tarea1.Result;

                    var sec = cont == 0 ? 1 : cont + 1;

                    var codigo = string.Concat("999", idcorte, sec);


                    var obj = new tbCorteSecuenciaCaja
                    {
                        codigo = codigo,
                        idcorte = idcorte,
                        sec = sec
                    };

                    contex.tbCorteSecuenciaCaja.Add(obj);
                    contex.SaveChanges();

                    var fecha = tarea2.Result;


                    var codigoNew = new tbcodigosCajas
                    {
                        codigoBox = codigo,
                        idCorte = idcorte,
                        estilo = estilo.TrimEnd(),
                        secBoxXCorte = sec,
                        fechaGenerado = fecha[0],
                        estado = true,
                        numeroImpresion = 0,
                        color = tarea3.Result.Color,
                        corteCompleto = porder, 
                        usuario = usuario
                    };

                    contex.tbcodigosCajas.Add(codigoNew);

                    contex.SaveChanges();

                    return obj;
                }

            }
            catch (Exception ex)
            {
                var sr = ex.Message;
               // await crearCaja(idcorte, estilo,porder, usuario);
                // throw;
                return null;
            }

        }

        public static getInfobox_Result BuscarCaja(string box)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    var obj = contex.getInfobox(box).FirstOrDefault();

                    return obj;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static getInfobox2_Result BuscarCaja2(string box)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    var obj = contex.getInfobox2(box).FirstOrDefault();

                    return obj;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static getcorteBox_Result imprimirBox(string codigobox, string usuario, string clasificacion)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    var obj = contex.getcorteBox(codigobox, usuario, clasificacion).First();

                    return obj;
                }
            }
            catch (Exception ex)
            {
                var objerror = new getcorteBox_Result();

                objerror.unidades = 0;
                objerror.corte = "corte";
                objerror.estado = "Error";

                string mes = ex.Message;


                return objerror;

            }
        }


        public static getcorteBox2_Result imprimirBox2(string codigobox, string usuario, string clasificacion)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    var obj = contex.getcorteBox2(codigobox, usuario, clasificacion).First();

                    return obj;
                }
            }
            catch (Exception ex)
            {
                var objerror = new getcorteBox2_Result();

                objerror.unidades = 0;
                objerror.corte = "corte";
                objerror.estado = "Error";

                string mes = ex.Message;


                return objerror;

            }
        }

        public async static Task<tbcodigosCajas> crearCaja(string estilo, string porder, string usuario)
        {
            try
            {
                var tarea1 = Task.Run(() =>
                {
                    using (var contex1 = new AuditoriaEntities())
                    {
                        var resp = contex1.tbCorteSecuenciaCaja.Where(x => x.corte.Equals(porder.Trim())).Count();

                        return resp;
                    }

                });

                var tarea2 = Task.Run(() =>
                {
                    using (var contex1 = new AuditoriaEntities())
                    {
                        var resp = contex1.ExtraefechaServidor(1).ToArray();

                        return resp;
                    }

                });

                var tarea3 = Task.Run(() =>
                {
                    using (var contex1 = new AuditoriaEntities())
                    {
                        var resp = contex1.POrder
                                          .Join(contex1.Bundle, x => x.Id_Order, y => y.Id_Order, (x, y) => new { x.POrderClient, y.Color, x.POrder1 })
                                          .Where(x => x.POrderClient.Equals(porder))
                                          .Select(x => new { x.Color, Porder = x.POrderClient }).Take(1).ToArray();

                        return resp[0];
                    }

                });


                await Task.WhenAll(tarea1, tarea2, tarea3);

                using (var contex = new AuditoriaEntities())
                {

                    var cont = tarea1.Result;

                    var sec = cont == 0 ? 1 : cont + 1;

                    var codigo = string.Concat("999", porder.Trim(), string.Format("{0:000}", sec));


                    var obj = new tbCorteSecuenciaCaja
                    {
                        codigo = codigo,
                        idcorte = 0,
                        sec = sec,
                        corte=porder.Trim()
                    };

                    contex.tbCorteSecuenciaCaja.Add(obj);
                    contex.SaveChanges();

                    var fecha = tarea2.Result;


                    var codigoNew = new tbcodigosCajas
                    {
                        codigoBox = codigo,
                        idCorte = 0,
                        estilo = estilo.TrimEnd(),
                        secBoxXCorte = sec,
                        fechaGenerado = fecha[0],
                        estado = true,
                        numeroImpresion = 0,
                        color = tarea3.Result.Color,
                        corteCompleto = porder,
                        usuario = usuario
                    };

                    contex.tbcodigosCajas.Add(codigoNew);

                    contex.SaveChanges();

                    return codigoNew;
                }

            }
            catch (Exception ex)
            {
                var sr = ex.Message;
                // await crearCaja(idcorte, estilo,porder, usuario);
                // throw;
                return null;
            }

        }
    }


}

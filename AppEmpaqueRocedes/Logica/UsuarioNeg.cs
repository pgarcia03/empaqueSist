using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppEmpaqueRocedes.Model;

namespace AppEmpaqueRocedes.Logica
{
    public class UsuarioNeg
    {
        internal static object SaveUsuario(tbUserEmpaque us)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {

                    var cont = contex.tbUserEmpaque.FirstOrDefault(x => x.nombreUsuario.Equals(us.nombreUsuario));

                    if (cont == null)
                    {
                        contex.tbUserEmpaque.Add(us);

                        contex.SaveChanges();

                        return 1;
                    }
                    else
                        return 2;



                }
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                return 0;
            }
        }


        internal static tbUserEmpaque GetUsuario(tbUserEmpaque us)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    var obj = contex.tbUserEmpaque.FirstOrDefault(u => u.nombreUsuario.Equals(us.nombreUsuario) && u.contraseña.Equals(us.contraseña));

                    return obj == null ? new tbUserEmpaque { nombreUsuario = "null", rol = "null" } : obj;

                }
            }
            catch (Exception ex)
            {

                return new tbUserEmpaque { rol = "error", nombreUsuario = ex.Message };
            }
        }


        internal static List<tbUserEmpaque> GetUsuario()
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {
                    var obj = contex.tbUserEmpaque.ToList();

                    return obj;

                }
            }
            catch (Exception ex)
            {
                string ee = ex.Message;
                return null;
            }
        }

        internal static int DelUsuario(tbUserEmpaque us)
        {
            try
            {
                using (var contex = new AuditoriaEntities())
                {

                    var obj = contex.tbUserEmpaque.First(x => x.nombreUsuario.Equals(us.nombreUsuario));
                    contex.tbUserEmpaque.Remove(obj);
                    contex.SaveChanges();

                    return 1;

                }
            }
            catch (Exception ex)
            {
                string ee = ex.Message;
                return 0;
            }
        }
    }
}

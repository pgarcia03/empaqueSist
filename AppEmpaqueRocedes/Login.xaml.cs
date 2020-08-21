using AppEmpaqueRocedes.Logica;
using AppEmpaqueRocedes.Model;
using AppEmpaqueRocedes.Seguridad;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppEmpaqueRocedes
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : MetroWindow
    {
        public Login()
        {
            InitializeComponent();

            txtuser.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtuser.Text) && !string.IsNullOrWhiteSpace(txtpass.Password))
                {

                    string password = HashClass.EncodePassword(string.Concat(txtuser.Text.ToLower(), txtpass.Password));

                    var obj = new tbUserEmpaque
                    {
                        nombreUsuario = txtuser.Text.ToLower(),
                        contraseña=password

                    };

                    var resp = UsuarioNeg.GetUsuario(obj);

                    switch (resp.rol.Trim())
                    {
                        case "error":
                            MessageBox.Show("Error","Ha ocurrido un error intente nuevamente");

                            break;
                        case "null":
                            MessageBox.Show("Icorrecto", "Las credenciales proporcionadas no son validas");
                            break;
                        default:
                            App.Current.Properties["User"] = resp;
                            var form =new Principal();                  
                            this.Close();
                            form.Show();

                            break;
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

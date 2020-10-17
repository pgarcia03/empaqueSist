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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AppEmpaqueRocedes.Logica;
using AppEmpaqueRocedes.Model;

namespace AppEmpaqueRocedes.Seguridad
{
    /// <summary>
    /// Lógica de interacción para CrearUsuario.xaml
    /// </summary>
    public partial class CrearUsuario : UserControl
    {
        //bool band;

        public CrearUsuario()
        {
            InitializeComponent();

            var lista = new string[] { "Seleccione...", "Admin", "Empaque", "SAdmin", "Tickets" };

            cbxroles.ItemsSource = lista;
            cbxroles.SelectedIndex = 0;

            cargaUsuarios();
          //  band = false;
        }

        void cargaUsuarios()
        {
            try
            {
                var listaUser = UsuarioNeg.GetUsuario();

                gridUsuarios.ItemsSource = listaUser;
            }
            catch (Exception)
            {

                throw;
            }
        }

        bool Validaciondeentradas(TextBox us, PasswordBox pass, PasswordBox pass2, ComboBox combo)
        {
            if (string.IsNullOrEmpty(us.Text.TrimStart()) || string.IsNullOrEmpty(pass.Password.Trim()) || string.IsNullOrEmpty(pass2.Password.Trim()) || combo.SelectedItem.ToString() == "Seleccione...")
            {

                MessageBox.Show("Aviso", "Verifique datos ingresados!!!, ", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }

            return false;
        }

        private void BtnCrearUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               

                    string password = HashClass.EncodePassword(string.Concat(txtuser.Text.ToLower(), txtcontraseña.Password));

                    var res = Validaciondeentradas(txtuser, txtcontraseña, txtconfimacionC, cbxroles);

                    if (res)
                        return;

                    if (!txtcontraseña.Password.ToString().Equals(txtconfimacionC.Password.ToString()))
                    {
                        MessageBox.Show("Alerta", "La Confirmacion de contraseña no coincide");
                        return;
                    }

                    var us = new tbUserEmpaque();
                    us.nombreUsuario = txtuser.Text.ToLower();
                    us.contraseña = password;
                    us.rol = cbxroles.SelectedItem.ToString();

                    BtnCrearUser.IsEnabled = false;

                    var response = (int)UsuarioNeg.SaveUsuario(us);

                    if (response == 1)
                    {
                        txtuser.Text = string.Empty;
                        txtcontraseña.Password = string.Empty;
                        txtconfimacionC.Password = string.Empty;
                        cbxroles.SelectedIndex = 0;
                        cargaUsuarios();
                        MessageBox.Show("Exito", "Usuario Creado Correctamente");
                    }
                    else if (response == 2)
                    {
                        
                        MessageBox.Show("Alerta", "El Nombre de Usuario ya Esta en Uso");
                    }
                    else
                    {

                        MessageBox.Show("Alerta", "Ha Fallado el Proceso");
                    }

                    BtnCrearUser.IsEnabled = true;

               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Alerta", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                string mes = ex.Message;
                BtnCrearUser.IsEnabled = true;
                ///throw;
            }

        }

        private void txtconfimacionC_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

              

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // var ComboItem = (string)cbxroles.SelectedItem;

                var re = (tbUserEmpaque)gridUsuarios.SelectedItem;


                var resp = MessageBox.Show("Esta Seguro de Eliminar el Usuario ", re.nombreUsuario, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (resp == MessageBoxResult.Yes)
                {

                    var res = UsuarioNeg.DelUsuario(re);

                    if (res == 1)
                    {
                        MessageBox.Show("Exito", $"Usuario {re.nombreUsuario} Eliminado Correctamente ", MessageBoxButton.OK, MessageBoxImage.Information);
                        cargaUsuarios();

                    }
                    else
                    {
                        MessageBox.Show("Exito", $"Ha Fallado la Eliminación del Usuario {re.nombreUsuario}", MessageBoxButton.OK, MessageBoxImage.Error);

                    }


                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
    }
}

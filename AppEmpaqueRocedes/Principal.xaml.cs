using AppEmpaqueRocedes.Seguridad;
using AppEmpaqueRocedes.Model;
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
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    public partial class Principal : MetroWindow
    {
        public Principal()
        {
            InitializeComponent();

            BtnCrearUsuario.Visibility = Visibility.Collapsed;
            LinkEmpaque.Visibility = Visibility.Collapsed;
            LinkCodigos.Visibility = Visibility.Collapsed;
            LinkReporte.Visibility = Visibility.Collapsed;

            var obj = (tbUserEmpaque)App.Current.Properties["User"];

            lbluser.Content = obj.nombreUsuario.ToUpper();

            stackPanelContenido.Children.Clear();

            switch (obj.rol.Trim())
            {
                case "Admin":
                    LinkEmpaque.Visibility = Visibility.Visible;
                    LinkCodigos.Visibility = Visibility.Visible;
                    LinkReporte.Visibility = Visibility.Visible;
                    stackPanelContenido.Children.Add(new GenerarCodigos());
                    break;
                case "Empaque":
                    LinkEmpaque.Visibility = Visibility.Visible;
                    LinkCodigos.Visibility = Visibility.Visible;
                    stackPanelContenido.Children.Add(new EscanerEmpaque());
                    break;
                case "SAdmin":
                    BtnCrearUsuario.Visibility = Visibility.Visible;
                    LinkEmpaque.Visibility = Visibility.Visible;
                    LinkCodigos.Visibility = Visibility.Visible;
                    LinkReporte.Visibility = Visibility.Visible;
                    stackPanelContenido.Children.Add(new CrearUsuario());
                    break;
                default:
                    break;
            }


        }

        private void LinkEmpaque_Click(object sender, RoutedEventArgs e)
        {
            stackPanelContenido.Children.Clear();

            // Añadir el CustomControl
            stackPanelContenido.Children.Add(new EscanerEmpaque());
        }

        private void LinkReporte_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LinkCodigos_Click(object sender, RoutedEventArgs e)
        {
            stackPanelContenido.Children.Clear();

            // Añadir el CustomControl
            stackPanelContenido.Children.Add(new GenerarCodigos());
        }

        private void BtnCrearUsuario_Click(object sender, RoutedEventArgs e)
        {
            stackPanelContenido.Children.Clear();

            // Añadir el CustomControl
            stackPanelContenido.Children.Add(new CrearUsuario());
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Properties["User"]=null;
        }
    }
}

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
    /// Lógica de interacción para PricipalTickets.xaml
    /// </summary>
    public partial class PricipalTickets : MetroWindow
    {
        public PricipalTickets()
        {
            InitializeComponent();

           
            LinkTickets.Visibility = Visibility.Collapsed;
          

            var obj = (tbUserEmpaque)App.Current.Properties["User"];

            lbluser.Content = obj.nombreUsuario.ToUpper();

            stackPanelContenido.Children.Clear();

            switch (obj.rol.Trim())
            {
                case "SAdmin":
                    LinkTickets.Visibility = Visibility.Visible;
                  //  LinkCodigos.Visibility = Visibility.Visible;
                  //  LinkReporte.Visibility = Visibility.Visible;
                    stackPanelContenido.Children.Add(new TicktesScan());
                    break;
                case "Tickets":
                    LinkTickets.Visibility = Visibility.Visible;
                    //  LinkCodigos.Visibility = Visibility.Visible;
                    //  LinkReporte.Visibility = Visibility.Visible;
                    stackPanelContenido.Children.Add(new TicktesScan());
                    break;
                default:

                    break;
            }


        }
     
        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Properties["User"] = null;
        }

        private void LinkTickets_Click(object sender, RoutedEventArgs e)
        {
            stackPanelContenido.Children.Clear();

            // Añadir el CustomControl
            stackPanelContenido.Children.Add(new TicktesScan());
        }
    }
}

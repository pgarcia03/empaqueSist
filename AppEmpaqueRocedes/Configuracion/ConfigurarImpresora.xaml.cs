using AppEmpaqueRocedes.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
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
//using System.Windows.Shapes;

namespace AppEmpaqueRocedes.Configuracion
{
    /// <summary>
    /// Interaction logic for ConfigurarImpresora.xaml
    /// </summary>
    public partial class ConfigurarImpresora : UserControl
    {

        string ruta;
        public ConfigurarImpresora()
        {
            InitializeComponent();

          //  var CurrentDirectory = Directory.GetCurrentDirectory();

          //  ruta = CurrentDirectory.ToString();

            cbxImpresora.ItemsSource = getimpresoras();

            cbxImpresora.SelectedIndex = 0;

            lblruta.Content = string.Empty;

        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ruta= System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                var impresoraSeleccionada = cbxImpresora.SelectedItem.ToString();

                var obj = new objetoJson { impresora = impresoraSeleccionada };

                string jsonString = JsonConvert.SerializeObject(obj);

                string path = Path.Combine(ruta, "impresoraSeleccionada.json");

                System.IO.File.WriteAllText(path, jsonString);

                lblruta.Content = "Ruta: " + path;

                MessageBox.Show("Guardado con exito!","Exito",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {

                lblruta.Content = "Ruta: " + ruta;
                MessageBox.Show("Ha ocurrido un error => " + ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);

            }
          

            // var resp = ImpresoraPredeterminada();
        }

        private List<string> getimpresoras()
        {
            var list = new List<string>();

            list.Add("Seleccione...");

            for (Int32 i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                list.Add(PrinterSettings.InstalledPrinters[i].ToString());
            }

            return list;

        }

        private string ImpresoraPredeterminada()
        {
            for (Int32 i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                PrinterSettings a = new PrinterSettings();
                a.PrinterName = PrinterSettings.InstalledPrinters[i].ToString();
                if (a.IsDefaultPrinter)
                { return PrinterSettings.InstalledPrinters[i].ToString(); }
            }
            return "";
        }
    }
}

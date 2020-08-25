using AppEmpaqueRocedes.Model;
using AppEmpaqueRocedes.Logica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Speech.Synthesis;
using System.Threading;
using Seagull.BarTender.Print;

namespace AppEmpaqueRocedes
{
    /// <summary>
    /// Lógica de interacción para EscanerEmpaque.xaml
    /// </summary>
    public partial class EscanerEmpaque : UserControl
    {

        public EscanerEmpaque()
        {
            InitializeComponent();

            var obj = (tbUserEmpaque)App.Current.Properties["User"];

            usuario = obj.nombreUsuario.ToLower().Trim();

            //  txtcorte.TextChanged += new TextChangedEventHandler(txtcorte_TextChanged);
            //   txtcorte.Focus();
            txtcodigo.IsEnabled = false;

            idporder = 0;
            idbox = 0;
            cont = 0;
            band = true;


            var ListaClasificacion = new string[] { "Seleccione...", "Primera", "Segunda", "Tercera" };

            comboclasificacion.ItemsSource = ListaClasificacion;

            comboclasificacion.SelectedIndex = 0;

            _worker = new BackgroundWorker();

            _worker.WorkerReportsProgress = true;

            _worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_worker_DoWork);

        }

        private BackgroundWorker _worker;
        private int idporder, idbox, cont;
        private List<PorderDat> lista = new List<PorderDat>();
        private bool band;
        private string usuario;


        #region Tareas Segundo plano
        private void CancelWorker(object sender, RoutedEventArgs e)
        {
            _worker.CancelAsync();
        }

        void _worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                List<object> genericlist = e.Argument as List<object>;

                var item = (getcorteBox_Result)genericlist[0];
                //var codigobox = (string)genericlist[1];

                BackgroundWorker worker = sender as BackgroundWorker;

                var lb = @"D:\Box.btw";
                using (Engine engine = new Engine(true))
                {
                    engine.Start();

                    if (!item.estado.Equals("Error"))
                    {

                        LabelFormatDocument btformate = engine.Documents.Open(lb);
                        btformate.SubStrings["lblcorte"].Value = item.corte.TrimEnd();
                        btformate.SubStrings["lblestilo"].Value = item.estilo.TrimEnd();
                        btformate.SubStrings["lblunidades"].Value = item.unidades.ToString();
                        btformate.SubStrings["lblcolor"].Value = item.color.TrimEnd();
                        btformate.SubStrings["lblestado"].Value = item.estado.TrimEnd();
                        btformate.SubStrings["lblcodigobox"].Value = item.codigoBox.TrimEnd();
                        btformate.SubStrings["lblcorteGuion"].Value = item.corteguion.TrimEnd();

                        var resp = btformate.Print();

                        //  Task.Run(() => { return ActualizarEstado(item.codigoBarra); });
                    }

                    //engine.Start();
                    //  btformate.PrinterCodeTemplate.Performance.AllowSerialization = false;
                    // btformate.ExportImageToClipboard(Seagull.BarTender.Print.ColorDepth.ColorDepth256, new Resolution(200));
                    //  btformat.Close(BarTender.BtSaveOptions.btDoNotSaveChanges);
                    engine.Stop();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //  throw;
            }

        }
        #endregion

        #region Eventos de Cajas de texto
        //listo
        private void txtcorte_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (band)
                {
                    lblsugestion.ItemsSource = null;
                    lblsugestion.Visibility = Visibility.Collapsed;
                    lblCondicional.Content = "NoSelect";
                    txtcorte.Text = string.Empty;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        //listo
        private void txtcorte_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                band = false;

                lblsugestion.SelectedIndex = 0;
                lblsugestion.Focus();
            }
        }
        //listo
        private void txtcorte_TextChanged(object sender, TextChangedEventArgs e)
        {
            var corte = txtcorte.Text;

            if (txtcorte.Text.Count() > 2)
            {

                lista = PorderNeg.GetPordersAutocompletado(corte);

                List<string> listas = new List<string>();

                foreach (var item in lista)
                {
                    listas.Add(item.POrder.TrimEnd());
                }

                if (lista.Count > 0)
                {
                    if (lblCondicional.Content.ToString() == "NoSelect")
                    {
                        lblsugestion.ItemsSource = listas;
                        lblsugestion.Visibility = Visibility.Visible;
                    }

                }
                else if (txtcorte.Text == "")
                {
                    lblsugestion.ItemsSource = null;
                    lblsugestion.Visibility = Visibility.Collapsed;
                }
                else
                {
                    lblsugestion.ItemsSource = null;
                    lblsugestion.Visibility = Visibility.Collapsed;
                }

            }

        }

        private void txtcodigo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (txtcodigo.Text != string.Empty && idbox != 0)
                    {
                        var bandera1 = false;// verifica si el codigo de barra existe en base de datos 
                        var bandera = false; // Verifica si el codigo ha sido escanedo o esta inactivo

                        using (AuditoriaEntities contex = new AuditoriaEntities())
                        {
                            var codigobarra = txtcodigo.Text.TrimEnd();

                            var busqueda = contex.tbBultosCodigosBarra.FirstOrDefault(x => x.codigoBarra.TrimEnd().Equals(codigobarra));

                            if (busqueda == null)
                            {
                                bandera1 = true;
                            }
                            else
                            {
                                // valida que el carton exista y no halla sido escaneado
                                //if (busqueda.estado.TrimEnd().Equals("Escaneado") || busqueda.estado.TrimEnd().Equals("Inactivo"))
                                if (busqueda.Restante<=0)
                                {
                                    bandera = true;
                                }

                            }

                        }


                        if (bandera1)
                        {

                            // string us = (string)App.Current.Properties["user"];
                            Thread tarea = new Thread(new ParameterizedThreadStart(mensaje));
                            tarea.Start("Codigo no existe");

                            lblstatus.Content = "Codigo no existe!";
                            lblCodigoScan.Content = txtcodigo.Text.TrimEnd();

                            txtcodigo.Text = string.Empty;
                            txtcodigo.Focus();

                        }
                        else
                        {
                            if (bandera)
                            {
                                lblstatus.Content = "Exceso de unidades en talla";
                                lblCodigoScan.Content = txtcodigo.Text.TrimEnd();

                                txtcodigo.Text = string.Empty;
                                txtcodigo.Focus();

                                Thread tarea = new Thread(new ParameterizedThreadStart(mensaje));
                                tarea.Start("Exceso de unidades en talla");

                            }
                            else
                            {

                                var regis = CodigosNeg.EscaneoCodigo(lblbox.Content.ToString(), txtcodigo.Text.TrimEnd(), usuario);

                                // var regis = (int)resp[0];
                                if (regis == 1)
                                {
                                    cont += regis;

                                    lblUnidadesCaja.Content = cont.ToString();

                                    var totalunidades = Convert.ToInt16(lblTotalEmpaquadas.Content.ToString());

                                    lblTotalEmpaquadas.Content = totalunidades + regis;

                                    lblCodigoScan.Content = txtcodigo.Text.TrimEnd();
                                    lblstatus.Content = "Codigo Leido Correctamente!!!";

                                    txtcodigo.Text = string.Empty;
                                    txtcodigo.Focus();

                                    Thread tarea = new Thread(new ParameterizedThreadStart(mensaje));
                                    tarea.Start("Codigo Leido Correctamente!!!");
                                }
                                else
                                {

                                    lblCodigoScan.Content = txtcodigo.Text.TrimEnd();
                                    lblstatus.Content = "Codigo No Leido!!!";

                                    txtcodigo.Text = string.Empty;
                                    txtcodigo.Focus();

                                    lblCodigoScan.Content = string.Empty;

                                    Thread tarea = new Thread(new ParameterizedThreadStart(mensaje));
                                    tarea.Start("Error de Lectura, Intente de nuevo por favor!!!");

                                }


                            }
                        }

                    }
                    else
                    {

                        lblstatus.Content = "Campos vacíos";
                        txtcodigo.Text = "";
                        txtcodigo.Focus();
                        Thread tarea = new Thread(new ParameterizedThreadStart(mensaje));
                        tarea.Start("Campos vacíos");

                    }
                }
            }
            catch (Exception ex)
            {

                lblCodigoScan.Content = "";
                txtcodigo.Text = string.Empty;
                txtcodigo.Focus();
                lblstatus.Content = "";

                MessageBox.Show(ex.Message);
            }
        }

        private void txtboxscan_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {

                    limpiarTodo();

                    var box = txtboxscan.Text.Trim().ToLower();

                    var resp = CodigosNeg.BuscarCaja(box);

                    if (resp == null)
                    {
                        MessageBox.Show("El codigo no existe", "Advertencia");
                    }
                    else
                    {
                        cont = (int)resp.totalencaja;

                        lblUnidadesCaja.Content = cont;
                        lblbox.Content = resp.codigoBox.TrimEnd();

                        lblCorte.Content = resp.corteCompleto.TrimEnd();
                        lblCorteT.Content = resp.corteCompleto.TrimEnd();

                        lblEstilo.Content = resp.estilo.TrimEnd();
                        lblEstiloT.Content = resp.estilo.TrimEnd();

                        lblunidades.Content = resp.Quantity;
                        lblestadobox.Content = resp.clasificacion==string.Empty?"Pendiente":resp.clasificacion.TrimEnd();
                        idbox = resp.id;

                        txtboxscan.Clear();
                        txtcodigo.Clear();
                        txtcodigo.IsEnabled = true;
                        txtcodigo.Focus();
                        txtcorte.Clear();

                        idporder = resp.Id_Order;

                        var total = PorderNeg.TotalesLeft(idporder);

                        lblTotalcajas.Content = (total.Count() - 1).ToString();
                        lblTotalEmpaquadas.Content = total.Sum(x => x.unidades);

                        lblsugestion.Visibility = Visibility.Collapsed;
                        lblCondicional.Content = "Select";


                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        //Listo
        #region Eventos ListBox

        private void lblsugestion_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            band = true;
        }

        private void lblsugestion_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (lblsugestion.ItemsSource != null)
                {

                    lblsugestion.Visibility = Visibility.Collapsed;

                    if (lblsugestion.SelectedIndex != -1)
                    {
                        lblCondicional.Content = "Select";
                        txtcorte.Text = lblsugestion.SelectedItem.ToString();

                        var obj = lista.FirstOrDefault(x => x.POrder.Equals(txtcorte.Text));
                        idporder = obj.Id_Order;
                        lblCorte.Content = obj.POrder;
                        lblEstilo.Content = obj.style;
                        lblunidades.Content = obj.Quantity;
                        lblCorteT.Content = obj.POrder;
                        lblEstiloT.Content = obj.style;

                        txtcorte.Text = string.Empty;

                        lblstatus.Content = "Crear Caja Para Iniciar Escaneo";

                        if (idporder != 0)
                        {

                            var total = PorderNeg.Totales(idporder);

                            lblTotalcajas.Content = total.Count().ToString();
                            lblTotalEmpaquadas.Content = total.Sum(x => x.unidades);

                        }

                        band = true;

                    }

                    txtcorte.TextChanged += new TextChangedEventHandler(txtcorte_TextChanged);

                }
            }
            else if (e.Key == Key.Up)
            {
                band = false;

                var item = lblsugestion.SelectedIndex;

                if (item == 0)
                {
                    txtcorte.Focus();
                }

            }
        }

        private void lblsugestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lblsugestion.ItemsSource != null && band)
                {

                    lblsugestion.Visibility = Visibility.Collapsed;

                    txtcorte.TextChanged -= new TextChangedEventHandler(txtcorte_TextChanged);

                    if (lblsugestion.SelectedIndex != -1)
                    {
                        lblCondicional.Content = "Select";
                        txtcorte.Text = lblsugestion.SelectedItem.ToString();

                        var obj = lista.FirstOrDefault(x => x.POrder.Equals(txtcorte.Text));
                        idporder = obj.Id_Order;
                        lblCorte.Content = obj.POrder;
                        lblEstilo.Content = obj.style;
                        lblunidades.Content = obj.Quantity;
                        lblCorteT.Content = obj.POrder;
                        lblEstiloT.Content = obj.style;
                        lblstatus.Content = "Crear Caja Para Iniciar Escaneo";
                        txtcorte.Text = string.Empty;
                        if (idporder != 0)
                        {

                            var total = PorderNeg.Totales(idporder);

                            lblTotalcajas.Content = total.Count().ToString();
                            lblTotalEmpaquadas.Content = total.Sum(x => x.unidades);

                        }
                    }

                    txtcorte.TextChanged += new TextChangedEventHandler(txtcorte_TextChanged);

                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region Eventos click de botones
        private async void BtnAbrir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (idporder != 0 && lblEstilo.Content.ToString() != string.Empty)
                {
                    var resp = await CodigosNeg.crearCaja(idporder, lblEstilo.Content.ToString());

                    lblbox.Content = resp.codigo;
                    idbox = resp.id;

                    lblUnidadesCaja.Content = 0;
                    lblCodigoScan.Content = string.Empty;

                    txtcodigo.Text = string.Empty;
                    txtcodigo.IsEnabled = true;
                    txtcodigo.Focus();

                    lblstatus.Content = "Esperando Lectura de Escanner";

                    lblestadobox.Content = "Pendiente";
                }

            }
            catch (Exception ex)
            {
                string r = ex.Message;
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lblsugestion.ItemsSource != null && band)
                {

                    lblsugestion.Visibility = Visibility.Collapsed;

                    // txtcorte.TextChanged -= new TextChangedEventHandler(txtcorte_TextChanged);

                    if (lblsugestion.SelectedIndex != -1)
                    {
                        lblCondicional.Content = "Select";
                        txtcorte.Text = lblsugestion.SelectedItem.ToString();

                        var obj = lista.FirstOrDefault(x => x.POrder.Equals(txtcorte.Text));
                        idporder = obj.Id_Order;
                        lblCorte.Content = obj.POrder;
                        lblEstilo.Content = obj.style;
                        lblunidades.Content = obj.Quantity;
                        lblCorteT.Content = obj.POrder;
                        lblEstiloT.Content = obj.style;

                        if (idporder != 0)
                        {

                            var total = PorderNeg.Totales(idporder);

                            lblTotalcajas.Content = total.Count().ToString();
                            lblTotalEmpaquadas.Content = total.Sum(x => x.unidades);

                        }
                    }

                    txtcorte.TextChanged += new TextChangedEventHandler(txtcorte_TextChanged);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BtnBuscarBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                limpiarTodo();

                var box = txtboxscan.Text.Trim().ToLower();

                var resp = CodigosNeg.BuscarCaja(box);

                if (resp == null)
                {
                    MessageBox.Show("El codigo no existe", "Advertencia");
                }
                else
                {
                    cont = (int)resp.totalencaja;

                    lblUnidadesCaja.Content = cont;
                    lblbox.Content = resp.codigoBox.TrimEnd();

                    lblCorte.Content = resp.corteCompleto.TrimEnd();
                    lblCorteT.Content = resp.corteCompleto.TrimEnd();

                    lblEstilo.Content = resp.estilo.TrimEnd();
                    lblEstiloT.Content = resp.estilo.TrimEnd();

                    lblunidades.Content = resp.Quantity;
                   // lblestadobox.Content = resp.clasificacion;
                    lblestadobox.Content = resp.clasificacion == string.Empty ? "Pendiente" : resp.clasificacion.TrimEnd();
                    idbox = resp.id;

                    txtboxscan.Clear();
                    txtcodigo.Clear();
                    txtcodigo.IsEnabled = true;
                    txtcodigo.Focus();
                    txtcorte.Clear();

                    idporder = resp.Id_Order;

                    var total = PorderNeg.TotalesLeft(idporder);

                    lblTotalcajas.Content = (total.Count() - 1).ToString();
                    lblTotalEmpaquadas.Content = total.Sum(x => x.unidades);

                    lblsugestion.Visibility = Visibility.Collapsed;
                    lblCondicional.Content = "Select";

                    lblstatus.Content = "Esperando Lectura de Escanner";

                }


            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var clasificacion = comboclasificacion.SelectedItem.ToString();
                // var b = string.Compare(clasificacion, "Seleccione...");

                if ((idbox != 0 || !lblbox.Content.ToString().Equals(string.Empty)) && string.Compare(clasificacion, "Seleccione...") != 0)
                {

                    var resp = CodigosNeg.imprimirBox(lblbox.Content.ToString(), usuario, clasificacion.ToUpper());


                    List<object> arg = new List<object>();
                    arg.Add(resp);

                    Thread tarea = new Thread(new ParameterizedThreadStart(mensajeVoz));
                    tarea.Start("Iniciando Proceso de impresión");

                    _worker.RunWorkerAsync(arg);

                    var total = PorderNeg.Totales(idporder);

                    lblTotalcajas.Content = total.Count().ToString();
                    lblTotalEmpaquadas.Content = total.Sum(x => x.unidades);

                    limpiarDespuesDImprimir();
                }
                else
                {

                    lblstatus.Content = "";

                    Thread tarea = new Thread(new ParameterizedThreadStart(mensaje));
                    tarea.Start("Campos vacíos");
                }



            }
            catch (Exception)
            {

                MessageBox.Show("No es posible realizar el comando");
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                limpiarTodo();
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion

        #region funciones
        void limpiarTodo()
        {
            //variables
            idporder = 0;
            idbox = 0;
            cont = 0;
            band = true;
            lista.Clear();// = null;

            //autocompletado
            txtcorte.Text = string.Empty;
            txtcorte.Focus();

            //control panel izquierdo
            lblCorte.Content = string.Empty;
            lblEstilo.Content = string.Empty;
            lblunidades.Content = string.Empty;
            lblTotalEmpaquadas.Content = string.Empty;
            lblTotalcajas.Content = string.Empty;

            //titulos
            lblCorteT.Content = string.Empty;
            lblEstiloT.Content = string.Empty;


            //estados
            lblCodigoScan.Content = string.Empty;
            lblbox.Content = string.Empty;
            lblUnidadesCaja.Content = 0;
            lblstatus.Content = "Realice Busqueda, Esperando...";
            comboclasificacion.SelectedIndex = 0;
            lblestadobox.Content = string.Empty;

        }

        void limpiarDespuesDImprimir()
        {
            //variables
            idbox = 0;
            cont = 0;
            band = true;

            //modificar unidades empaquadas y total de cajas

            //estados
            txtcodigo.Clear();
            txtcodigo.IsEnabled = false;
            lblCodigoScan.Content = string.Empty;
            lblbox.Content = string.Empty;
            lblUnidadesCaja.Content = 0;
            lblstatus.Content = "Crear Caja Para Iniciar Escaneo";
            comboclasificacion.SelectedIndex = 0;
            lblestadobox.Content = string.Empty;

        }

        void mensajeVoz(object texto)
        {
            SpeechSynthesizer voz = new SpeechSynthesizer();
            voz.Rate = 0;
            voz.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            voz.SetOutputToDefaultAudioDevice();
            voz.Speak(texto.ToString());
        }

        void mensaje(object texto)
        {
            SpeechSynthesizer voz = new SpeechSynthesizer();
            voz.Rate = 0;
            voz.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            voz.SetOutputToDefaultAudioDevice();
            voz.Speak(texto.ToString());
        }



        #endregion

    }
}

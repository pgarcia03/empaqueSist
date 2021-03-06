﻿using AppEmpaqueRocedes.Logica;
using AppEmpaqueRocedes.Model;
using Seagull.BarTender.Print;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// Lógica de interacción para GenerarCodigos.xaml
    /// </summary>
    public partial class GenerarCodigos : UserControl
    {
        private bool band = true;
        private int idporder = 0;
        private List<PorderDat> lista = new List<PorderDat>();
        private List<CodigosDat> listacodigo = new List<CodigosDat>();
        private BackgroundWorker _worker;

        #region Tareas Segundo plano

        private void CancelWorker(object sender, RoutedEventArgs e)
        {
            _worker.CancelAsync();
        }

        private async Task<string> ActualizarEstado(string codigoBarra)
        {
            try
            {
                var resp = await Task.Run(() =>
                {
                    using (var contex = new AuditoriaEntities())
                    {
                        var obj = contex.tbBultosCodigosBarra.Find(codigoBarra);

                        obj.estado = "Impreso";
                        obj.fechaImpreso = contex.ExtraefechaServidor(1).First();

                        contex.SaveChanges();

                        return "Exito";
                    }


                });

                return resp;

            }
            catch (Exception ex)
            {
                string mes = ex.Message;
                return "Error";
            }
        }

        void _worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                List<object> genericlist = e.Argument as List<object>;

                var Lista = (List<CodigosDat>)genericlist[0];
                var bandI = (bool)genericlist[1];

                BackgroundWorker worker = sender as BackgroundWorker;

                var lb = @"D:\ticketUni.btw";
                using (Engine engine = new Engine(true))
                {
                    engine.Start();

                    foreach (var item in Lista)
                    {
                        if (item.Estado.Equals("Generado") || bandI )
                        {                        

                            LabelFormatDocument btformate = engine.Documents.Open(lb);
                            btformate.SubStrings["lblcorte"].Value = item.POrder.Trim();
                            btformate.SubStrings["lbltalla"].Value = item.Size.TrimEnd();
                            btformate.SubStrings["lblcodigo"].Value = item.codigoBarra.TrimEnd();

                            var resp= btformate.Print();

                            

                            Task.Run(() => { return ActualizarEstado(item.codigoBarra); });
                        }
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

        public GenerarCodigos()
        {
            InitializeComponent();

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            _worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_worker_DoWork);
        }

        #region Evento de txtcorte
        private void Txtcorte_GotFocus(object sender, RoutedEventArgs e)
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
            catch (Exception ex)
            {
                var mess = ex.Message;
            }
        }
        private void Txtcorte_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Down)
                {

                    band = false;

                    lblsugestion.Focus();
                    lblsugestion.SelectedIndex = 0;
                    //lblsugestion.Focus();

                }
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }

        }
        private void Txtcorte_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                string mess = ex.Message;
            }

        }
        #endregion

        #region Eventos listBox
        private void Lblsugestion_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            band = true;
        }

        private void Lblsugestion_KeyUp(object sender, KeyEventArgs e)
        {
            try
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
                            lblUnidades.Content = obj.Quantity;

                            if (idporder != 0)
                            {
                                listacodigo = CodigosNeg.GetCodigosDatsXtalla(idporder).Result;

                                gridCodigos.ItemsSource = listacodigo;
                            }

                            band = true;

                        }

                        txtcorte.TextChanged += new TextChangedEventHandler(Txtcorte_TextChanged);

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
            catch (Exception ex)
            {
                string mess = ex.Message;
            }

        }

        private void Lblsugestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lblsugestion.ItemsSource != null && band)
                {

                    txtcorte.TextChanged -= new TextChangedEventHandler(Txtcorte_TextChanged);

                    if (lblsugestion.SelectedIndex != -1)
                    {
                        lblCondicional.Content = "Select";
                        txtcorte.Text = lblsugestion.SelectedItem.ToString();

                        lblsugestion.Visibility = Visibility.Collapsed;
                        var obj = lista.FirstOrDefault(x => x.POrder.Equals(txtcorte.Text));
                        idporder = obj.Id_Order;
                        lblCorte.Content = obj.POrder;
                        lblEstilo.Content = obj.style;
                        lblUnidades.Content = obj.Quantity;


                        if (idporder != 0)
                        {
                            listacodigo = CodigosNeg.GetCodigosDatsXtalla(idporder).Result;

                            gridCodigos.ItemsSource = listacodigo;
                        }

                        band = true;
                    }

                    txtcorte.TextChanged += new TextChangedEventHandler(Txtcorte_TextChanged);

                }
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
            }
        }
        #endregion

        #region Eventos Botones _Click
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (idporder != 0)
                {
                    listacodigo = CodigosNeg.GetCodigosDatsXtalla(idporder).Result;

                    gridCodigos.ItemsSource = listacodigo;
                }

            }
            catch (Exception ex)
            {
                string mess = ex.Message;
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnImprimir.IsEnabled = false;
                if (idporder != 0 && listacodigo.Count > 0)
                {
                    var num = lblCorte.Content.ToString().IndexOf('-', 4);

                    var corte = lblCorte.Content.ToString().Substring(0, num == -1 ? lblCorte.Content.ToString().Length : num);

                    var resp = CodigosNeg.GuardarCodigos(listacodigo, corte);

                    var cont = listacodigo.Count();

                    if (Convert.ToInt16(resp[1]) == 200)
                    {

                       // limpiar();

                        var result = (List<CodigosDat>)resp[0];

                        string respMess = cont > result.Count ? "Algunos codigos de No han sido Guardados!!!, Desea imprimirlos??" : "Guardado con exito!!!, Desea imprimirlos??";

                        var respmensaje = MessageBox.Show(respMess, "Informacion", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                        if (respmensaje == MessageBoxResult.OK)
                        {
                         //   MessageBoxTimeout((System.IntPtr)0, $"Iniciando impresion de {result.Count} tickets ", "Informacion", 0, 0, 1000);

                           // Thread tarea = new Thread(new ParameterizedThreadStart(MensajeVoz));
                           // tarea.Start("Iniciando Trabajo de impresión");

                            List<object> arg = new List<object>
                            {
                                result,
                                false
                            };

                            Limpiar();

                            _worker.RunWorkerAsync(arg);
                        }

                    }
                    else
                    {
                        //Thread tarea = new Thread(new ParameterizedThreadStart(MensajeVoz));
                       // tarea.Start("Ha ocurrido un error");

                        string respMess = "Ha ocurrido un error";

                        var respmensaje = MessageBox.Show(respMess, "Informacion", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                        ///  MessageBox.Show("Ha ocurrido un error comuniquese con el departamento de informatica para recibir asistencia??", "Informacion", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {

                    string respMess = "Debe Seleccionar corte y Generar codigos";

                    var respmensaje = MessageBox.Show(respMess, "Informacion", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                   // Thread tarea = new Thread(new ParameterizedThreadStart(MensajeVoz));
                   // tarea.Start("Debe Seleccionar corte y Generar codigos");
                }

                BtnImprimir.IsEnabled = true;

            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                BtnImprimir.IsEnabled = true;
                //Thread tarea = new Thread(new ParameterizedThreadStart(MensajeVoz));
                //tarea.Start("Ha ocurrido un error");

                string respMess = "Ha ocurrido un error";

                var respmensaje = MessageBox.Show(respMess, "Informacion", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Limpiar();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void BtnImprimirAvanzada_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnImprimirAvanzada.IsEnabled = false;
                if (idporder != 0 && listacodigo.Count > 0 && txtinicio.Text!=string.Empty && txtfinal.Text!=string.Empty)
                {
                    

                    if (ValidarNumeros(txtinicio.Text) && ValidarNumeros(txtfinal.Text))
                    {
                        var secI = Convert.ToInt32(txtinicio.Text) < Convert.ToInt32(txtfinal.Text) ? Convert.ToInt32(txtinicio.Text) : Convert.ToInt32(txtfinal.Text);
                        var secF = Convert.ToInt32(txtinicio.Text) < Convert.ToInt32(txtfinal.Text) ? Convert.ToInt32(txtfinal.Text) : Convert.ToInt32(txtfinal.Text);
                        string respMess = $"Desea imprimir la siguiente secuencia {secI}-{secF}??";

                        var respmensaje = MessageBox.Show(respMess, "Informacion", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                        if (respmensaje == MessageBoxResult.OK)
                        {
                           // Thread tarea = new Thread(new ParameterizedThreadStart(MensajeVoz));
                           // tarea.Start("Iniciando Trabajo de impresión");

                            var ListaSecuencia = listacodigo.Where(x => x.NSeq >= secI && x.NSeq <= secF).ToList();

                            // MessageBoxTimeout((System.IntPtr)0, $"Iniciando impresion de {ListaSecuencia.Count} tickets ", "Informacion", 0, 0, 1000);

                            List<object> arg = new List<object>
                            {
                                ListaSecuencia,
                                true
                            };

                            Limpiar();

                            _worker.RunWorkerAsync(arg);

                        
                        }
                        else
                        {
                            Limpiar();
                        }
                    }
                    else
                    {
                        //   Thread tarea = new Thread(new ParameterizedThreadStart(MensajeVoz));
                        //   tarea.Start("Debe ingresar datos numericos en secuencias");
                        string respMess = "Debe ingresar datos numericos en secuencias";

                        var respmensaje = MessageBox.Show(respMess, "Informacion", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    }
                   
                }
                else
                {
                  //  Thread tarea = new Thread(new ParameterizedThreadStart(MensajeVoz));
                  //  tarea.Start("Debe Seleccionar corte y Generar codigos");
                    string respMess = "Debe Seleccionar corte y Generar codigos";

                    var respmensaje = MessageBox.Show(respMess, "Informacion", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                }

                BtnImprimirAvanzada.IsEnabled = true;

            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                BtnImprimir.IsEnabled = true;
              //  Thread tarea = new Thread(new ParameterizedThreadStart(MensajeVoz));
              //  tarea.Start("Ha ocurrido un error");

                string respMess = "Ha ocurrido un error";

                var respmensaje = MessageBox.Show(respMess, "Informacion", MessageBoxButton.OKCancel, MessageBoxImage.Error);

            }
        }
        #endregion

        #region funciones
       

        void Limpiar()
        {
            lblsugestion.ItemsSource = null;
            lblsugestion.Visibility = Visibility.Collapsed;
            lblCondicional.Content = "NoSelect";
            txtcorte.Text = string.Empty;

            lblCorte.Content = string.Empty;
            lblEstilo.Content = string.Empty;
            lblUnidades.Content = "0";
            txtinicio.Text = string.Empty;
            txtfinal.Text = string.Empty;

            listacodigo.Clear();
            lista.Clear();
            idporder = 0;
            band = true;
            gridCodigos.ItemsSource = null;// = listacodigo;
                                           // gridCodigos.ItemsSource=listacodigo;

        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern int MessageBoxTimeout(IntPtr hwnd, String text, String title, uint type, Int16 wLanguageId, Int32 milliseconds);


        void MensajeVoz(object texto)
        {
            SpeechSynthesizer voz = new SpeechSynthesizer
            {
                Rate = 0
            };
            voz.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            voz.SetOutputToDefaultAudioDevice();
            voz.Speak(texto.ToString());
        }

        bool ValidarNumeros( string cadena)
        {
            return Regex.IsMatch(cadena, @"^[0-9]");
        }

        #endregion

      
    }
}

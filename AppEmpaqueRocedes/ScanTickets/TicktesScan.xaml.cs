using AppEmpaqueRocedes.Logica;
using AppEmpaqueRocedes.Model;
using Seagull.BarTender.Print;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    /// Lógica de interacción para TicktesScan.xaml
    /// </summary>
    public partial class TicktesScan : UserControl
    {

        //  private bool band = true;
        //  private int idporder = 0;
        public int idemp = 0;
        public string emp = "";
        //  public List<Pervasives> lista = new List<Pervasives>();

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
                        if (item.Estado.Equals("Generado") || bandI)
                        {

                            LabelFormatDocument btformate = engine.Documents.Open(lb);
                            btformate.SubStrings["lblcorte"].Value = item.POrder.Trim();
                            btformate.SubStrings["lbltalla"].Value = item.Size.TrimEnd();
                            btformate.SubStrings["lblcodigo"].Value = item.codigoBarra.TrimEnd();

                            var resp = btformate.Print();



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

        void cargaCombo()
        {
            try
            {
                using (AuditoriaEntities contex = new AuditoriaEntities())
                {
                    var linea = contex.Linea.ToList();

                    var bihorario = contex.Biohorario.ToList();


                    linea.Insert(0,new Linea { id_linea=0,numero= "Seleccione..."});
                    bihorario.Insert(0, new Biohorario { id_bio = 0, bihorario = "Seleccione..." });

                    comboLinea.ItemsSource = linea;//lineaArr;
                    comboLinea.DisplayMemberPath = "numero";
                    comboLinea.SelectedValuePath = "id_linea";
                    comboLinea.SelectedIndex = 0;

                    comboBihorario.ItemsSource = bihorario;//lineaArr;
                    comboBihorario.DisplayMemberPath = "bihorario";
                    comboBihorario.SelectedValuePath = "id_bio";
                    comboBihorario.SelectedIndex = 0;

                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public TicktesScan()
        {
            InitializeComponent();

            var obj = (tbUserEmpaque)App.Current.Properties["User"];

            emp = obj.nombreUsuario.ToUpper();

            txtoperario.Focus();

            cargaCombo();

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            _worker.DoWork += new System.ComponentModel.DoWorkEventHandler(_worker_DoWork);
        }

        #region Evento de txtcorte
        private void txtoperario_GotFocus(object sender, RoutedEventArgs e)
        {
            txtoperario.Text = string.Empty;
        }

        private void txtoperario_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {

                    limpiarOper();

                    var operario = txtoperario.Text.Trim().ToLower();

                    var sql = "select top 1 empno,estatus,module,lastname,firstname,badgeno,loc1,loc2 from employee where badgeno = '" + operario + "'";

                    var dt = Pervasive_PSQL.GetDataTable(sql);

                    if (dt.Rows.Count == 0)
                    {
                        txtoperario.Text = string.Empty;
                        txtoperario.Focus();

                        lblEstadoTicket.Content = "El operario " + operario + " no existe";

                        MensajeVoz("El operario no existe!!!");

                        //MessageBox.Show("El operario no existe", "Advertencia");
                    }
                    else
                    {
                        var nombre = string.Concat(dt.Rows[0][4], " ", dt.Rows[0][3]).ToUpper().TrimEnd();
                        lblNombre.Content = nombre;//string.Concat(dt.Rows[0][4], " ", dt.Rows[0][3]);
                        lblCod.Content = dt.Rows[0][0].ToString();
                        lblModulo.Content = dt.Rows[0][6].ToString();
                        lblLinea.Content = dt.Rows[0][7].ToString();

                        txtoperario.Text = string.Empty;

                        txtticket.Focus();


                        lblEstadoTicket.Content = "Lectura Exitosa!!!";

                        //  MensajeVoz("Lectura Exitosa!");

                        using (AuditoriaEntities contex = new AuditoriaEntities())
                        {
                            var codigobarra = dt.Rows[0][5].ToString().TrimEnd();
                            var obj = contex.Operario.Where(x => x.codigoBarra.ToUpper().Equals(codigobarra)).FirstOrDefault();

                            if (obj == null)
                            {
                                var objnew = new Operario
                                {
                                    nombre = nombre,
                                    codigo = dt.Rows[0][0].ToString(),
                                    codigoBarra = dt.Rows[0][5].ToString(),
                                    activo = true

                                };

                                contex.Operario.Add(objnew);
                                contex.SaveChanges();
                                idemp = objnew.id_operario;
                            }
                            else
                            {
                                obj.codigoBarra = dt.Rows[0][5].ToString();
                                obj.codigo = dt.Rows[0][0].ToString();

                                idemp = obj.id_operario;
                            }



                            contex.SaveChanges();


                            var listadb = contex.spdProduccionOperarioDiario(idemp).ToList();

                            lbltotal.Content = listadb.Count.ToString();

                            lbltotalBihorario.Content = comboBihorario.SelectedValue.ToString() == "0" 
                                                        ? 
                                                        listadb.Where(x => x.bihorario == listadb.Max(z => z.bihorario)).Count().ToString() 
                                                        :
                                                        listadb.Where(x => x.bihorario ==Convert.ToInt16(comboBihorario.SelectedValue.ToString())).Count().ToString();

                            gridCodigos.ItemsSource = listadb;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                lblEstadoTicket.Content = ex.Message;

                MensajeVoz(ex.Message);

            }
        }

        private void txtticket_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtticket_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (txtticket.Text.Trim().Equals("11101"))
                    {
                        txtoperario.Clear();
                        txtticket.Clear();
                        txtoperario.Focus();

                        return;
                    }

                    if (comboBihorario.SelectedValue.ToString() == "0" || comboLinea.SelectedValue.ToString() == "0")
                    {
                        lblEstadoTicket.Content = "Debe seleccionar Linea y bihorario";

                        txtticket.Clear();
                        txtticket.Focus();

                        return;
                    }

                    Limpiar();

                    var cadena = txtticket.Text.Trim();// txtticket.Text.Substring(txtticket.Text);

                    var chart = cadena.Substring(0, 1);

                    var serial = chart == "0" ? cadena.Substring(1, cadena.Length - 1) : cadena;

                    var sql = " select s.prodno,s.serialno,o.operno,s.bundleno,o.descr,o.ctdescr,linenumber,plant,b.qty" +
                              " from serial2 s inner join oper o on s.operno = o.operno " +
                              " inner join prod p on s.prodno = p.prodno" +
                              " inner join bundle b on b.prodno = p.prodno" +
                              " where s.serialno = '" + serial + "' and b.bundleno = s.bundleno";

                    lblTicketLeido.Content = serial;

                    var lista = new List<Pervasives>();

                    var dt = Pervasive_PSQL.GetDataTable(sql);

                    foreach (DataRow item in dt.Rows)
                    {
                        var obj = new Pervasives
                        {
                            Prodno = item.ItemArray[0].ToString(),
                            Serialno = item.ItemArray[1].ToString(),
                            Operno = item.ItemArray[2].ToString(),
                            Bundleno = Convert.ToInt16(item.ItemArray[3].ToString()),
                            Descr = item.ItemArray[4].ToString(),
                            Ctdescr = item.ItemArray[5].ToString(),
                            BundleUni = Convert.ToInt32(item.ItemArray[8].ToString())

                        };

                        lista.Add(obj);
                    }

                    if (lista.Count == 0)
                    {
                        txtticket.Text = string.Empty;
                        txtticket.Focus();

                        lblEstadoTicket.Content = "El codigo no existe, Advertencia";

                        //   MensajeVoz("El codigo no existe!!!");
                        //   MessageBox.Show("El codigo no existe", "Advertencia");
                    }
                    else
                    {
                        lblCorte.Content = lista[0].Prodno;
                        lblBulto.Content = lista[0].Bundleno.ToString();
                        lblEstilo.Content = lista[0].Serialno;
                        lbloperacion.Content = lista[0].Descr;
                        lblUnidades.Content = lista[0].BundleUni;

                        // txtticket.Text = string.Empty;
                        // txtticket.Focus();

                        using (AuditoriaEntities contex = new AuditoriaEntities())
                        {
                            var serial2 = dt.Rows[0][1].ToString().TrimEnd();
                            var band = contex.tbProduccionTickets.Where(x => x.serial.Equals(serial2)).FirstOrDefault();

                            if (band == null)
                            {
                                var newobjS = new tbProduccionTickets()
                                {
                                    serial = serial2, // dt.Rows[0][1].ToString(),
                                    codigoOperacion = dt.Rows[0][5].ToString(),
                                    descOperacion = dt.Rows[0][4].ToString(),
                                    corte = dt.Rows[0][0].ToString(),
                                    nSeq = Convert.ToInt16(dt.Rows[0][3].ToString()),
                                    linea = dt.Rows[0][6].ToString(),
                                    fechaRegistro = DateTime.Now,
                                    usuario = emp,
                                    operarioId = idemp,
                                    BundleUni = Convert.ToInt32(dt.Rows[0][8].ToString()),
                                    bihorario=Convert.ToInt16(comboBihorario.SelectedValue.ToString()),
                                    lineaConfeccion= Convert.ToInt16(comboLinea.SelectedValue.ToString())
                                };

                                contex.tbProduccionTickets.Add(newobjS);

                                contex.SaveChanges();



                                txtticket.Text = string.Empty;
                                txtticket.Focus();

                                lblEstadoTicket.Content = "Agregado Correctamente";

                                //  MensajeVoz("Lectura Exitosa");


                            }
                            else
                            {
                                txtticket.Text = string.Empty;
                                txtticket.Focus();
                                lblEstadoTicket.Content = "Lectura Duplicada";

                                //  MensajeVoz("Lectura Duplicada");
                            }


                            var listadb = contex.spdProduccionOperarioDiario(idemp).ToList();

                            lbltotal.Content = listadb.Count.ToString();

                            lbltotalBihorario.Content = comboBihorario.SelectedValue.ToString() == "0"
                                                        ?
                                                        listadb.Where(x => x.bihorario == listadb.Max(z => z.bihorario)).Count().ToString()
                                                        :
                                                        listadb.Where(x => x.bihorario == Convert.ToInt16(comboBihorario.SelectedValue.ToString())).Count().ToString();

                            gridCodigos.ItemsSource = listadb;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                lblEstadoTicket.Content = ex.Message;

                MensajeVoz(ex.Message);

            }
        }

        #endregion

        #region Eventos Botones _Click

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Limpiar();
                Limpiartodo();

            }
            catch (Exception ex)
            {
                lblEstadoTicket.Content = ex.Message;

                MensajeVoz(ex.Message);

            }
        }

        private void comboLinea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtticket.Clear();
                txtticket.Focus();

                lblEstadoTicket.Content = string.Empty;

                //var ee = comboLinea.SelectedValue;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void comboBihorario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                txtticket.Clear();
                txtticket.Focus();

                lblEstadoTicket.Content = string.Empty;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region funciones

        void limpiarOper()
        {
            lblCorte.Content = string.Empty;
            lblEstilo.Content = string.Empty;
            lblUnidades.Content = "0";
            lblBulto.Content = "0";
            lbloperacion.Content = string.Empty;


            lblCod.Content = "";
            lblLinea.Content = "";
            lblModulo.Content = "";
            lblNombre.Content = "";

            lblTicketLeido.Content = string.Empty;
            lblEstadoTicket.Content = string.Empty;
            //idporder = 0;
            idemp = 0;
            // band = true;
            gridCodigos.ItemsSource = null;

            lbltotal.Content = string.Empty;
            lbltotalBihorario.Content = string.Empty;
            //  lista.Clear();
        }

        void Limpiartodo()
        {


            lblCorte.Content = string.Empty;
            lblEstilo.Content = string.Empty;
            lblUnidades.Content = "0";
            lblBulto.Content = "0";
            lbloperacion.Content = string.Empty;


            lblCod.Content = "";
            lblLinea.Content = "";
            lblModulo.Content = "";
            lblNombre.Content = "";

            //idporder = 0;
            idemp = 0;
            // band = true;
            gridCodigos.ItemsSource = null;
            //  lista.Clear();

            txtoperario.Focus();
            lbltotal.Content = string.Empty;
            lbltotalBihorario.Content = string.Empty;
        }

        void Limpiar()
        {

            lblCorte.Content = string.Empty;
            lblEstilo.Content = string.Empty;
            lblUnidades.Content = "0";
            lblBulto.Content = "0";
            lbloperacion.Content = string.Empty;

            lblEstadoTicket.Content = string.Empty;

            lblTicketLeido.Content = string.Empty;


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

        bool ValidarNumeros(string cadena)
        {
            return Regex.IsMatch(cadena, @"^[0-9]");
        }

        #endregion


    }
}

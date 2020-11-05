using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using Pervasive.Data.SqlClient;

/**
 * Pervasive ADO.NET PSQL data provider template
 */

namespace AppEmpaqueRocedes.Logica
{
    /// <summary>
	/// Summary description for Pervasive_PSQL.
	/// </summary>
   public class Pervasive_PSQL
    {
        public static DataTable GetDataTable(string query)
        {
            var cnn = ConfigurationManager.AppSettings["PervasiveSQLClient"];
            PsqlConnection DBConn = new PsqlConnection(cnn.ToString());
            try
            {
                DataTable dt = new DataTable();
                // Open the connection
                DBConn.Open();
                PsqlDataAdapter da = new PsqlDataAdapter();
                da.SelectCommand = new PsqlCommand(query, DBConn);
                da.Fill(dt);
                //Console.WriteLine("Connection Successful!");

                return dt;

            }
            catch (PsqlException ex)
            {
                // Connection failed
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                DBConn.Close();
            }
        }

        public static string GetDataTable1(string query)
        {
            var cnn = ConfigurationManager.AppSettings["PervasiveSQLClient"];
            PsqlConnection DBConn = new PsqlConnection(cnn.ToString());
            PsqlCommand comm = new PsqlCommand();
            try
            {

                DBConn.Open();
                comm.Connection = DBConn;
                comm.CommandText = "Update_Oper";
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.Clear();

                // comm.Parameters.Add()
                comm.Parameters.Add(":id_oper", PsqlDbType.Integer);
                comm.Parameters[comm.Parameters.Count - 1].Value = 1;

                comm.Parameters.Add(":Id_Seccion", PsqlDbType.Integer);
                comm.Parameters[comm.Parameters.Count - 1].Value = 1;

                comm.ExecuteNonQuery();
                return "OK";



            }
            catch (PsqlException ex)
            {
                // Connection failed
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                DBConn.Close();
            }
        }

        public static DataTable GetDataTable1proc(string query)
        {
            var cnn = ConfigurationManager.AppSettings["PervasiveSQLClient"];
            PsqlConnection DBConn = new PsqlConnection(cnn.ToString());
            PsqlCommand comm = new PsqlCommand();
            try
            {

                DBConn.Open();
                comm.Connection = DBConn;
                comm.CommandText = "Update_Oper";
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.Clear();


                comm.Parameters.Add(":id_oper", PsqlDbType.Integer);
                comm.Parameters[comm.Parameters.Count - 1].Value = 1;

                comm.Parameters.Add(":Id_Seccion", PsqlDbType.Integer);
                comm.Parameters[comm.Parameters.Count - 1].Value = 1;

                PsqlDataAdapter da = new PsqlDataAdapter(comm);
                var dt = new DataTable();
                da.Fill(dt);

                return dt;


            }
            catch (PsqlException ex)
            {
                // Connection failed
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                DBConn.Close();
            }
        }

        public static string GetDataTable1proc(string query, string d)
        {
            var cnn = ConfigurationManager.AppSettings["PervasiveSQLClient"];
            PsqlConnection DBConn = new PsqlConnection(cnn.ToString());
            PsqlCommand comm = new PsqlCommand();

            try
            {

                DBConn.Open();
                comm.Connection = DBConn;
                comm.CommandText = "Update_Oper";
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.Clear();


                comm.Parameters.Add(":id_oper", PsqlDbType.Integer);
                comm.Parameters[comm.Parameters.Count - 1].Value = 1;

                comm.Parameters.Add(":Id_Seccion", PsqlDbType.Integer);
                comm.Parameters[comm.Parameters.Count - 1].Value = 1;


                comm.Parameters.Add(":parSalida", PsqlDbType.VarChar, 100);
                comm.Parameters[comm.Parameters.Count - 1].Direction = ParameterDirection.Output;

                comm.ExecuteNonQuery();

                var resp = comm.Parameters[":parSalida"].Value.ToString();

                return resp;

            }
            catch (PsqlException ex)
            {
                // Connection failed
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                DBConn.Close();
            }
        }
    }
}

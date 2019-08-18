using BnWinJAnrufmonitorApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BnWinJAnrufmonitorApi
{


    class Program
    {
        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
        private static string host = "localhost";
        private static string username = "sa";
        private static string password = "kwpsarix";
        private static string database = "BNWINS";
        private static string instance = "kwp";
        string phoneNumer;
        private static SqlConnection cnn;
        public static IntPtr hWnd;
        public static Boolean allowHiding = true;

        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                if (args[1].Contains("visible"))
                {
                    allowHiding = false;
                }
            }
            hWnd = GetConsoleWindow();
            cnn = DatabaseHandler.BuildConnecion(host, instance, username, password, database);
            try
            {
                if (allowHiding)
                {
                    //Thread.Sleep(5000);
                    //ShowWindow(hWnd, 0);
                }
                ////////////////////////////////////////
                /// Try to Connect to the Sql Server ///
                ////////////////////////////////////////
                //Console.WriteLine("Connecting to SQL Server");
                cnn.Open();
                //Console.WriteLine("!!!Connection Open!!!");
                LogfileHandler.Log("Sql Server Connected");

               
                //Console.Write("Press any key to exit... ");
                //Console.ReadKey();
               

                if (args.Length > 0 && args[0].Length > 5)
                {

                    Person person = DatabaseHandler.GetContact(cnn, args[0]);
                    if (person != null)
                    {
                        Console.WriteLine(new JObject(
                                 new JProperty("vorname", person.Vorname),
                                 new JProperty("nachname", person.Nachname),
                                 new JProperty("telefonnummer", person.Telefonnummer),
                                 new JProperty("ort", person.Ort),
                                 new JProperty("strasse", person.Strasse),
                                 new JProperty("plz", person.PLZ)
                       ).ToString());
                    }
                }

                ////////////////////////////////////
                /// Closing Sqlserver Connection ///
                ////////////////////////////////////
                cnn.Close();
                //Console.WriteLine("!!!Connection Closed!!!");
                LogfileHandler.Log("Sql Server disonnected");

            }
            catch (SqlException e)
            {
                ShowWindow(hWnd, 1);
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.ToString());
                //MessageBox.Show("Bitte lassen sie Dieses Fenster Geöffnet und informieren sie ihren Systemadministrator \n\n\n" + e.ToString(), "KWP -> JAnrufMonitor             Schwerwiegender Fehler!!! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cnn.Close();
                LogfileHandler.Log("Sql Error: " + e.ToString());
                Environment.Exit(1);
            }
            catch (ArgumentNullException e)
            {
                ShowWindow(hWnd, 1);
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.ToString());
                //MessageBox.Show("Bitte lassen sie Dieses Fenster Geöffnet und informieren sie ihren Systemadministrator \n\n\n" + e.ToString(), "KWP -> JAnrufMonitor            Schwerwiegender Fehler!!! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cnn.Close();
                LogfileHandler.Log("ArgumentNull Error: " + e.ToString());
                Environment.Exit(1);
            }
            catch (NullReferenceException e)
            {
                ShowWindow(hWnd, 1);
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.ToString());
                //MessageBox.Show("Bitte lassen sie Dieses Fenster Geöffnet und informieren sie ihren Systemadministrator \n\n\n" + e.ToString(), "KWP -> JAnrufMonitor             Schwerwiegender Fehler!!! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cnn.Close();
                LogfileHandler.Log("NullReference Error: " + e.ToString());
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                ShowWindow(hWnd, 1);
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.ToString());
                //MessageBox.Show("Bitte lassen sie Dieses Fenster Geöffnet und informieren sie ihren Systemadministrator \n\n\n" + e.ToString(), "KWP -> JAnrufMonitor            Schwerwiegender Fehler!!! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cnn.Close();
                LogfileHandler.Log("Error: " + e.ToString());
                Environment.Exit(1);
            }

        }
    }
    
}
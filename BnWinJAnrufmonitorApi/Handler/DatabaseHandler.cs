using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace BnWinJAnrufmonitorApi
{
    class DatabaseHandler
    {

        public static SqlConnection BuildConnecion(String host, String instance, String username, String password, String database)
        {
            ///////////////////////////////
            /// Creating Sql connection ///
            ///////////////////////////////
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = host + "\\" + instance;
            builder.UserID = username;
            builder.Password = password;
            builder.InitialCatalog = database;
            SqlConnection cnn = new SqlConnection(builder.ConnectionString);
            LogfileHandler.Log("Connectionstring: " + builder.ConnectionString);
            return cnn;
        }

        public static Models.Person GetContact(SqlConnection cnn, string phoneNumber)
        {
            List<Models.Person> person = new List<Models.Person>();
            SqlCommand queryCommand;
            SqlDataReader queryReader;
            phoneNumber = phoneNumber.Replace(" ", "");
            phoneNumber = phoneNumber.Replace("-", "");
            phoneNumber = phoneNumber.Replace("/", "");
            string shortNumber = phoneNumber.Substring(5);

            //////////////////////////////////////////////
            /// Building and Executing the Sql String ///
            ////////////////////////////////////////////
            queryCommand = new SqlCommand(null, cnn);
            queryCommand.CommandText = "Select * From dbo.adrKontakte Where TelNrN LIKE @telNr;";
            queryCommand.Parameters.Add("@telNr", SqlDbType.VarChar, shortNumber.Length+2).Value = "%"+shortNumber+"%";
            queryCommand.Prepare();
            queryReader = queryCommand.ExecuteReader();
            //Console.WriteLine("Reading dbo.adrKontakte AdrTel");
            LogfileHandler.Log("Reading dbo.adrKontakte AdrTel");
            ///////////////////////////////////
            /// Reading Sql Query Response ///
            /////////////////////////////////

            
            while (queryReader.Read())
            {
                
                string phoneNumberTemp = queryReader["TelNrN"].ToString();
                phoneNumberTemp = phoneNumberTemp.Replace(" ", "");
                phoneNumberTemp = phoneNumberTemp.Replace("-", "");
                phoneNumberTemp = phoneNumberTemp.Replace("/", "");
                if (phoneNumberTemp == phoneNumber)
                {
                    person.Add(new Models.Person {
                        AdrNrGes = queryReader["AdrNrGes"].ToString(),
                        Telefonnummer = phoneNumber
                    });
                    LogfileHandler.Log(person.ToString());
                    break;
                }
            }
            ////////////////////////////////////
            /// Closing Query Return Reader ///
            //////////////////////////////////
            queryReader.Close();

            if(person.Count == 0)
            {
                return null;
            }

            //////////////////////////////////////////////
            /// Building and Executing the Sql String ///
            ////////////////////////////////////////////
            queryCommand = new SqlCommand(null, cnn);
            queryCommand.CommandText = "Select * From dbo.adrAdressen Where AdrNrGes = @AdrNrGes;";
            queryCommand.Parameters.Add("@AdrNrGes", SqlDbType.VarChar, person[0].AdrNrGes.Length).Value = person[0].AdrNrGes;
            queryCommand.Prepare();
            queryReader = queryCommand.ExecuteReader();
            //Console.WriteLine("Reading dbo.adrAdressen ProjAdr");
            LogfileHandler.Log("Reading dbo.adrAdressen ProjAdr");

            ///////////////////////////////////
            /// Reading Sql Query Response ///
            /////////////////////////////////
            while (queryReader.Read())
            {
                person[0].Nachname = queryReader["Name"].ToString();
                person[0].Vorname = queryReader["Vorname"].ToString();
                person[0].Strasse = queryReader["Strasse"].ToString();
                person[0].OrtID = (int)queryReader["Ort"];
            }
            //closing reader
            queryReader.Close();

            //Get all City Values for all Projects from the Database

            //////////////////////////////////////////////
            /// Building and Executing the Sql String ///
            ////////////////////////////////////////////
            queryCommand = new SqlCommand(null, cnn);
            queryCommand.CommandText = "Select * From dbo.adrOrte Where OrtID = @OrtID;";

            queryCommand.Parameters.Add("@OrtID", SqlDbType.Int, person[0].OrtID.ToString().Length).Value = person[0].OrtID;
            queryCommand.Prepare();
            queryReader = queryCommand.ExecuteReader();

            //Console.WriteLine("Reading dbo.adrOrte");
            LogfileHandler.Log("Reading dbo.adrOrte");
            ///////////////////////////////////
            /// Reading Sql Query Response ///
            /////////////////////////////////
            while (queryReader.Read())
            {
                person[0].PLZ = queryReader["PLZ"].ToString();
                person[0].Ort = queryReader["Ort"].ToString();
            }
            ////////////////////////////////////
            /// Closing Query Return Reader ///
            //////////////////////////////////
            queryReader.Close();
            if (person.Count <= 1)
            {
                return person[0];
            }
            else
            {
                return null;
            }
        }
    }
}

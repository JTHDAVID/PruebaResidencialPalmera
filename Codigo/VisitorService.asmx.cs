using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace PruebaResidencialPalmera.Codigo
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class VisitorService : WebService
    {
        [WebMethod]
        public List<VisitorItem> GetVisitorNames(string prefixText)
        {
            List<VisitorItem> visitorNames = new List<VisitorItem>();
            string connectionString = ConfigurationManager.ConnectionStrings["Conexion_sqlserver"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT ID_Visitante, Nombre FROM [MANT].[Visitantes] WHERE Nombre LIKE @prefixText", connection))
                {
                    command.Parameters.AddWithValue("@prefixText", "%" + prefixText + "%");
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        visitorNames.Add(new VisitorItem
                        {
                            ID_Visitante = (int)reader["ID_Visitante"],
                            Nombre = reader["Nombre"].ToString()
                        });
                    }
                }
            }
            return visitorNames;
        }

        [WebMethod]
        public VisitorDetails GetVisitorDetails(int idVisitante)
        {
            VisitorDetails details = new VisitorDetails();
            string connectionString = ConfigurationManager.ConnectionStrings["Conexion_sqlserver"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("MANT.SPVisitantes_Detalles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_Visitante", idVisitante);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        details.Identidad = reader["Identidad"].ToString();
                        details.Placa_Vehiculo = reader["Placa_Vehiculo"].ToString();
                        details.Cantidad_Personas = (int)reader["Cantidad_Personas"];
                        details.Comentarios = reader["Comentarios"].ToString();
                        details.Recurrente = (bool)reader["Recurrente"];
                    }
                }
            }
            return details;
        }

        public class VisitorItem
        {
            public int ID_Visitante { get; set; }
            public string Nombre { get; set; }
        }

        public class VisitorDetails
        {
            public string Identidad { get; set; }
            public string Placa_Vehiculo { get; set; }
            public int Cantidad_Personas { get; set; }
            public string Comentarios { get; set; }
            public bool Recurrente { get; set; }
        }
    }
}
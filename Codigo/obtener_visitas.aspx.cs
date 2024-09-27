using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace PruebaResidencialPalmera.Codigo
{
    public partial class obtener_visitas : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Obtener el ID del visitante de la URL
                string idVista = Request.QueryString["id_vista"];
                if (!string.IsNullOrEmpty(idVista))
                {
                    // Llamar al método para cargar los detalles del visitante
                    CargarDetallesVisitante(idVista);
                }
                else
                {
                    lblNombre.Text = "No se proporcionó un ID de visitante.";
                }
            }
        }

        private void CargarDetallesVisitante(string idVista)
        {
            string cadenaConexion = ConfigurationManager.ConnectionStrings["Conexion_sqlserver"].ConnectionString;

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand comando = new SqlCommand("MANT.SPVisitantes_Detalles", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddWithValue("@ID_Visitante", idVista);

                    conexion.Open();
                    SqlDataReader reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        // Asignar valores a las etiquetas
                        lblNombre.Text = reader["Nombre"].ToString();
                        lblIdentidad.Text = reader["Identidad"].ToString();
                        lblPlacaVehiculo.Text = reader["Placa_Vehiculo"].ToString();
                        lblCantidadPersonas.Text = reader["Cantidad_Personas"].ToString();
                        lblComentarios.Text = reader["Comentarios"].ToString();
                        lblRecurrente.Text = (bool)reader["Recurrente"] ? "Sí" : "No";
                    }
                    else
                    {
                        lblNombre.Text = "No se encontraron detalles para el ID proporcionado.";
                    }
                }
            }
        }
    }
}
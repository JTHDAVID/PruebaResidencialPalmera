using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using ZXing;

namespace PruebaResidencialPalmera.Codigo
{
    public partial class visitas : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Aquí puedes cargar los visitantes iniciales si es necesario
            }
        }

        protected void BtnRegistrar_Click(object sender, EventArgs e)
        {
            int idResidente = 0;
            int idVisitante;

            // Permitir ingreso manual del ID de visitante
            if (int.TryParse(ID_Visitante.Text, out idVisitante))
            {
                idResidente = idVisitante; // Reemplazamos con el ID_Visitante si se ha seleccionado
            }

            string identidadVisitante = ID_Visitante.Text;
            string nombreVisitante = Nombre.Text;
            string placaVehiculo = Placa_Vehiculo.Text;
            int cantidadPersonas;

            // Verificamos que la cantidad de personas sea un número
            if (!int.TryParse(Cantidad_Personas.Text, out cantidadPersonas))
            {
                lblMensaje.Text = "Error: La cantidad de personas debe ser un número.";
                return;
            }

            string comentario = Comentarios.Text;

            byte[] foto = null;
            if (Foto.HasFile)
            {
                // Leemos la imagen como un array de bytes
                using (BinaryReader br = new BinaryReader(Foto.PostedFile.InputStream))
                {
                    foto = br.ReadBytes(Foto.PostedFile.ContentLength);
                }
            }

            // Obtenemos la cadena de conexión desde el archivo de configuración
            string cadenaConexion = ConfigurationManager.ConnectionStrings["Conexion_sqlserver"].ConnectionString;

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                try
                {
                    // Abrimos la conexión a la base de datos
                    conexion.Open();
                    using (SqlCommand comando = new SqlCommand("TRANS.SPVisitas_Guardar", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        // Agregamos los parámetros necesarios al comando
                        comando.Parameters.AddWithValue("@ID_Residente", idResidente);
                        comando.Parameters.AddWithValue("@ID_Visitante", idVisitante);
                        comando.Parameters.AddWithValue("@Nombre", nombreVisitante);
                        comando.Parameters.AddWithValue("@Identidad", identidadVisitante);
                        comando.Parameters.AddWithValue("@Placa_Vehiculo", placaVehiculo);
                        comando.Parameters.AddWithValue("@Cantidad_Personas", cantidadPersonas);
                        comando.Parameters.AddWithValue("@Recurrente", Recurrente.Checked);
                        comando.Parameters.AddWithValue("@Vigencia_Dias", Recurrente.Checked ? (object)int.Parse(Vigencia_Dias.Text) : DBNull.Value);
                        comando.Parameters.AddWithValue("@Comentarios", comentario);

                        if (foto != null)
                        {
                            comando.Parameters.AddWithValue("@Foto", foto);
                        }
                        else
                        {
                            comando.Parameters.AddWithValue("@Foto", DBNull.Value);
                        }

                        // Creamos un parámetro de salida para la URL
                        SqlParameter urlParam = new SqlParameter("@URL", SqlDbType.NVarChar, 250)
                        {
                            Direction = ParameterDirection.Output
                        };
                        comando.Parameters.Add(urlParam);

                        // Ejecutamos el comando
                        comando.ExecuteNonQuery();

                        // Obtenemos la URL de la imagen generada
                        string url = urlParam.Value.ToString();

                        // Generamos el código QR
                        GenerateQrCode(url);
                    }

                    // Mensaje de éxito
                    lblMensaje.CssClass = "text-success";
                    lblMensaje.Text = "Datos guardados exitosamente.";
                    divBotonCompartir.Style["display"] = "block"; // Mostrar el botón de compartir
                }
                catch (Exception ex)
                {
                    // Mensaje de error
                    lblMensaje.CssClass = "text-danger";
                    lblMensaje.Text = "Error: " + ex.Message;
                }
            }
        }

        private void GenerateQrCode(string url)
        {
            // Configuración del generador de códigos QR
            var qrWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 200,
                    Width = 200
                }
            };

            // Generamos el código QR y lo convertimos en imagen
            var pixelData = qrWriter.Write(url);
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                        pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                using (var memoryStream = new MemoryStream())
                {
                    // Guardamos la imagen en un stream y convertimos a Base64
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    string base64String = Convert.ToBase64String(memoryStream.ToArray());
                    imgQrCode.ImageUrl = "data:image/png;base64," + base64String;
                    imgQrCode.Visible = true;
                }
            }
        }

        protected void BtCancelar_Click(object sender, EventArgs e)
        {
            // Reiniciamos los campos del formulario
            ID_Visitante.Text = string.Empty;
            Nombre.Text = string.Empty;
            Placa_Vehiculo.Text = string.Empty;
            Cantidad_Personas.Text = "0";
            Recurrente.Checked = false;
            Vigencia_Dias.Text = string.Empty;
            divDuracionVisita.Style["display"] = "none";
            Vigencia_Dias.Enabled = false;
            Comentarios.Text = string.Empty;
            imgQrCode.Visible = false;
            lblMensaje.Text = string.Empty;
            divBotonCompartir.Style["display"] = "none"; // Ocultar el botón de compartir
        }

        protected void VisitaRecurrente_CheckedChanged(object sender, EventArgs e)
        {
            // Mostramos u ocultamos el campo de duración de visita basado en el checkbox
            if (Recurrente.Checked)
            {
                divDuracionVisita.Style["display"] = "block";
                Vigencia_Dias.Enabled = true;
            }
            else
            {
                divDuracionVisita.Style["display"] = "none";
                Vigencia_Dias.Enabled = false;
            }
        }
    }
}

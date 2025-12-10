using System.Configuration;
using System.Data.SqlClient;

namespace POS_FeriaUniversitaria.AccesoDatos.Infraestructura
{
    /// <summary>
    /// Encapsula la forma de obtener conexiones a la BD.
    /// Lee la cadena de conexión desde Web.config del proyecto Web.
    /// </summary>
    public static class ConexionBD
    {
        public static SqlConnection ObtenerConexion()
        {
            // El nombre "BD_FeriaUniversitaria" debe coincidir con Web.config
            string cadena = ConfigurationManager
                .ConnectionStrings["BD_FeriaUniversitaria"]
                .ConnectionString;

            return new SqlConnection(cadena);
        }
    }
}

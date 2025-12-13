/*
Universidad Tecnológica de Panamá
Facultad de Ingeniería en Sistemas Computacionales
Licenciatura en Desarrollo y Gestión de Software

Asignatura - Desarrollo de Software IV

Proyecto Semestral - Mini POS para Feria Universitaria

Facilitador: Regis Rivera

Estudiante:
Julio Solís | 8-1011-1457

Grupo: 1GS222

Fecha de entrega: 16 de diciembre de 2025
II Semestre | II Año
*/

using System.Configuration;
using System.Data.SqlClient;

namespace POS_FeriaUniversitaria.AccesoDatos.Infraestructura
{
   /* Encapsula la forma de obtener conexiones a la BD.
      Lee la cadena de conexión desde Web.config del proyecto Web. */
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

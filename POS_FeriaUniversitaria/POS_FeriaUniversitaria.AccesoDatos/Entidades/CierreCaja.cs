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

using System;

namespace POS_FeriaUniversitaria.AccesoDatos.Entidades
{
    // Resumen diario de ventas (simulación de cierre de caja).
    public class CierreCaja
    {
        public int CierreId { get; set; }
        public DateTime FechaCierre { get; set; }
        public decimal TotalVentas { get; set; }
        public int CantidadTransacciones { get; set; }
    }
}

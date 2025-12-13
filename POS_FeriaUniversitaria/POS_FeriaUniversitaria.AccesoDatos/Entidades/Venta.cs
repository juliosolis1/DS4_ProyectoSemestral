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
using System.Collections.Generic;

namespace POS_FeriaUniversitaria.AccesoDatos.Entidades
{
    // Representa una venta realizada en la feria.
    public class Venta
    {
        public int VentaId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Observaciones { get; set; }

        // Lista de detalles de la venta
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}

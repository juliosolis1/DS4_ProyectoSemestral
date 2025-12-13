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
using POS_FeriaUniversitaria.AccesoDatos.Entidades;

namespace POS_FeriaUniversitaria.Servicios.Interfaces
{
    // Capa de orquestación/negocio para ventas.
    public interface IVentaService
    {
        int RegistrarVenta(Venta venta, List<DetalleVenta> detalles);
        List<Venta> VentasDelDia(DateTime fecha);
    }
}

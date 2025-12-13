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

namespace POS_FeriaUniversitaria.AccesoDatos.Repositorios
{
    // Contrato de persistencia para registrar ventas y consultar reportes.
    public interface IVentaRepository
    {
        /* Inserta la cabecera de venta y sus detalles en una transacción.
           También descuenta stock de Productos.
           Devuelve el Id autogenerado de la venta. */
        int CrearVenta(Venta venta, List<DetalleVenta> detalles);

        // Ventas (cabecera) de una fecha concreta (para reporte diario).
        List<Venta> ListarPorFecha(DateTime fecha);
    }
}

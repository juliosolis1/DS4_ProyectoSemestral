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
using POS_FeriaUniversitaria.AccesoDatos.Repositorios;
using POS_FeriaUniversitaria.Servicios.Interfaces;

namespace POS_FeriaUniversitaria.Servicios.Implementaciones
{
    /* Capa de Servicios (VentaService):
       - Aplica validaciones de negocio antes de registrar una venta.
       - Delegación al repositorio para guardar cabecera/detalles y actualizar inventario. */

    public class VentaService : IVentaService
    {
        // Repositorio de ventas: responsable de la persistencia y operaciones en BD (ADO.NET).
        private readonly IVentaRepository _repo;

    /* Instancia el repositorio concreto.
       En un proyecto más grande se reemplazaría por Inyección de Dependencias (DI). */
        public VentaService()
        {
            _repo = new VentaRepository();
        }

        /* Registra una venta aplicando validaciones mínimas:
           - Debe existir al menos un detalle (ítems vendidos).
           - El total debe ser mayor que cero.
            Si todo es válido, delega la transacción completa al repositorio. */

        public int RegistrarVenta(Venta venta, List<DetalleVenta> detalles)
        {
            if (detalles == null || detalles.Count == 0)
                throw new InvalidOperationException("La venta debe tener al menos un ítem.");

            if (venta.Total <= 0)
                throw new InvalidOperationException("El total de la venta debe ser mayor que cero.");

            return _repo.CrearVenta(venta, detalles);
        }

        public List<Venta> VentasDelDia(DateTime fecha) => _repo.ListarPorFecha(fecha);
    }
}

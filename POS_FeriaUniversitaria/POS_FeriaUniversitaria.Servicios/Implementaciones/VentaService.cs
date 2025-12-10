using System;
using System.Collections.Generic;
using POS_FeriaUniversitaria.AccesoDatos.Entidades;
using POS_FeriaUniversitaria.AccesoDatos.Repositorios;
using POS_FeriaUniversitaria.Servicios.Interfaces;

namespace POS_FeriaUniversitaria.Servicios.Implementaciones
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _repo;

        public VentaService()
        {
            // Igual que ProductoService, sin DI formal
            _repo = new VentaRepository();
        }

        /// <summary>
        /// Valida reglas simples (no total 0, no detalles vacíos) y delega al repo.
        /// </summary>
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

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

using System.Collections.Generic;
using System.Linq;

namespace POS_FeriaUniversitaria.Web.Models
{
    // Carrito completo (lista + total).
    public class CarritoViewModel
    {
        public List<CarritoItemViewModel> Items { get; set; } = new List<CarritoItemViewModel>();

        public decimal Total => Items.Sum(i => i.Subtotal);
    }
}

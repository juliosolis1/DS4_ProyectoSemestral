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

using System.ComponentModel.DataAnnotations;

namespace POS_FeriaUniversitaria.Web.Models
{
    // Ítem del carrito en sesión (con datos de producto para mostrar).
    public class CarritoItemViewModel
    {
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string Nombre { get; set; }

        [Display(Name = "Imagen")]
        public string ImagenPortada { get; set; }

        [Display(Name = "Precio")]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}

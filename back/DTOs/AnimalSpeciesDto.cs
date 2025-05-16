using System;

namespace back.DTOs
{
    public class AnimalSpeciesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // Nombre de la especie (perro, gato, ave, etc.)
        public string Description { get; set; } = null!; // Descripción de la especie
        public bool IsActive { get; set; }
        public string CommonIssues { get; set; } = string.Empty; // Problemas comunes de salud
        public string CareInstructions { get; set; } = string.Empty; // Instrucciones de cuidado
        public DateTime CreatedDate { get; set; }
    }
}

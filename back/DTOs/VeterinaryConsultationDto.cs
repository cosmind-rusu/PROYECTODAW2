using System;

namespace back.DTOs
{
    public class VeterinaryConsultationDto
    {
        public int Id { get; set; }
        public int AnimalSpeciesId { get; set; } // Especie del animal
        public int TreatmentId { get; set; } // Tratamiento aplicado
        public string PetName { get; set; } = null!; // Nombre de la mascota
        public string OwnerName { get; set; } = null!; // Nombre del dueño
        public string ContactInfo { get; set; } = null!; // Información de contacto
        public decimal Cost { get; set; } // Costo de la consulta
        public string Description { get; set; } = null!; // Problema/diagnóstico
        public string TreatmentNotes { get; set; } = string.Empty; // Notas sobre el tratamiento
        public string Prescription { get; set; } = string.Empty; // Receta/medicamentos
        public DateTime ConsultationDate { get; set; } // Fecha de la consulta
        public DateTime? FollowUpDate { get; set; } // Fecha de seguimiento
        
        // Propiedades auxiliares para UI
        public string? AnimalSpeciesName { get; set; }
        public string? TreatmentName { get; set; }
    }
}

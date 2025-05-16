using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class VeterinaryConsultation
    {
        public int Id { get; set; }
        public int AnimalSpeciesId { get; set; } // Especie del animal
        public AnimalSpecies? AnimalSpecies { get; set; } // Relación con la especie del animal
        public int TreatmentId { get; set; } // Tratamiento aplicado
        public string PetName { get; set; } = null!; // Nombre de la mascota
        public string OwnerName { get; set; } = null!; // Nombre del dueño
        public string ContactInfo { get; set; } = null!; // Información de contacto
        public decimal Cost { get; set; } // Costo de la consulta
        public string Description { get; set; } = null!; // Descripción del problema/diagnóstico
        public string TreatmentNotes { get; set; } = string.Empty; // Notas sobre el tratamiento
        public string Prescription { get; set; } = string.Empty; // Receta/medicamentos
        public DateTime ConsultationDate { get; set; } // Fecha de la consulta
        public DateTime? FollowUpDate { get; set; } // Fecha de seguimiento si es necesaria
        public string UserId { get; set; } = null!; // ID del veterinario
        public IdentityUser? User { get; set; } // Usuario (veterinario)
        public Treatment? Treatment { get; set; } // Representa el tratamiento
    }
}

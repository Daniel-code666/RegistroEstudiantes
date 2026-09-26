using System.ComponentModel.DataAnnotations;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudiante.Application.Subjects.Dtos;

public class SubjectRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int? ProfessorId { get; set; }

    public int Credits { get; set; } = AcademicRules.CreditsPerSubject;

    public void Validate()
    {
        ValidationUtilities.ValidateObject(this);
        if (Credits != AcademicRules.CreditsPerSubject)
            throw new Common.ValidationException("Cada materia debe tener 3 créditos.");
    }
}

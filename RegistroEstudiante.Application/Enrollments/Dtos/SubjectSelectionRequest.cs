using System.ComponentModel.DataAnnotations;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Domain.Constants;

namespace RegistroEstudiante.Application.Enrollments.Dtos;

public class SubjectSelectionRequest
{
    [Required]
    public List<int> SubjectIds { get; set; } = [];

    public void Validate()
    {
        ValidationUtilities.ValidateObject(this);
        if (SubjectIds.Count > AcademicRules.MaxSubjectsPerStudent)
            throw new Common.ValidationException("El estudiante puede seleccionar máximo 3 materias.");
        if (SubjectIds.Any(x => x <= 0))
            throw new Common.ValidationException("Los identificadores de las materias deben ser positivos.");
        if (SubjectIds.Distinct().Count() != SubjectIds.Count)
            throw new Common.ValidationException("No se puede seleccionar una materia más de una vez.");
    }
}

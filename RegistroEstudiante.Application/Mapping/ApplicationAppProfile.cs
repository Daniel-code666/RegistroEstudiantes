using AutoMapper;
using RegistroEstudiante.Application.Users.Dtos;
using RegistroEstudiante.Domain.Entities;
using RegistroEstudiante.Application.Roles;
using RegistroEstudiante.Application.Subjects.Dtos;
using RegistroEstudiante.Application.Enrollments.Dtos;

namespace RegistroEstudiante.Application.Mapping
{
    public class ApplicationAppProfile : Profile
    {
        public ApplicationAppProfile()
        {
            CreateMap<Subject, StudentSubject>()
                .ForMember(x => x.SubjectId, o => o.MapFrom(x => x.Id))
                .ForMember(x => x.UserId, o => o.Ignore())
                .ForMember(x => x.User, o => o.Ignore())
                .ForMember(x => x.Subject, o => o.Ignore())
                .ForMember(x => x.CreationDate, o => o.Ignore())
                .ForMember(x => x.UpdatedDate, o => o.Ignore());
            CreateMap<RoleRequest, Role>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name.Trim()))
                .ForMember(x => x.Description, o => o.MapFrom(x => x.Description.Trim()))
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Active, o => o.Ignore())
                .ForMember(x => x.CreationDate, o => o.Ignore())
                .ForMember(x => x.UpdatedDate, o => o.Ignore());
            CreateMap<Role, RoleResponse>();

            CreateMap<SubjectRequest, Subject>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name.Trim()))
                .ForMember(x => x.Description, o => o.MapFrom(x => x.Description.Trim()))
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Active, o => o.Ignore())
                .ForMember(x => x.Professor, o => o.Ignore())
                .ForMember(x => x.StudentSubjects, o => o.Ignore())
                .ForMember(x => x.CreationDate, o => o.Ignore())
                .ForMember(x => x.UpdatedDate, o => o.Ignore());
            CreateMap<Subject, SubjectResponse>()
                .ForMember(x => x.ProfessorName, o => o.MapFrom(x => x.Professor == null ? "Sin profesor" : x.Professor.Name + " " + x.Professor.LastName));

            CreateMap<Subject, StudentSubjectResponse>();
            CreateMap<User, ClassmateResponse>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name + " " + x.LastName));
            CreateMap<User, StudentRecordResponse>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name + " " + x.LastName))
                .ForMember(x => x.Subjects, o => o.MapFrom(x => x.StudentSubjects
                    .Where(y => y.Subject.Active).OrderBy(y => y.SubjectId).Select(y => y.Subject)));

            CreateMap<UserFieldsRequest, User>()
                .ForMember(x => x.Name, o => o.MapFrom(x => x.Name.Trim()))
                .ForMember(x => x.LastName, o => o.MapFrom(x => x.LastName.Trim()))
                .ForMember(x => x.Email, o => o.MapFrom(x => x.Email.Trim()))
                .ForMember(x => x.NormalizedEmail, o => o.MapFrom(x => x.Email.Trim().ToUpperInvariant()))
                .ForMember(x => x.IdentificationNumber, o => o.MapFrom(x => x.IdentificationNumber.Trim()))
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.RoleId, o => o.Ignore())
                .ForMember(x => x.Role, o => o.Ignore())
                .ForMember(x => x.PasswordHash, o => o.Ignore())
                .ForMember(x => x.TokenVersion, o => o.Ignore())
                .ForMember(x => x.Active, o => o.Ignore())
                .ForMember(x => x.CreationDate, o => o.Ignore())
                .ForMember(x => x.UpdatedDate, o => o.Ignore())
                .ForMember(x => x.StudentSubjects, o => o.Ignore())
                .ForMember(x => x.TaughtSubjects, o => o.Ignore())
                .ForMember(x => x.Subjects, o => o.Ignore());

            CreateMap<RegisterRequest, User>().IncludeBase<UserFieldsRequest, User>();
            CreateMap<CreateUserRequest, User>().IncludeBase<RegisterRequest, User>();
            CreateMap<UpdateUserRequest, User>().IncludeBase<UserFieldsRequest, User>();
            CreateMap<UpdateProfileRequest, User>().IncludeBase<UserFieldsRequest, User>();

            CreateMap<User, UserDetailsResponse>().IncludeBase<User, UserResponse>();

            CreateMap<User, UserResponse>()
                .ForMember(x => x.Role, o => o.MapFrom(x => x.Role.Name));
        }
    }
}

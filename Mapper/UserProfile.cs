using AutoMapper;
using Redatech.Dto;
using Redatech.Models;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UsuarioDto>().ReverseMap();
        CreateMap<Essay, RedacaoDto>().ReverseMap();
        CreateMap<Correction, CorrecaoDto>().ReverseMap();
        CreateMap<Class, TurmaDto>().ReverseMap();

        // Novo mapeamento: TurmaModel -> TurmaComAlunosDto
        CreateMap<Class, TurmaComAlunosDto>()
            .ForMember(dest => dest.Alunos, opt => opt.MapFrom(src =>
                src.ClassStudents.Select(ta => ta.Student)));

        // Novo mapeamento: UsuarioModel -> AlunoNaTurmaDto
        CreateMap<User, AlunoNaTurmaDto>()
            .ForMember(dest => dest.DataVinculo, opt => opt.Ignore()); // vamos preencher no service
    }
}

using AutoMapper;
using Redatech.Dto;
using Redatech.Models;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<User, UsuarioDto>().ReverseMap();
        CreateMap<Essay, RedacaoDto>().ReverseMap();
        CreateMap<Correction, CorrecaoDto>().ReverseMap();
        CreateMap<Class, TurmaDto>().ReverseMap();

        // Novo mapeamento: TurmaModel -> TurmaComAlunosDto
        CreateMap<Class, TurmaComAlunosDto>()
            .ForMember(dest => dest.Alunos, opt => opt.MapFrom(src =>
                src.TurmasAlunos.Select(ta => ta.Aluno)));

        // Novo mapeamento: UsuarioModel -> AlunoNaTurmaDto
        CreateMap<User, AlunoNaTurmaDto>()
            .ForMember(dest => dest.DataVinculo, opt => opt.Ignore()); // vamos preencher no service
    }
}

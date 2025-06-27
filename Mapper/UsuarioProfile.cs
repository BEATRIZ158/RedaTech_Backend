using AutoMapper;
using Redatech.Dto;
using Redatech.Enums;
using Redatech.Models;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<UsuarioModel, UsuarioDto>()
        .ForMember(dest => dest.TipoUsuario,
                   opt => opt.MapFrom(src => Enum.GetName(typeof(TipoUsuario), src.TipoUsuario))) // <-- aqui!
        .ReverseMap()
        .ForMember(dest => dest.TipoUsuario,
                   opt => opt.MapFrom(src => Enum.Parse<TipoUsuario>(src.TipoUsuario)));

        CreateMap<RedacaoModel, RedacaoDto>().ReverseMap();
        CreateMap<CorrecaoModel, CorrecaoDto>().ReverseMap();
        CreateMap<TurmaModel, TurmaDto>().ReverseMap();

        // Novo mapeamento: TurmaModel -> TurmaComAlunosDto
        CreateMap<TurmaModel, TurmaComAlunosDto>()
            .ForMember(dest => dest.Alunos, opt => opt.MapFrom(src =>
                src.TurmasAlunos.Select(ta => ta.Aluno)));

        // Novo mapeamento: UsuarioModel -> AlunoNaTurmaDto
        CreateMap<UsuarioModel, AlunoNaTurmaDto>()
            .ForMember(dest => dest.DataVinculo, opt => opt.Ignore()); // vamos preencher no service

        CreateMap<UsuarioModel, UsuarioLogadoDto>().ReverseMap();

        CreateMap<RefreshTokenDto, RefreshTokenModel>().ReverseMap();
    }
}

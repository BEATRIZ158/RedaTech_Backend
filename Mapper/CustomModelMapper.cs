using AutoMapper;

namespace Redatech.Mapper
{
    public class CustomModelMapper
    {
        private static readonly IMapper _mapper;

        /* */
        static CustomModelMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UsuarioProfile>(); // Adiciona o perfil do usuário
            });
            _mapper = config.CreateMapper();
        }

        //Método genérico para converter um objeto de um tipo para outro
        public static TDestination ParseObject<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TDestination>(source);
        }

        //Método para converter uma lista de objetos de um tipo para outro
        public static List<TDestination> ParseObjectList<TSource, TDestination>(List<TSource> sourceList)
        {
            return sourceList.Select(source => _mapper.Map<TDestination>(source)).ToList();
        }
    }
}

using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.UsuarioService
{
    public interface IUsuarioInterface
    {
        //Uma espécie de contrato, define os métodos que precisam ser implementados, a classe precisa respeitar esse contrato
        Task<ServiceResponse<List<UsuarioDto>>> GetUsuarios();//Lista usuários geral
        Task<ServiceResponse<List<UsuarioDto>>> CreateUsuario(UsuarioDto novoUsuario); //Cria novo usuario e retorna os dados dele
        Task<ServiceResponse<UsuarioDto>> GetUsuarioById(int id);//Lista usuário pelo ID
        Task<ServiceResponse<List<UsuarioDto>>> UpdateUsuario(UsuarioDto editadoUsuario);//Retorna usuários atualizados
        Task<ServiceResponse<List<UsuarioDto>>> InativaUsuario(int id);//Ativa/Inativa Usuario e retorna lista de usuários
        Task<ServiceResponse<List<UsuarioDto>>> DeleteUsuario(int id);//Deletar usuário (Não pretendo usar)
        Task<ServiceResponse<string>> LoginAsync(LoginDto loginDto);
    }
}

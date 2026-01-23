using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.UsuarioService
{
    public interface IUserService
    {
        Task<ServiceResponse<List<UsuarioDto>>> GetUsers();
        Task<ServiceResponse<List<UsuarioDto>>> CreateUser(UsuarioDto novoUsuario);
        Task<ServiceResponse<UsuarioDto>> GetUserById(int id);
        Task<ServiceResponse<List<UsuarioDto>>> UpdateUser(UsuarioDto editadoUsuario);
        Task<ServiceResponse<List<UsuarioDto>>> InactiveUser(int id);
        Task<ServiceResponse<List<UsuarioDto>>> DeleteUser(int id);
        Task<ServiceResponse<List<UsuarioDto>>> GetUserByName(string nomeParcial);
        Task<ServiceResponse<UsuarioLogadoDto>> Login(LoginDto loginDto);
    }
}

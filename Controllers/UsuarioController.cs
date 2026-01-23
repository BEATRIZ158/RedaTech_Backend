using Microsoft.AspNetCore.Mvc;
using Redatech.Dto;
using Redatech.Models;
using Redatech.Service.UsuarioService;

namespace Redatech.Controllers
{
    //Criando uma rota base
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUserService _usuarioInterface;

        public UsuarioController(IUserService usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }

        /// <summary>
        /// Retorna a lista de usuários
        /// </summary>
        /// <param name="">Não precisa passar nenhum valor</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> GetUsers()
        {
            var response = await _usuarioInterface.GetUsers();
            return Ok(response);
        }

        /// <summary>
        /// Buscar um usuário pelo seu Id
        /// </summary>
        /// <param name="id">Id do usuário que está sendo buscado</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<UsuarioDto>>> GetUserById(int id)
        {
            // Chama o Service para buscar o usuário
            var response = await _usuarioInterface.GetUserById(id);

            // Retorna o response do Service
            return Ok(response);
        }

        /// <summary>
        /// Criação de usuário
        /// </summary>
        /// <param name="novoUsuario">Dados do novo usuário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> CreateUser(UsuarioDto novoUsuario)
        {
            return Ok(await _usuarioInterface.CreateUser(novoUsuario));
        }

        /// <summary>
        /// Atualizar os dados do usuário
        /// </summary>
        /// <param name="editadoUsuario">Dados do usuário atualizados</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> UpdateUser(UsuarioDto editadoUsuario)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = await _usuarioInterface.UpdateUser(editadoUsuario);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Inativar ou Ativar usuário
        /// </summary>
        /// <param name="id">Id do Usuário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> InactiveUser(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = await _usuarioInterface.InactiveUser(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Deletar usuário
        /// </summary>
        /// <param name="id">Id do Usuário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> DeleteUsuario(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = await _usuarioInterface.DeleteUser(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Buscar usuário por caracter
        /// </summary>
        /// <param name="nomeParcial">Nome parcial do usuário.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("buscar-por-nome/{nomeParcial}")]
        public async Task<IActionResult> BuscarUsuariosPorNome(string nomeParcial)
        {
            var resposta = await _usuarioInterface.GetUserByName(nomeParcial);

            if (!resposta.Sucesso)
                return BadRequest(resposta.Mensagem);

            return Ok(resposta);
        }

        /// <summary>
        /// Faz login do usuário com email e senha.
        /// </summary>
        /// <param name="loginDto">Dados de login do usuário.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<UsuarioLogadoDto>>> Login([FromBody] LoginDto loginDto)
        {
            var resposta = await _usuarioInterface.Login(loginDto);
            if (!resposta.Sucesso)
                return BadRequest(resposta);

            return Ok(resposta);
        }
    }
}

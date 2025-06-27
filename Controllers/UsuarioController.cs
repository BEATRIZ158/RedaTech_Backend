using Microsoft.AspNetCore.Authorization;
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
        private readonly IUsuarioInterface _usuarioInterface;

        public UsuarioController(IUsuarioInterface usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }

        /// <summary>
        /// Retorna a lista de usuários
        /// </summary>
        /// <param name="">Não precisa passar nenhum valor</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> GetUsuarios()
        {
            var response = await _usuarioInterface.GetUsuarios();
            return Ok(response);
        }

        /// <summary>
        /// Buscar um usuário pelo seu Id
        /// </summary>
        /// <param name="id">Id do usuário que está sendo buscado</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<UsuarioDto>>> GetUsuarioById(int id)
        {
            // Chama o Service para buscar o usuário
            var response = await _usuarioInterface.GetUsuarioById(id);

            // Retorna o response do Service
            return Ok(response);
        }

        /// <summary>
        /// Criação de usuário
        /// </summary>
        /// <param name="novoUsuario">Dados do novo usuário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> CreateUsuario(UsuarioDto novoUsuario)
        {
            return Ok(await _usuarioInterface.CreateUsuario(novoUsuario));
        }

        /// <summary>
        /// Atualizar os dados do usuário
        /// </summary>
        /// <param name="editadoUsuario">Dados do usuário atualizados</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<UsuarioDto>>> UpdateUsuario(UsuarioDto editadoUsuario)
        {
            ServiceResponse<UsuarioDto> serviceResponse = await _usuarioInterface.UpdateUsuario(editadoUsuario);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Inativar ou Ativar usuário
        /// </summary>
        /// <param name="id">Id do Usuário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpPut("inativaUsuario/{id}")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> InativaUsuario(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = await _usuarioInterface.InativaUsuario(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Deletar usuário
        /// </summary>
        /// <param name="id">Id do Usuário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> DeleteUsuario(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = await _usuarioInterface.DeleteUsuario(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Buscar usuário por caracter
        /// </summary>
        /// <param name="nomeParcial">Nome parcial do usuário.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpGet("buscar-por-nome/{nomeParcial}")]
        public async Task<IActionResult> BuscarUsuariosPorNome(string nomeParcial)
        {
            var resposta = await _usuarioInterface.GetUsuariosByName(nomeParcial);

            if (!resposta.Sucesso)
                return BadRequest(resposta.Mensagem);

            return Ok(resposta);
        }

        /// <summary>
        /// Buscar usuários alunos por caractere
        /// </summary>
        /// <param name="nomeParcial">Nome parcial do usuário.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpGet("buscar-alunos-por-nome/{nomeParcial}")]
        public async Task<IActionResult> BuscarAlunosPorNome(string nomeParcial)
        {
            var resposta = await _usuarioInterface.GetAlunosByName(nomeParcial);

            if (!resposta.Sucesso)
                return BadRequest(resposta.Mensagem);

            return Ok(resposta);
        }

        [Authorize(Roles = "Professor")]
        [HttpPut("inativar-aluno/{id}")]
        public async Task<ActionResult<ServiceResponse<UsuarioDto>>> InativarAluno(int id)
        {
            ServiceResponse<UsuarioDto> serviceResponse = await _usuarioInterface.InativarAluno(id);
            return Ok(serviceResponse);
        }

        [Authorize(Roles = "Professor")]
        [HttpPut("ativar-aluno/{id}")]
        public async Task<ActionResult<ServiceResponse<UsuarioDto>>> AtivarAluno(int id)
        {
            ServiceResponse<UsuarioDto> serviceResponse = await _usuarioInterface.AtivarAluno(id);
            return Ok(serviceResponse);
        }

        [Authorize]
        [HttpGet("obter-id-logado")]
        public async Task<ActionResult<ServiceResponse<int>>> ObterIdUsuarioLogado()
        {
            return await _usuarioInterface.ObterIdDoUsuarioLogado();
        }
    }
}

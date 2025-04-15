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

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> GetUsuarios()
        {
            var response = await _usuarioInterface.GetUsuarios();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<UsuarioDto>>> GetUsuarioById(int id)
        {
            // Chama o Service para buscar o usuário
            var response = await _usuarioInterface.GetUsuarioById(id);

            // Retorna o response do Service
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> CreateUsuario(UsuarioDto novoUsuario)
        {
            return Ok(await _usuarioInterface.CreateUsuario(novoUsuario));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> UpdateUsuario(UsuarioDto editadoUsuario)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = await _usuarioInterface.UpdateUsuario(editadoUsuario);
            return Ok(serviceResponse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> InativaUsuario(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = await _usuarioInterface.InativaUsuario(id);
            return Ok(serviceResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> DeleteUsuario(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = await _usuarioInterface.DeleteUsuario(id);
            return Ok(serviceResponse);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] LoginDto loginDto)
        {
            var resposta = await _usuarioInterface.LoginAsync(loginDto);
            if (!resposta.Sucesso)
                return BadRequest(resposta);

            return Ok(resposta);
        }
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Redatech.Dto;
using Redatech.Models;
using Redatech.Service.TurmaService;

namespace Redatech.Controllers
{
    //Criando uma rota base
    [Route("api/[controller]")]
    [ApiController]
    public class TurmaController : ControllerBase
    {
        private readonly ITurmaInterface _turmaInterface;

        public TurmaController(ITurmaInterface turmaInterface)
        {
            _turmaInterface = turmaInterface;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> GetUsuarios()
        {
            var response = await _turmaInterface.GetTurmas();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<TurmaDto>>> GetTurmaById(int id)
        {
            var response = await _turmaInterface.GetTurmaById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> CreateTurma(TurmaDto novaTurmaDto)
        {
            return Ok(await _turmaInterface.CreateTurma(novaTurmaDto));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> UpdateTurma(TurmaDto editadoTurmaDto)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.UpdateTurma(editadoTurmaDto);
            return Ok(serviceResponse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> InativaTurma(int id)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.InativaTurma(id);
            return Ok(serviceResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> DeleteTurma(int id)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.DeleteTurma(id);
            return Ok(serviceResponse);
        }

        [HttpDelete("remover-aluno")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> RemoverUsuarioDaTurma(
            [FromQuery] int turmaId,
            [FromQuery] int alunoId)
        {
            var response = await _turmaInterface.RemoverAlunoDaTurma(turmaId, alunoId);
            return Ok(response);
        }

        [HttpPost("adicionar-aluno")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> AdicionarAlunoNaTurma(
            [FromQuery] int turmaId,
            [FromQuery] int alunoId)
        {
            var resposta = await _turmaInterface.AdicionarAlunoNaTurma(turmaId, alunoId);
            return Ok(resposta);
        }

        [HttpGet("ListarAlunosDaTurma/{turmaId}")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> ListarAlunosDaTurma(int turmaId)
        {
            var response = await _turmaInterface.ListarAlunosDaTurma(turmaId);
            return Ok(response);
        }
    }
}

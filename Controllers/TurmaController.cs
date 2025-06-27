using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Listando todas as turma
        /// </summary>
        /// <param name="">Nenhum parametro é necessário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpGet("listar-todas-turmas")]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> GetTurmas()
        {
            var response = await _turmaInterface.GetTurmas();
            return Ok(response);
        }

        /// <summary>
        /// Listando uma turma pelo id
        /// </summary>
        /// <param name="id">Passa o id da turma a ser buscada</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<TurmaDto>>> GetTurmaById(int id)
        {
            var response = await _turmaInterface.GetTurmaById(id);
            return Ok(response);
        }

        /// <summary>
        /// Criando uma nova turma
        /// </summary>
        /// <param name="novaTurmaDto">Passa os dados da nova turma</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> CreateTurma(TurmaDto novaTurmaDto)
        {
            return Ok(await _turmaInterface.CreateTurma(novaTurmaDto));
        }

        /// <summary>
        /// Editando uma turma
        /// </summary>
        /// <param name="editadoTurmaDto">Passa os dados atualizados da turma</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> UpdateTurma(TurmaDto editadoTurmaDto)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.UpdateTurma(editadoTurmaDto);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Inativa/Ativa uma turma
        /// </summary>
        /// <param name="id">Passa o id da turma a ser ativada/inativada</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> InativaTurma(int id)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.InativaTurma(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Deleta a turma
        /// </summary>
        /// <param name="id">Passa o id da turma.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> DeleteTurma(int id)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.DeleteTurma(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Remover aluno da turma
        /// </summary>
        /// <param name="turmaId">Passa o id da turma.</param>
        /// <param name="alunoId">Passa o id do aluno.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpDelete("remover-aluno")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> RemoverUsuarioDaTurma(
            [FromQuery] int turmaId,
            [FromQuery] int alunoId)
        {
            var response = await _turmaInterface.RemoverAlunoDaTurma(turmaId, alunoId);
            return Ok(response);
        }

        /// <summary>
        /// Listar alunos da turma
        /// </summary>
        /// <param name="turmaId">Passa o id da turma.</param>
        /// <param name="alunoId">Passa o id do aluno.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpPost("adicionar-aluno")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> AdicionarAlunoNaTurma(
            [FromQuery] int turmaId,
            [FromQuery] int alunoId)
        {
            var resposta = await _turmaInterface.AdicionarAlunoNaTurma(turmaId, alunoId);
            return Ok(resposta);
        }

        /// <summary>
        /// Listar alunos da turma
        /// </summary>
        /// <param name="turmaId">Passa o id da turma.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpGet("ListarAlunosDaTurma/{turmaId}")]
        public async Task<ActionResult<ServiceResponse<List<AlunoNaTurmaDto>>>> ListarAlunosDaTurma(int turmaId)
        {
            var response = await _turmaInterface.ListarAlunosDaTurma(turmaId);
            return Ok(response);
        }

        [Authorize(Roles = "Professor")]
        [HttpGet("ListarAlunosForaDaTurma/{turmaId}")]
        public async Task<ActionResult<ServiceResponse<List<AlunoForaDaTurmaDto>>>> ListarAlunosForaDaTurma(int turmaId)
        {
            var response = await _turmaInterface.ListarAlunosForaDaTurma(turmaId);
            return Ok(response);
        }

        /// <summary>
        /// Buscar turma por caracter
        /// </summary>
        /// <param name="nomeTurmaParcial">Nome parcial da turma.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Professor")]
        [HttpGet("buscar-por-nome/{nomeTurmaParcial}")]
        public async Task<IActionResult> BuscarTurmasPorNome(string nomeTurmaParcial)
        {
            var resposta = await _turmaInterface.GetTurmasByName(nomeTurmaParcial);

            if (!resposta.Sucesso)
                return BadRequest(resposta.Mensagem);

            return Ok(resposta);
        }

        [Authorize(Roles = "Professor")]
        [HttpGet("listar-todos-alunos")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> GetAlunosSalvos()
        {
            var response = await _turmaInterface.ListarAlunosSalvos();
            return Ok(response);
        }
    }
}

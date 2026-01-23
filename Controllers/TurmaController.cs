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
        private readonly IClassService _turmaInterface;

        public TurmaController(IClassService turmaInterface)
        {
            _turmaInterface = turmaInterface;
        }

        /// <summary>
        /// Listando todas as turma
        /// </summary>
        /// <param name="">Nenhum parametro é necessário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> GetUsuarios()
        {
            var response = await _turmaInterface.GetClasses();
            return Ok(response);
        }

        /// <summary>
        /// Listando uma turma pelo id
        /// </summary>
        /// <param name="id">Passa o id da turma a ser buscada</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<TurmaDto>>> GetClassById(int id)
        {
            var response = await _turmaInterface.GetClassById(id);
            return Ok(response);
        }

        /// <summary>
        /// Criando uma nova turma
        /// </summary>
        /// <param name="novaTurmaDto">Passa os dados da nova turma</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> CreateTurma(TurmaDto novaTurmaDto)
        {
            return Ok(await _turmaInterface.CreateClass(novaTurmaDto));
        }

        /// <summary>
        /// Editando uma turma
        /// </summary>
        /// <param name="editadoTurmaDto">Passa os dados atualizados da turma</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> UpdateClass(TurmaDto editadoTurmaDto)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.UpdateClass(editadoTurmaDto);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Inativa/Ativa uma turma
        /// </summary>
        /// <param name="id">Passa o id da turma a ser ativada/inativada</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> InactiveClass(int id)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.InactiveClass(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Deleta a turma
        /// </summary>
        /// <param name="id">Passa o id da turma.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<TurmaDto>>>> DeleteClass(int id)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = await _turmaInterface.DeleteClass(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Remover aluno da turma
        /// </summary>
        /// <param name="turmaId">Passa o id da turma.</param>
        /// <param name="alunoId">Passa o id do aluno.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpDelete("remover-aluno")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> RemoveStudentToClass(
            [FromQuery] int turmaId,
            [FromQuery] int alunoId)
        {
            var response = await _turmaInterface.RemoveStudentToClass(turmaId, alunoId);
            return Ok(response);
        }

        /// <summary>
        /// Listar alunos da turma
        /// </summary>
        /// <param name="turmaId">Passa o id da turma.</param>
        /// <param name="alunoId">Passa o id do aluno.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost("adicionar-aluno")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> AddStudentToClass(
            [FromQuery] int turmaId,
            [FromQuery] int alunoId)
        {
            var resposta = await _turmaInterface.AddStudentToClass(turmaId, alunoId);
            return Ok(resposta);
        }

        /// <summary>
        /// Listar alunos da turma
        /// </summary>
        /// <param name="turmaId">Passa o id da turma.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("ListarAlunosDaTurma/{turmaId}")]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDto>>>> ListStudentsToClass(int turmaId)
        {
            var response = await _turmaInterface.ListStudentsToClass(turmaId);
            return Ok(response);
        }

        /// <summary>
        /// Buscar turma por caracter
        /// </summary>
        /// <param name="nomeTurmaParcial">Nome parcial da turma.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("buscar-por-nome/{nomeTurmaParcial}")]
        public async Task<IActionResult> GetClassesByName(string nomeTurmaParcial)
        {
            var resposta = await _turmaInterface.GetClassesByName(nomeTurmaParcial);

            if (!resposta.Sucesso)
                return BadRequest(resposta.Mensagem);

            return Ok(resposta);
        }
    }
}

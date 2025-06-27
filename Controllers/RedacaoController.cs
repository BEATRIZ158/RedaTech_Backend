using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redatech.Dto;
using Redatech.Models;
using Redatech.Service.RedacaoService;

namespace Redatech.Controllers
{
    //Criando uma rota base
    [Route("api/[controller]")]
    [ApiController]
    public class RedacaoController : ControllerBase
    {
        private readonly IRedacaoInterface _redacaoInterface;

        public RedacaoController(IRedacaoInterface redacaoInterface)
        {
            _redacaoInterface = redacaoInterface;
        }

        /// <summary>
        /// Listando todas as redações
        /// </summary>
        /// <param name="">Nenhum parametro é necessário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> GetRedacoes()
        {
            var response = await _redacaoInterface.GetRedacoes();
            return Ok(response);
        }

        /// <summary>
        /// Buscando a redação pelo Id
        /// </summary>
        /// <param name="id">Id da redação que está sendo buscada</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<RedacaoDto>>> GetRedacaoById(int id)
        {
            var response = await _redacaoInterface.GetRedacaoById(id);

            return Ok(response);
        }

        /// <summary>
        /// Atualizar redação 
        /// </summary>
        /// <param name="redacaoDto">Passar os dados atualizados da Redação</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Aluno")]
        [HttpPut("atualizar")]
        public async Task<ActionResult<ServiceResponse<RedacaoDto>>> UpdateRedacao(
            [FromForm] RedacaoDto redacaoDto, IFormFile? novoArquivo)
        {
            var resposta = await _redacaoInterface.UpdateRedacaoAsync(redacaoDto, novoArquivo);
            return Ok(resposta);
        }

        /// <summary>
        /// Deleta a Redação 
        /// </summary>
        /// <param name="id">Passar o id da Redação</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Aluno")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> DeleteRedacao(int id)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = await _redacaoInterface.DeleteRedacao(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Buscar redacoes por titulo parcial 
        /// </summary>
        /// <param titulo="tituloParcial">Passar o titulo parcial da redação</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize]
        [HttpGet("buscar-por-titulo/{tituloParcial}")]
        public async Task<IActionResult> BuscarRedacoesPorTitulo(string tituloParcial)
        {
            var resposta = await _redacaoInterface.GetRedacoesByTitulo(tituloParcial);

            if (!resposta.Sucesso)
                return BadRequest(resposta.Mensagem);

            return Ok(resposta);
        }

        /// <summary>
        /// Buscar redacoes por titulo parcial 
        /// </summary>
        /// <param redacao="dto">Passar o titulo parcial da redação</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [Authorize(Roles = "Aluno")]
        [HttpPost("enviar-redacao")]
        public async Task<IActionResult> EnviarRedacao([FromForm] RedacaoUploadDto dto)
        {
            var response = await _redacaoInterface.CreateRedacaoComUpload(dto);

            if (!response.Sucesso)
                return BadRequest(response.Mensagem);

            return Ok(response);
        }

        /// <summary>
        /// Listar redações por turma
        /// </summary>
        /// <param idturma="idTurma">Passar o id da Redação</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("listar-por-turma/{idTurma}")]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> ListarRedacoesPorTurma(int idTurma)
        {
            var response = await _redacaoInterface.ListarRedacoesPorTurma(idTurma);
            return Ok(response);
        }


        /// <summary>
        /// Listar redações com correção
        /// </summary>
        /// <param idturma="idTurma">Passar o id da Turma</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("listar-com-correcao/{idTurma}")]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> ListarRedacoesComCorrecao(int idTurma)
        {
            var response = await _redacaoInterface.ListarRedacoesComCorrecao(idTurma);
            return Ok(response);
        }

        /// <summary>
        /// Listar redações sem correção
        /// </summary>
        /// <param idturma="idTurma">Passar o id da Turma</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("listar-sem-correcao/{idTurma}")]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> ListarRedacoesSemCorrecao(int idTurma)
        {
            var response = await _redacaoInterface.ListarRedacoesSemCorrecao(idTurma);
            return Ok(response);
        }
    }
}

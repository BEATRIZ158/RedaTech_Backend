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
        private readonly IEssayService _redacaoInterface;

        public RedacaoController(IEssayService redacaoInterface)
        {
            _redacaoInterface = redacaoInterface;
        }

        /// <summary>
        /// Listando todas as redações
        /// </summary>
        /// <param name="">Nenhum parametro é necessário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> GetEssays()
        {
            var response = await _redacaoInterface.GetEssays();
            return Ok(response);
        }

        /// <summary>
        /// Buscando a redação pelo Id
        /// </summary>
        /// <param name="id">Id da redação que está sendo buscada</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<RedacaoDto>>> GetEssayById(int id)
        {
            var response = await _redacaoInterface.GetEssayById(id);

            return Ok(response);
        }

        /// <summary>
        /// Atualizar redação 
        /// </summary>
        /// <param name="redacaoDto">Passar os dados atualizados da Redação</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPut("atualizar")]
        public async Task<ActionResult<ServiceResponse<RedacaoDto>>> UpdateEssay(
            [FromForm] RedacaoDto redacaoDto, IFormFile? novoArquivo)
        {
            var resposta = await _redacaoInterface.UpdateEssayAsync(redacaoDto, novoArquivo);
            return Ok(resposta);
        }

        /// <summary>
        /// Deleta a Redação 
        /// </summary>
        /// <param name="id">Passar o id da Redação</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> DeleteEssay(int id)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = await _redacaoInterface.DeleteEssay(id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Salva a redação no sistema 
        /// </summary>
        /// <param name="redacaoDto">Passe os dados padrão da redação, Id e Caminho do arquivo não precisa passar, pois são gerados automaticamente!</param>
        /// <param name="arquivo">Passar o arquivo que contém o texto</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost("enviar-redacao")]
        public async Task<IActionResult> EnviarRedacao([FromForm] RedacaoDto redacaoDto, IFormFile arquivo)
        {
            var uploadResponse = await _redacaoInterface.UploadFileEssay(arquivo);

            if (!uploadResponse.Sucesso)
                return BadRequest(uploadResponse.Mensagem);

            redacaoDto.CaminhoArquivo = uploadResponse.Dados;

            var createResponse = await _redacaoInterface.CreateEssay(redacaoDto);

            if (!createResponse.Sucesso)
                return BadRequest(createResponse.Mensagem);

            return Ok(createResponse);
        }
    }
}

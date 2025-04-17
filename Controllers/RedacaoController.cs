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
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<RedacaoDto>>> GetRedacaoById(int id)
        {
            var response = await _redacaoInterface.GetRedacaoById(id);

            return Ok(response);
        }

        //[HttpPost]
        //public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> CreateRedacao(RedacaoDto novaRedacao)
        //{
         //   return Ok(await _redacaoInterface.CreateRedacao(novaRedacao));
        ///}

        /// <summary>
        /// Atualiza a redação 
        /// </summary>
        /// <param name="editadaRedacaoDto">Dados da redação atualizada</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> UpdateRedacao(RedacaoDto editadaRedacaoDto)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = await _redacaoInterface.UpdateRedacao(editadaRedacaoDto);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Deleta a Redação 
        /// </summary>
        /// <param name="id">Passar o id da Redação</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> DeleteRedacao(int id)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = await _redacaoInterface.DeleteRedacao(id);
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
            var uploadResponse = await _redacaoInterface.UploadArquivoRedacao(arquivo);

            if (!uploadResponse.Sucesso)
                return BadRequest(uploadResponse.Mensagem);

            redacaoDto.CaminhoArquivo = uploadResponse.Dados;

            var createResponse = await _redacaoInterface.CreateRedacao(redacaoDto);

            if (!createResponse.Sucesso)
                return BadRequest(createResponse.Mensagem);

            return Ok(createResponse);
        }
    }
}

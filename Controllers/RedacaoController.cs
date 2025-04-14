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

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> GetRedacoes()
        {
            var response = await _redacaoInterface.GetRedacoes();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<RedacaoDto>>> GetRedacaoById(int id)
        {
            var response = await _redacaoInterface.GetRedacaoById(id);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> CreateRedacao(RedacaoDto novaRedacao)
        {
            return Ok(await _redacaoInterface.CreateRedacao(novaRedacao));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> UpdateRedacao(RedacaoDto editadaRedacaoDto)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = await _redacaoInterface.UpdateRedacao(editadaRedacaoDto);
            return Ok(serviceResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<RedacaoDto>>>> DeleteRedacao(int id)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = await _redacaoInterface.DeleteRedacao(id);
            return Ok(serviceResponse);
        }
    }
}

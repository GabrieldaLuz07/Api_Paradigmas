using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using ApiTF.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace ApiTF.Controllers
{
    public class StockLogController : ControllerBase
    {
        public readonly StockLogService _service;

        public StockLogController(StockLogService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtém os logs de um determinado produto.
        /// </summary>
        /// <param name="idproduto">O ID do produto a ser obtido os logs.</param>
        /// <returns>A lista de logs do produto.</returns>
        /// <response code="200">Indica que a operação foi bem-sucedida e retorna logs correspondentes ao produto.</response>
        /// <response code="404">Indica que o ID do produto informado não existe ou nenhum log foi encontrado para o mesmo.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("/logs/{idproduto}")]
        public ActionResult<List<StockLogResultDTO>> GetStockLogs(int idproduto)
        {
            try
            {
                var logs = _service.GetStockLogByIdProduto(idproduto);
                return logs;
            }
            catch (NotFoundException E)
            {
                return NotFound(E.Message);
            }
            catch (Exception E)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, E.Message);
            }
        }
    }
}

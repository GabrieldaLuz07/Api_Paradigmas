using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using ApiTF.Services;
using ApiTF.BaseDados.Models;
using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using System.Collections.Generic;

namespace ApiTF.Controllers
{
    /// <summary>
    /// Controlador das vendas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly SaleService _service;
        public SaleController(SaleService service)
        {
            _service = service;
        }

        /// <summary>
        /// Inserir uma nova venda.
        /// </summary>
        /// <param name="sales">Vendas a serem inseridas.
        /// É necessário informar todos os campos para criar uma nova venda.
        /// <br>Os retornos são:
        /// 200= Sucesso;
        /// 400= Requisição inválida;
        /// 422= Entidade inválida;</br></param>
        [HttpPost("/sales")]
        [ProducesResponseType(typeof(TbSale), 201)]
        public ActionResult<TbSale> Insert([FromBody] List<SaleDTO> sales)
        {
            try
            {
                var sale = _service.Insert(sales);
                return Ok(sale);
            }
            catch (InvalidEntityExceptions E)
            {
                return new ObjectResult(new { error = E.Message })
                {
                    StatusCode = 422
                };
            }
            catch (BadRequestException E)
            {
                return BadRequest(E.Message);
            }
            catch (Exception E)
            {
                return StatusCode(500, "Erro interno no servidor: " + E.Message);
            }
        }

        /// <summary>
        /// Buscar uma venda pelo código.
        /// </summary>
        /// <param name="code">Código da venda que será buscado para exibição.
        /// <br>Os retornos são:
        /// 200= Sucesso;
        /// 404= Produto não encontrado;
        /// 500= Erro interno do servidor;</br></param>
        [HttpGet("/products/getByCode/{code}")]
        public ActionResult<TbProduct> GetByCode(string code)
        {
            try
            {
                var venda = _service.GetByCode(code);
                return Ok(venda);
            }
            catch (NotFoundException E)
            {
                return NotFound(E.Message);
            }
            catch (Exception E)
            {
                return new ObjectResult(new { error = E.Message })
                {
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// Obtém um relatório de vendas por período.
        /// </summary>
        /// <param name="startDate">A data de início do período.</param>
        /// <param name="endDate">A data de fim do período.</param>
        /// <returns>Uma lista de relatórios de vendas agrupados por código da venda.</returns>
        /// <response code="200">Indica que o relatório de vendas foi retornado com sucesso.</response>
        /// <response code="400">Indica que as datas de início e fim não foram fornecidas ou são inválidas.</response>
        /// <response code="404">Indica que não foram encontradas vendas no período especificado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("report")]
        public ActionResult<List<SalesReportDTO>> GetSalesReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                var report = _service.GetSalesByPeriod(startDate, endDate);
                return Ok(report);
            }
            catch (BadRequestException E)
            {
                return BadRequest(E.Message);
            }
            catch (Exception E)
            {
                return StatusCode(500, "Internal server error: " + E.Message);
            }
        }

    }
}

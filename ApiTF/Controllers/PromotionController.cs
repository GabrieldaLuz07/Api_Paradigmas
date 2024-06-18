using ApiTF.BaseDados.Models;
using ApiTF.Services;
using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace ApiTF.Controllers
{
    /// <summary>
    /// Controlador das promoções.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly PromotionService _service;
        public PromotionController(PromotionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Inserir uma nova promoção.
        /// </summary>
        /// <param name="dto">Estrutura da promoção a ser inserida.
        /// É necessário informar todos os campos para criar uma nova promoção.
        /// <br>Os retornos são:
        /// 200= Sucesso;
        /// 400= Requisição inválida;
        /// 422= Entidade inválida;</br></param>
        /// 500= Erro interno de servidor;</br></param>
        [HttpPost("/promotions")]
        [ProducesResponseType(typeof(TbProduct), 201)]
        [ProducesResponseType(500)]
        public ActionResult<TbPromotion> Insert(PromotionDTO dto)
        {
            try
            {
                var promotion = _service.Insert(dto);
                return CreatedAtAction(nameof(Insert), new { id = promotion.Id }, promotion);
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
                return new ObjectResult(new { error = E.Message })
                {
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// Atualizar uma promoção existente.
        /// </summary>
        /// <param name="id">Identificador da promoção que será atualizada.</param>
        /// <param name="dto">É necessário informar os campos que serão alterados da promoção.
        /// <br>Os retornos são:
        /// 200= Sucesso;
        /// 400= Requisição inválida;
        /// 404= Promoção não encontrada;
        /// 422= Promoção inválida;</br></param>
        [HttpPatch("/promotions/{id}")]
        public ActionResult<TbPromotion> Update(int id, PromotionDTO dto)
        {
            try
            {
                var promotion = _service.Update(dto, id);
                return Ok(promotion);
            }
            catch (InvalidEntityExceptions E)
            {
                return new ObjectResult(new { error = E.Message })
                {
                    StatusCode = 422
                };

            }
            catch (NotFoundException E)
            {
                return NotFound(E.Message);
            }
            catch (BadRequestException E)
            {
                return new ObjectResult(new { error = E.Message })
                {
                    StatusCode = 400
                };
            }
            catch (Exception E)
            {
                return BadRequest(E.Message);
            }

        }

        /// <summary>
        /// Buscar todas as promoções cadastradas.
        /// </summary>
        /// Os retornos são:
        /// 200= Sucesso;
        /// 404= Nenhuma promoção cadastrada;
        /// 500= Erro interno do servidor;
        [HttpGet("/promotions/getAllByDate/{idproduto}/{startDate}/{endDate}")]
        public ActionResult<TbPromotion> GetAllByDate(int idproduto, DateTime startDate, DateTime endDate)
        {
            try
            {
                IEnumerable<TbPromotion> promotions = _service.GetAllByDate(idproduto, startDate, endDate);
                return Ok(promotions);
            }
            catch (Exception E)
            {
                return new ObjectResult(new { error = E.Message })
                {
                    StatusCode = 500
                };
            }
        }
    }
}

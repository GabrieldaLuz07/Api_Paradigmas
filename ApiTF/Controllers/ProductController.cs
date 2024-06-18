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
    /// Controlador dos produtos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;
        public ProductController(ProductService service)
        {
            _service = service;
        }

        /// <summary>
        /// Inserir um novo produto.
        /// </summary>
        /// <param name="dto">Estrutura do produto a ser inserido.
        /// É necessário informar todos os campos para criar um novo produto.
        /// <br>Os retornos são:
        /// 201= Produto inserido com sucesso;
        /// 400= Dados inválidos;
        /// 422= Entidade inválida;</br></param>
        /// 500= Erro interno de servidor;</br></param>
        [HttpPost("/products")]
        [ProducesResponseType(typeof(TbProduct), 201)]
        [ProducesResponseType(500)]
        public ActionResult<TbProduct> Insert(ProductDTO dto)
        {
            try
            {
                var produto = _service.Insert(dto);
                return CreatedAtAction(nameof(Insert), new { id = produto.Id }, produto);
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
        /// Atualizar um produto existente.
        /// </summary>
        /// <param name="id">Identificador do produto que será atualizado.</param>
        /// <param name="dto">É necessário informar os campos que serão alterados do produto.
        /// <br>Os retornos são:
        /// 200= Sucesso;
        /// 400= Requisição inválida;
        /// 404= Produto não encontrado;
        /// 422= Produto inválido;</br></param>
        [HttpPut("/products/{id}")]
        public ActionResult<TbProduct> Update(int id, ProductUpdateDTO dto)
        {
            try
            {
                var produto = _service.Update(dto, id);
                return Ok(produto);
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
        /// Buscar um produto existente.
        /// </summary>
        /// <param name="barcode">Identificador do produto que será buscado para exibição.
        /// <br>Os retornos são:
        /// 200= Sucesso;
        /// 404= Produto não encontrado;
        /// 500= Erro interno do servidor;</br></param>
        [HttpGet("/products/getByBarcode/{barcode}")]
        public ActionResult<TbProduct> GetByBarcode(string barcode)
        {
            try
            {
                var produto = _service.GetByBarcode(barcode);
                return Ok(produto);
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
        /// Buscar todos os produto cadastrados.
        /// </summary>
        /// Os retornos são:
        /// 200= Sucesso;
        /// 404= Nenhum produto cadastrado;
        /// 500= Erro interno do servidor;
        [HttpGet("/products/getAllByDescription/{description}")]
        public ActionResult<TbProduct> GetAllByDescription(string description)
        {
            try
            {
                IEnumerable<TbProduct> products = _service.GetAllByDescription(description);
                return Ok(products);
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
        /// Ajustar estoque.
        /// </summary>
        /// <param name="id">Identificador do produto que será atualizado.</param>
        /// <param name="stock">É necessário informar a quantidade a incrementar ou decrementar.
        /// <br>Os retornos são:
        /// 200= Sucesso;
        /// 400= Requisição inválida;
        /// 404= Produto não encontrado;
        /// 422= Produto inválido;</br></param>
        [HttpPatch("/products/stock/{id}/{stock}")]
        public ActionResult<string> Update(int id, int stock)
        {
            try
            {
                int stockFinal = _service.UpdateStock(id, stock);
                return Ok("Nova quantidade em estoque " + stockFinal);
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
    }
}

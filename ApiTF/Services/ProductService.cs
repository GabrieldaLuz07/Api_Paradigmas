using System.Collections.Generic;
using System;
using ApiTF.BaseDados;
using ApiTF.BaseDados.Models;
using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using System.Linq;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ApiTF.Services
{
    public class ProductService
    {
        private readonly TfDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly StockLogService _stockLogService;
        public readonly IValidator<ProductDTO> _validatorProduct;
        public readonly IValidator<ProductUpdateDTO> _validatorProductUpdate;

        public ProductService(TfDbContext dbcontext, IMapper mapper, StockLogService stockLogService,
            IValidator<ProductDTO> validatorProduct, IValidator<ProductUpdateDTO> validatorProductUpdate)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
            _stockLogService = stockLogService;
            _validatorProduct = validatorProduct;
            _validatorProductUpdate = validatorProductUpdate;
        }

        public TbProduct Insert(ProductDTO dto)
        {
            if (dto.Description == null)
            {
                throw new BadRequestException("Dados inválidos");
            }
            if (dto.Barcodetype == null)
            {
                throw new BadRequestException("Dados inválidos");
            }
            if (dto.Barcodetype == null)
            {
                throw new BadRequestException("Dados inválidos");
            }

            var product = _mapper.Map<TbProduct>(dto);

            _dbcontext.Add(product);
            _dbcontext.SaveChanges();

            _stockLogService.InsertStockLog(new StockLogDTO 
            { 
                Productid = product.Id,
                Qty = product.Stock,
                Createdat = DateTime.Now
            });

            return product;
        }

        public TbProduct Update(ProductUpdateDTO dto, int id)
        {
            var produto = GetById(id);
            if (produto == null)
                throw new NotFoundException("Produto não existe");

            //var validation = _validatorProductUpdate.Validate(dto);
            //if (!validation.IsValid)
            //{
           //     throw new DataValidationException("Dados inválidos", validation.Errors);
           // }

            
            int stockAntigo = produto.Stock;

            _mapper.Map(dto, produto);

            _dbcontext.Update(produto);
            _dbcontext.SaveChanges();

            _stockLogService.InsertStockLog(new StockLogDTO
            {
                Productid = produto.Id,
                Qty = produto.Stock - stockAntigo,
                Createdat = DateTime.Now
            });

            return produto;
        }

        public int UpdateStock(int id, int stock)
        {
            if (stock == 0)
                throw new InvalidEntityExceptions("Quantidade inválida");

            var produto = GetById(id);

            if ((produto.Stock += stock) < 0)
                throw new ArgumentException("Quantidade em estoque menor que o solicitado");

            int stockAntigo = produto.Stock;
            produto.Stock += stock;

            _dbcontext.Update(produto);
            _dbcontext.SaveChanges();

            _stockLogService.InsertStockLog(new StockLogDTO
            {
                Productid = produto.Id,
                Qty = produto.Stock - stockAntigo,
                Createdat = DateTime.Now
            });

            return produto.Stock;
        }

        public TbProduct GetById(int id)
        {
            var produto = _dbcontext.TbProducts.FirstOrDefault(p => p.Id == id);
            if (produto == null)
                throw new NotFoundException("Produto não existe");

            return produto;
        }

        public TbProduct GetByBarcode(string barcode)
        {
            if (barcode.Trim() == null)
                throw new Exception("Código de barras não informado");

            var produto = _dbcontext.TbProducts.FirstOrDefault(p => p.Barcode.ToLower() == barcode.ToLower());
            if (produto == null)
                throw new NotFoundException("Registro não existe");

            return produto;

        }

        public IEnumerable<TbProduct> GetAllByDescription(string description)
        {
            return _dbcontext.TbProducts.Where(p => p.Description.ToLower().Contains(description.ToLower())).ToList();
        }

    }
}

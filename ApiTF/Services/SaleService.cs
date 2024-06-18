using ApiTF.BaseDados.Models;
using ApiTF.BaseDados;
using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.IO;

namespace ApiTF.Services
{
    public class SaleService
    {
        private readonly TfDbContext _dbcontext;
        private readonly ProductService _productService;
        private readonly PromotionService _promotionService;
        private readonly StockLogService _stockLogService;
        private readonly IMapper _mapper;
        private readonly IValidator<SaleDTO> _validator;

        public SaleService(TfDbContext dbcontext, ProductService productService, PromotionService promotionService,
                           StockLogService stockLogService, IMapper mapper, IValidator<SaleDTO> validator)
        {
            _dbcontext = dbcontext;
            _productService = productService;
            _promotionService = promotionService;
            _stockLogService = stockLogService;
            _mapper = mapper;
            _validator = validator;
        }

        public IEnumerable<TbSale> Insert(List<SaleDTO> sales)
        {
            var listaSales = new List<TbSale>();
            foreach (var saleDTO in sales)
            {
                var validation = _validator.Validate(saleDTO);
                if (!validation.IsValid)
                {
                    throw new BadRequestException("Dados inválidos");
                }

                var product = _productService.GetById(saleDTO.Productid);
                if (product == null)
                    throw new NotFoundException("Produto não existente");

                if (product.Stock < saleDTO.Qty)
                    throw new InsufficientStockException("Estoque insuficiente para a movimentação");

                decimal precoUnitario = product.Price;
                decimal precoOriginal = precoUnitario;
                foreach (var promotion in _promotionService.GetActivePromotions(saleDTO.Productid))
                {
                    precoUnitario = ApplyPromotion(precoUnitario, promotion);
                }

                decimal totalDiscount = precoOriginal - precoUnitario;

                var novoStock = product.Stock - saleDTO.Qty;
                _productService.UpdateStock(product.Id, novoStock);

                var stockLogDto = new StockLogDTO
                {
                    Productid = saleDTO.Productid,
                    Qty = -saleDTO.Qty,
                    Createdat = DateTime.Now
                };
                _stockLogService.InsertStockLog(stockLogDto);

                var sale = _mapper.Map<TbSale>(saleDTO);

                sale.Code = Guid.NewGuid().ToString();
                sale.Price = product.Price;
                sale.Discount = totalDiscount;
                sale.Createat = DateTime.Now;

                _dbcontext.Add(sale);
                listaSales.Add(sale);
            }

            _dbcontext.SaveChanges();

            return listaSales;
        }


        public TbSale GetByCode(string code)
        {
            if (code.Trim() == null)
                throw new Exception("Código não informado");

            var sale = _dbcontext.TbSales.FirstOrDefault(c => c.Code == code);
            if (sale == null)
            {
                throw new NotFoundException("Venda não encontrada");
            }
            return sale;
        }

        public List<SalesReportDTO> GetSalesByPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                throw new BadRequestException("Datas de início e fim são obrigatórias");
            }

            var query = from sale in _dbcontext.TbSales
                        join product in _dbcontext.TbProducts on sale.Productid equals product.Id
                        where sale.Createat >= startDate && sale.Createat < endDate.AddDays(1)
                        select new SalesReportDTO
                        {
                            SaleCode = sale.Code,
                            ProductDescription = product.Description,
                            Price = sale.Price,
                            Quantity = sale.Qty,
                            SaleDate = sale.Createat
                        };

            return query.ToList();
        }

        public decimal ApplyPromotion(decimal price, TbPromotion promotion)
        {
            decimal precoDesconto = price;

            switch (promotion.Promotiontype)
            {
                case 0:
                    precoDesconto = price * (1 - promotion.Value / 100);
                    break;
                case 1:
                    precoDesconto = price - promotion.Value;
                    break;
                default:
                    precoDesconto = price;
                    break;
            }

            return precoDesconto;
        }

    }
}

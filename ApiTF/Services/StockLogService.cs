using ApiTF.BaseDados.Models;
using ApiTF.BaseDados;
using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace ApiTF.Services
{
    public class StockLogService
    {
        private readonly TfDbContext _dbContext;
        private readonly IMapper _mapper;

        public StockLogService(TfDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public TbStockLog InsertStockLog(StockLogDTO dto)
        {
            var stockLog = _mapper.Map<TbStockLog>(dto);

            _dbContext.TbStockLogs.Add(stockLog);
            _dbContext.SaveChanges();

            return stockLog;
        }

        public List<StockLogResultDTO> GetStockLogByIdProduto(int idproduto)
        {
            var produto = _dbContext.TbProducts.Any(p => p.Id == idproduto);
            if (!produto)
            {
                throw new NotFoundException("Produto não existe.");
            }

            var logs = from log in _dbContext.TbStockLogs
                       where log.Productid == idproduto
                       select new StockLogResultDTO
                       {
                           Date = log.Createdat,
                           Barcode = log.Product.Barcode,
                           Description = log.Product.Description,
                           Quantity = log.Qty
                       };

            if (!logs.Any())
            {
                throw new NotFoundException("Nenhum log encontrado para o produto.");
            }

            return logs.ToList();
        }
    }
}

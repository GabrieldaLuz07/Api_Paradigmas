using ApiTF.BaseDados.Models;
using ApiTF.Services.DTOs;
using AutoMapper;

namespace ApiTF.Services.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductDTO, TbProduct>();
            CreateMap<TbProduct, ProductDTO>();
            CreateMap<ProductUpdateDTO, TbProduct>();

            CreateMap<StockLogDTO, TbStockLog>();
            CreateMap<TbStockLog, StockLogDTO>();

            CreateMap<PromotionDTO, TbPromotion>();
            CreateMap<TbPromotion, PromotionDTO>();

            CreateMap<SaleDTO, TbSale>();
            CreateMap<TbSale, SaleDTO>();
        }
    }
}

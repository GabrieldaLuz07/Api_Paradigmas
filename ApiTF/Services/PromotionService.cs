using ApiTF.BaseDados.Models;
using ApiTF.BaseDados;
using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System;
using AutoMapper;
using FluentValidation;
using System.IO;


namespace ApiTF.Services
{
    public class PromotionService
    {
        private readonly TfDbContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly IValidator<PromotionDTO> _validator;

        public PromotionService(TfDbContext dbcontext, IMapper mapper, IValidator<PromotionDTO> validator)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
            _validator = validator;
        }

        public TbPromotion Insert(PromotionDTO dto)
        {
            GetProduto(dto.Productid);

            var validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                throw new DataValidationException("Dados inválidos", validation.Errors);
            }

            var promotion = _mapper.Map<TbPromotion>(dto);

            _dbcontext.Add(promotion);
            _dbcontext.SaveChanges();

            return promotion;
        }

        public TbPromotion Update(PromotionDTO dto, int id)
        {
            var promotion = GetById(id);
            if (promotion == null)
            {
                throw new NotFoundException("Promoção não existe.");
            }

            GetProduto(dto.Productid);

            var validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                throw new DataValidationException("Dados inválidos", validation.Errors);
            }

            promotion.Startdate = dto.Startdate;
            promotion.Enddate = dto.Enddate;
            promotion.Promotiontype = dto.Promotiontype;
            promotion.Value = dto.Value;

            _dbcontext.Update(promotion);
            _dbcontext.SaveChanges();

            return promotion;
        }

        public TbPromotion GetById(int id)
        {
            var entity = _dbcontext.TbPromotions.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                throw new NotFoundException("Promoção não existe.");
            }
            return entity;
        }

        public IEnumerable<TbPromotion> GetAllByDate(int idproduto, DateTime startDate, DateTime endDate)
        {
            GetProduto(idproduto);

            return _dbcontext.TbPromotions.Where(
                p => p.Productid == idproduto && p.Startdate >= startDate && p.Enddate <= endDate).ToList();
        }

        public List<TbPromotion> GetActivePromotions(int idproduto)
        {
            GetProduto(idproduto);

            return _dbcontext.TbPromotions.Where(
                p => p.Productid == idproduto && p.Startdate <= DateTime.Now && p.Enddate >= DateTime.Now)
                .OrderBy(p => p.Promotiontype).ToList();
        }

        public bool GetProduto(int idproduto)
        {
            if (!_dbcontext.TbProducts.Any(p => p.Id == idproduto))
                throw new Exception("Produto não existe");
            else
                return true;
        }
    }
}

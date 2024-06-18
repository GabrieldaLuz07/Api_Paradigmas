using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using FluentValidation;
using Microsoft.VisualBasic;
using System;

namespace ApiTF.Services.Validate
{
    public class PromotionValidate : AbstractValidator<PromotionDTO>
    {
        public PromotionValidate()
        {

            RuleFor(promotion => promotion.Startdate)
                .NotEmpty().WithMessage("Data de início da promoção é obrigatória.");

            RuleFor(promotion => promotion.Enddate)
                .NotEmpty().WithMessage("Data fim da promoção é obrigatória.");

            RuleFor(promotion => promotion.Promotiontype)
                .Must(p => p == 0 || p == 1)
                .WithMessage("Tipo de promoção deve ser 0 (desconto percentual) ou 1 (desconto fixo)");

            RuleFor(promotion => promotion.Value)
                .GreaterThan(0)
                .WithMessage("Valor da promoção inválido");

            When(promotion => promotion.Promotiontype == 0, () =>
            {
                RuleFor(promotion => promotion.Value)
                    .InclusiveBetween(0, 100)
                    .WithMessage("Para desconto percentual, o valor deve estar entre 0 e 100");
            });

            When(promotion => promotion.Promotiontype == 1, () =>
            {
                RuleFor(promotion => promotion.Value)
                    .GreaterThan(0)
                    .WithMessage("Tipo da promoção não informado ou inválido");
            });
        }
    }
}

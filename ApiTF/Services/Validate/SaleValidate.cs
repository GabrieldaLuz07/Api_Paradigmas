using ApiTF.BaseDados.Models;
using ApiTF.Services.DTOs;
using ApiTF.Services.Exceptions;
using System.Text.RegularExpressions;
using ApiTF.Services;
using System;
using FluentValidation;

namespace ApiTF.Services.Validate
{
    public class SaleValidate : AbstractValidator<SaleDTO>
    {
        public SaleValidate()
        {
            RuleFor(sale => sale.Productid)
                .GreaterThan(0).WithMessage("O ID do produto é obrigatório.");

            RuleFor(sale => sale.Qty)
                .GreaterThan(0).WithMessage("A quantidade vendida deve ser maior que zero.");
        }
    }
}


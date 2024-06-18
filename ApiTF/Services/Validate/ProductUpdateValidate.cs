using ApiTF.Services.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ApiTF.Services.Validate
{
    public class ProductUpdateValidate : AbstractValidator<ProductUpdateDTO>
    {
        public static bool isValidBarcodeType(string barcodeType)
        {
            if (
                // EAN13
                barcodeType.Length == 13 && Regex.IsMatch(barcodeType, @"^\d{13}$") ||
                // DUN14
                barcodeType.Length == 14 && Regex.IsMatch(barcodeType, @"^\d{14}$") ||
                // UPC
                barcodeType.Length == 12 && Regex.IsMatch(barcodeType, @"^\d{12}$") ||
                // Code11
                Regex.IsMatch(barcodeType, @"^[0-9\-*]+$") ||
                // Code39
                Regex.IsMatch(barcodeType, @"^[A-Z0-9\-\.\/\+\%\ ]+$")
                )
                return true;
            else
                return false;
        }

        public ProductUpdateValidate()
        {
            RuleFor(product => product.Description)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MaximumLength(255).WithMessage("A descrição do produto não pode exceder 255 caracteres.");

            RuleFor(product => product.Barcode)
                .NotEmpty().WithMessage("O código de barras é obrigatório.")
                .MaximumLength(40).WithMessage("O código de barras do produto não pode exceder 40 caracteres.");

            RuleFor(product => product.Barcodetype)
                .NotEmpty().WithMessage("O tipo de código de barras é obrigatório.")
                .MaximumLength(10).WithMessage("O tipo de código de barras do produto não pode exceder 10 caracteres.")
                .Must(isValidBarcodeType).WithMessage("O código de barras não é válido para o tipo especificado.");

            RuleFor(product => product.Price)
                .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

            RuleFor(product => product.Costprice)
                .GreaterThan(0).WithMessage("O preço de custo deve ser maior que zero.");

        }
    }
}

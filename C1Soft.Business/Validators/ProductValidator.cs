using C1Soft.Business.DTOs.Product;
using FluentValidation;

namespace C1Soft.Business.Validators;

public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateValidator()
    {
        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("Ürün kodu zorunludur.")
            .MaximumLength(50).WithMessage("Ürün kodu en fazla 50 karakter olabilir.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Marka zorunludur.")
            .MaximumLength(100).WithMessage("Marka en fazla 100 karakter olabilir.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

        RuleFor(x => x.CriticalStockThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("Kritik stok eşiği negatif olamaz.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategori seçimi zorunludur.");
    }
}

public class ProductEditValidator : AbstractValidator<ProductEditDto>
{
    public ProductEditValidator()
    {
        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("Ürün kodu zorunludur.")
            .MaximumLength(50).WithMessage("Ürün kodu en fazla 50 karakter olabilir.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Marka zorunludur.")
            .MaximumLength(100).WithMessage("Marka en fazla 100 karakter olabilir.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

        RuleFor(x => x.CriticalStockThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("Kritik stok eşiği negatif olamaz.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategori seçimi zorunludur.");
    }
}

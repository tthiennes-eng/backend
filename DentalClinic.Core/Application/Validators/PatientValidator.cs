using FluentValidation;
using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Application.Validators;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(255).WithMessage("O nome não pode exceder 255 caracteres.");

        RuleFor(p => p.CPF.Value)
            .NotEmpty().WithMessage("O CPF é obrigatório.")
            .Length(11).WithMessage("O CPF deve conter exatamente 11 dígitos.");

        RuleFor(p => p.DateOfBirth)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThan(DateTime.Today).WithMessage("A data de nascimento não pode ser no futuro.");

        RuleFor(p => p.EmailAddress.Value)
            .EmailAddress().WithMessage("Formato de e-mail inválido.")
            .When(p => !string.IsNullOrEmpty(p.EmailAddress.Value));
    }
}

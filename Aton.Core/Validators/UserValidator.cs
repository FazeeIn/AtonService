using Aton.Core.Models;
using FluentValidation;

namespace Aton.Core.Validators;

public class UserValidator: AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login cannot be empty")
            .MinimumLength(6).WithMessage("Incorrect Login length. Characters must be more than 6")
            .MaximumLength(20).WithMessage("Incorrect Login length. Characters must be less than 20")
            .Matches("^[a-zA-Z][a-zA-Z0-9._-]*$")
            .WithMessage("The Login must consist of Latin letters, also begin with a " +
                         "Latin letter and contain only numbers, letters, periods, dashes and underscores");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(6).WithMessage("Incorrect Password length. Characters must be more than 6")
            .MaximumLength(20).WithMessage("Incorrect Password length. Characters must be less than 20")
            .Matches("^[a-zA-Z][a-zA-Z0-9._-]*$")
            .WithMessage("The Password must consist of Latin letters, also begin with a " +
                         "Latin letter and contain only numbers, letters, periods, dashes and underscores");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .MaximumLength(50).WithMessage("Incorrect Name length. Characters must be less than 50")
            .Matches("^[a-zA-Z][a-zA-Z]*$")
            .WithMessage("The Name must consist of Latin letters, also begin with a Latin letter");
            
        RuleFor(x => x.Gender)
            .InclusiveBetween(0, 2)
            .NotNull().WithMessage("CratedOn cannot be null")
            .WithMessage("Gender must be 0, 1 or 2");
        
        RuleFor(x => x.Admin)
            .NotEmpty().WithMessage("Status cannot be empty");
        
        RuleFor(x => x.Birthday)
            .Must(b => b == null || b.Value <= DateTime.UtcNow)
            .WithMessage("Дата рождения не может быть в будущем");

        RuleFor(x => x.CreatedOn)
            .NotNull().WithMessage("CratedOn cannot be null")
            .NotEmpty().WithMessage("CratedOn cannot be empty")
            .Must(b => b.Date <= DateTime.UtcNow);
        
        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy обязателен")
            .MinimumLength(6).WithMessage("Incorrect CreatedBy length. Characters must be more than 6")
            .MaximumLength(20).WithMessage("Incorrect CreatedBy length. Characters must be less than 20")
            .Matches("^[a-zA-Z][a-zA-Z0-9._-]*$")
            .WithMessage("the CreatedBy must consist of Latin letters, also begin with a " +
                         "Latin letter and contain only numbers, letters, periods, dashes and underscores");
    }    
}
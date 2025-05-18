using Aton.Core.Models;
using FluentValidation;

namespace Aton.Core.Validators;

public class UpdateUserValidator: AbstractValidator<UpdateUser>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Login)
            .MinimumLength(6).WithMessage("Incorrect Login length. Characters must be more than 6")
            .MaximumLength(20).WithMessage("Incorrect Login length. Characters must be less than 20")
            .Matches("^[a-zA-Z][a-zA-Z0-9._-]*$")
            .WithMessage("The Login must consist of Latin letters, also begin with a " +
                         "Latin letter and contain only numbers, letters, periods, dashes and underscores");
        
        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Incorrect Password length. Characters must be more than 6")
            .MaximumLength(20).WithMessage("Incorrect Password length. Characters must be less than 20")
            .Matches("^[a-zA-Z][a-zA-Z0-9._-]*$")
            .WithMessage("The Password must consist of Latin letters, also begin with a " +
                         "Latin letter and contain only numbers, letters, periods, dashes and underscores");
        
        RuleFor(x => x.Name)
            .MaximumLength(50).WithMessage("Incorrect Name length. Characters must be less than 50")
            .Matches("^[a-zA-Z][a-zA-Z]*$")
            .WithMessage("The Name must consist of Latin letters, also begin with a Latin letter");
            
        RuleFor(x => x.Gender)
            .InclusiveBetween(0, 2)
            .WithMessage("Gender must be 0, 1 or 2");
        
    } 
}
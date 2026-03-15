using FluentValidation;
using TinyUrlApp.Model;

namespace TinyUrlApp.validatorValidator
{
    public class EndPointValidator : AbstractValidator<LinkAdd>
    {
        public EndPointValidator() {
            RuleFor(a => a.originalUrl)
                    .NotEmpty().WithMessage("Url Should not empty")
                    .Matches(@"^(https?://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$").WithMessage("Invalid Url");
            RuleFor(a => a.isPrivate)
                .NotNull().WithMessage("isPrivate should not empty");
        }
    }

    public class EndPointIdValidator : AbstractValidator<int>
    {
        public EndPointIdValidator()
        {
            RuleFor(a => a)
                .GreaterThan(0).WithMessage("Invalid Code");
        }
    }
}

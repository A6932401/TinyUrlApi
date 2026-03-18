using FluentValidation;
using TinyUrlApp.Model;

namespace TinyUrlApp.validatorValidator
{
    public class EndPointValidator : AbstractValidator<LinkAdd>
    {
        public EndPointValidator() {
            RuleFor(a => a.originalUrl)
                    .NotEmpty().WithMessage("Url Should not empty")
                    .Must(url => isValidUrl(url))
                    .WithMessage("Invalid Url");
            RuleFor(a => a.isPrivate)
                .NotNull().WithMessage("isPrivate should not empty");
        }

        private bool isValidUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri)
                 && (uri.Scheme == Uri.UriSchemeHttp
                  || uri.Scheme == Uri.UriSchemeHttps)) { 
                return true;
            }

            return false;
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

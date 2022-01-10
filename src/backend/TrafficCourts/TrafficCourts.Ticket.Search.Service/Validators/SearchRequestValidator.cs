using FluentValidation;

namespace TrafficCourts.Ticket.Search.Service.Validators
{
    public class SearchRequestValidator : AbstractValidator<SearchRequest>
    {
        public SearchRequestValidator()
        {
            RuleFor(request => request.Number).NotEmpty().WithMessage("Number is required.");
            RuleFor(request => request.Time).NotEmpty().WithMessage("Time) is required.");
        }
    }
}

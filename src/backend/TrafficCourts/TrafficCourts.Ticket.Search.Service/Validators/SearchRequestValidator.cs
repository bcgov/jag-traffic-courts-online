using FluentValidation;

namespace TrafficCourts.Ticket.Search.Service.Validators
{
    public class SearchRequestValidator : AbstractValidator<SearchRequest>
    {
        public SearchRequestValidator()
        {
            RuleFor(request => request.Number).NotEmpty().WithMessage("Number is required.");
            RuleFor(request => request.Time.Hour).InclusiveBetween(0, 23);
            RuleFor(request => request.Time.Minute).InclusiveBetween(0, 59);
        }
    }
}

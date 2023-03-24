using FluentValidation;

namespace TrafficCourts.Test.Citizen.Service.Validators;

public abstract class ValidatorTest<TModel, TValidator> where TValidator : AbstractValidator<TModel>, new()
{
    protected readonly TValidator _sut = new TValidator();

    protected const string EnumValidator = "EnumValidator";
    protected const string MaximumLengthValidator = "MaximumLengthValidator";
    protected const string NotEmptyValidator = "NotEmptyValidator";
    protected const string NotNullValidator = "NotNullValidator";
    /// <summary>
    /// User supplied predicate in a Must() call.
    /// </summary>
    protected const string PredicateValidator = "PredicateValidator";
}

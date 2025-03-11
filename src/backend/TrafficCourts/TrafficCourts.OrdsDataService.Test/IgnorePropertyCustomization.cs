using AutoFixture;
using System.Linq.Expressions;

namespace TrafficCourts.OrdsDataService.Test;

public class IgnorePropertyCustomization<T> : ICustomization
{
    private readonly string _propertyName;

    public IgnorePropertyCustomization(string propertyName)
    {
        _propertyName = propertyName;
    }

    public IgnorePropertyCustomization(Expression<Func<T, object?>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            _propertyName = memberExpression.Member.Name;
        }
        else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
        {
            _propertyName = operand.Member.Name;
        }
        else
        {
            throw new ArgumentException("Invalid expression", nameof(expression));
        }
    }


    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new IgnorePropertySpecimenBuilder<T>(_propertyName));
    }
}

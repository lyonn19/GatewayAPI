using FluentValidation;
using FluentValidation.Results;
using GatewayApi.Common.Results;

namespace GatewayApi.Common.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected bool BeAValidGuid(string id)
    {
        return Guid.TryParse(id, out _);
    }
}

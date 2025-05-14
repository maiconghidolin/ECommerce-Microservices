using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.Attributes;

public class NotMinDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTimeOffset dto)
            return dto > DateTimeOffset.MinValue;

        return false;
    }
}

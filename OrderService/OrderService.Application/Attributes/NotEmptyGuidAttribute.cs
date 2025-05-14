using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.Attributes;

public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        return value is Guid guid && guid != Guid.Empty;
    }
}
using OrderService.Application.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.Models;

public class Address : BaseModel
{

    [NotEmptyGuid]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(250)]
    public string Street { get; set; }

    [Required]
    [MaxLength(50)]
    public string Number { get; set; }

    [Required]
    [MaxLength(250)]
    public string City { get; set; }

    [Required]
    [MaxLength(250)]
    public string State { get; set; }

    [Required]
    [MaxLength(50)]
    public string ZipCode { get; set; }
}
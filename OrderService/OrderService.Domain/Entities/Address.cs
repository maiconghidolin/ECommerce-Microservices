using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Domain.Entities;

[Table("Addresses")]

public class Address : BaseEntity
{
    [Required]
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

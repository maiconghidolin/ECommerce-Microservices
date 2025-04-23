using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.Entities;

public class BaseEntity
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public DateTimeOffset DeletedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

}

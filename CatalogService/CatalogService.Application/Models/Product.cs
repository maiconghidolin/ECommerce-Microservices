using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.Models;

public class Product : BaseModel
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
}

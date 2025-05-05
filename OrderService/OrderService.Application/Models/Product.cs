namespace OrderService.Application.Models;

public class Product : BaseModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
}

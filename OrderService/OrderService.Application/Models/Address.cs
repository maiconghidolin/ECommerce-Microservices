namespace OrderService.Application.Models;

public class Address : BaseModel
{

    public Guid UserId { get; set; }

    public string Street { get; set; }

    public string Number { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string ZipCode { get; set; }
}
﻿namespace OrderService.Application.Models;

public class OrderItem : BaseModel
{
    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
using System;
using System.Collections.Generic;
using Flunt.Validations;
using Store.Domain.Enums;

namespace Store.Domain.Entities;

public class Order : Entity
{
    public Order(Customer customer, decimal deliveryFee, Discount? discount)
    {
        AddNotifications(new Contract<Order>()
            .Requires()
            .IsNotNull(customer, "Customer", "Cliente inválido")
        );

        Customer = customer;
        Date = DateTime.Now;
        Number = Guid.NewGuid().ToString().Substring(0, 8);
        Items = new List<OrderItem>();
        DeliveryFee = deliveryFee;
        Status = EOrderStatus.WaitingPayment;
        Discount = discount;
    }

    public Customer Customer { get; private set; }
    public DateTime Date { get; private set; }
    public string Number { get; private set; }
    public IList<OrderItem> Items { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public EOrderStatus Status { get; private set; }
    public Discount? Discount { get; private set; }

    public void AddItem(Product? product, int quantity)
    {
        var item = new OrderItem(product, quantity);
        if (item.IsValid)
            Items.Add(item);
    }

    public decimal Total()
    {
        decimal total = 0;
        foreach (var item in Items)
        {
            total += item.Total();
        }

        total += DeliveryFee;
        total -= Discount is null ? 0 : Discount.Value();

        return total;
    }

    public void Pay(decimal amount)
    {
        if (amount == Total())
            Status = EOrderStatus.WaitingDelivery;
    }

    public void Ship()
    {
        Status = EOrderStatus.Delivered;
    }

    public void Cancel()
    {
        Status = EOrderStatus.Canceled;
    }
}
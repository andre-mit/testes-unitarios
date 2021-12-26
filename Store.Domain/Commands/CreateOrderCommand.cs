using System;
using System.Collections.Generic;
using Store.Domain.Commands.Interfaces;
using Flunt.Notifications;
using Flunt.Validations;

namespace Store.Domain.Commands;

public class CreateOrderCommand : Notifiable<Notification>, ICommand
{
    public CreateOrderCommand()
    {
        Items = new List<CreateOrderItemCommand>();
    }

    public CreateOrderCommand(string customer, string zipCode, string promoCode, IList<CreateOrderItemCommand> items)
    {
        Customer = customer;
        ZipCode = zipCode;
        PromoCode = promoCode;
        Items = items;
    }

    public string Customer { get; set; }
    public string ZipCode { get; set; }
    public string PromoCode { get; set; }
    public IList<CreateOrderItemCommand> Items { get; set; }

    public void Validate()
    {
        AddNotifications(new Contract<CreateOrderCommand>()
            .Requires()
            .IsNotNullOrWhiteSpace(Customer, "Customer", "Cliente inválido")
            .AreEquals(ZipCode.Length, 8, "ZipCode", "CEP inválido")
            .IsGreaterThan(Items.Count, 0, "Items", "Pedido não contém itens")
        );
    }
}
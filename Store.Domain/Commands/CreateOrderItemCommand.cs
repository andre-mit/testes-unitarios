using System;
using System.Collections.Generic;
using Store.Domain.Commands.Interfaces;
using Flunt.Notifications;
using Flunt.Validations;

namespace Store.Domain.Commands
{
    public class CreateOrderItemCommand : Notifiable<Notification>, ICommand
    {
        public CreateOrderItemCommand(Guid product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public Guid Product { get; set; }
        public int Quantity { get; set; }

        public void Validate()
        {
            AddNotifications(new Contract<CreateOrderItemCommand>()
                .Requires()
                .AreEquals(Product.ToString().Length, 36, "Product", "Produto inválido")
                .AreNotEquals(Product, Guid.Empty, "Product", "Produto inválido")
                .IsGreaterThan(Quantity, 0, "Quantity", "Quantidade inválida")
            );
        }
    }
}
using System.Linq;
using Flunt.Notifications;
using Store.Domain.Commands;
using Store.Domain.Commands.Interfaces;
using Store.Domain.Entities;
using Store.Domain.Handlers.Interfaces;
using Store.Domain.Repositories;

namespace Store.Domain.Handlers;

public class OrderHandler : Notifiable<Notification>, IHandler<CreateOrderCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDeliveryFeeRepository _deliveryFeeRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderHandler(ICustomerRepository customerRepository, IDeliveryFeeRepository deliveryFeeRepository, IDiscountRepository discountRepository, IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _customerRepository = customerRepository;
        _deliveryFeeRepository = deliveryFeeRepository;
        _discountRepository = discountRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public ICommandResult Handle(CreateOrderCommand command)
    {
        // Fail Fast Validation
        command.Validate();
        if (!command.IsValid)
            return new GenericCommandResult(false, "Pedido inválido", command.Notifications);

        // Obter cliente
        var customer = _customerRepository.Get(command.Customer);

        // Obter valor entrega
        var deliveryFee = _deliveryFeeRepository.Get(command.ZipCode);

        // Obter desconto
        var discount = _discountRepository.Get(command.PromoCode);

        // Gerar pedido
        var products = _productRepository.Get(command.Items.Select(i => i.Product).ToList());
        var order = new Order(customer, deliveryFee, discount);

        foreach (var item in command.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.Product);
            order.AddItem(product, item.Quantity);
        }

        // Agrupar as notificações
        AddNotifications(order.Notifications);

        // verifica se nao tem erros
        if (!IsValid)
            return new GenericCommandResult(false, "Falha ao gerar pedido", Notifications);

        // Salvar pedido
        _orderRepository.Save(order);
        return new GenericCommandResult(true, "Pedido gerado com sucesso", order);
    }
}
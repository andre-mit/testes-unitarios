using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Store.Domain.Commands;
using Store.Domain.Handlers;
using Store.Domain.Repositories;
using Store.Tests.Repositories;

namespace Store.Tests.Handlers;

[TestClass]
public class OrderHandlerTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDeliveryFeeRepository _deliveryFeeRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    private readonly OrderHandler _orderHandler;

    public OrderHandlerTests()
    {
        _customerRepository = new FakeCustomerRepository();
        _deliveryFeeRepository = new FakeDeliveryFeeRepository();
        _discountRepository = new FakeDiscountRepository();
        _productRepository = new FakeProductRepository();
        _orderRepository = new FakeOrderRepository();

        _orderHandler = new OrderHandler(_customerRepository, _deliveryFeeRepository, _discountRepository, _productRepository, _orderRepository);
    }

    [TestMethod]
    [TestCategory("Handlers")]
    public void Dado_um_cliente_inexistente_o_pedido_nao_deve_ser_gerado()
    {
        var command = new CreateOrderCommand();
        command.Customer = "123";
        command.ZipCode = "01747041";
        command.PromoCode = "12345";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));

        _orderHandler.Handle(command);

        Assert.AreEqual(_orderHandler.IsValid, false);
    }

    [TestMethod]
    [TestCategory("Handlers")]
    public void Dado_um_cep_invalido_o_pedido_deve_ser_gerado_normalmente()
    {
        var command = new CreateOrderCommand();
        command.Customer = "12345678911";
        command.ZipCode = "1111111";
        command.PromoCode = "12345";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));

        _orderHandler.Handle(command);

        Assert.AreEqual(_orderHandler.IsValid, true);
    }

    [TestMethod]
    [TestCategory("Handlers")]
    public void Dado_um_desconto_invalido_o_pedido_deve_ser_gerado_normalmente()
    {
        var command = new CreateOrderCommand();
        command.Customer = "12345678911";
        command.ZipCode = "1111111";
        command.PromoCode = "67890";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));

        _orderHandler.Handle(command);

        Assert.AreEqual(_orderHandler.IsValid, true);
    }

    [TestMethod]
    [TestCategory("Handlers")]
    public void Dado_um_pedido_sem_itens_o_mesmo_nao_deve_ser_gerado()
    {
        var command = new CreateOrderCommand();
        command.Customer = "12345678911";
        command.ZipCode = "1111111";
        command.PromoCode = "67890";

        command.Validate();

        Assert.AreEqual(command.IsValid, false);
    }

    [TestMethod]
    [TestCategory("Handlers")]
    public void Dado_um_comando_invalido_o_pedido_nao_deve_ser_gerado()
    {
        var command = new CreateOrderCommand();
        command.Customer = "";
        command.ZipCode = "01747041";
        command.PromoCode = "PROMO";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Validate();

        Assert.AreEqual(command.IsValid, false);
    }

    [TestMethod]
    [TestCategory("Handlers")]
    public void Dado_um_comando_valido_o_mesmo_deve_ser_gerado()
    {
        var command = new CreateOrderCommand();
        command.Customer = "12345678911";
        command.ZipCode = "01747041";
        command.PromoCode = "12345";
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
        command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));

        _orderHandler.Handle(command);

        Assert.AreEqual(_orderHandler.IsValid, true);
    }
}
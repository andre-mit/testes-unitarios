using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Store.Domain.Entities;
using Store.Domain.Enums;

namespace Store.Tests.Domain.Entities;

[TestClass]
public class OrderTests
{
    private readonly Customer _customer = new("André", "contato@andre-mit.dev");
    private readonly Product _product = new("Produto 01", 10, true);
    private readonly Discount _discount = new(10, DateTime.Now.AddDays(5));

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_novo_pedido_valido_ele_deve_gerar_um_numero_com_8_caracteres()
    {
        var order = new Order(_customer, 35.59m, null);
        Assert.AreEqual(8, order.Number.Length);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_novo_pedido_seu_status_deve_ser_aguardando_pagamento()
    {
        var order = new Order(_customer, 35.59m, null);
        Assert.AreEqual(EOrderStatus.WaitingPayment, order.Status);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_pagamento_seu_status_deve_ser_aguardando_entrega()
    {
        var order = new Order(_customer, 35.59m, null);
        order.AddItem(_product, 1);
        order.Pay(45.59m);
        Assert.AreEqual(EOrderStatus.WaitingDelivery, order.Status);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_o_cancelamento_de_um_pedido_seu_status_deve_ser_cancelado()
    {
        var order = new Order(_customer, 35.59m, null);
        order.Cancel();
        Assert.AreEqual(EOrderStatus.Canceled, order.Status);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_novo_item_sem_produto_o_mesmo_nao_deve_ser_adicionado()
    {
        var order = new Order(_customer, 35.59m, null);
        order.AddItem(null, 1);
        Assert.AreEqual(0, order.Items.Count);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_novo_item_com_quantidade_zero_ou_menor_o_mesmo_nao_deve_ser_adicionado()
    {
        var order = new Order(_customer, 35.59m, null);
        order.AddItem(_product, 0);
        Assert.AreEqual(0, order.Items.Count);
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_novo_pedido_valido_seu_total_deve_ser_50()
    {
        var order = new Order(_customer, 10, _discount);
        order.AddItem(_product, 5);
        Assert.AreEqual(50, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_desconto_expirado_o_valor_do_pedido_deve_ser_60()
    {
        var order = new Order(_customer, 10, new Discount(10, DateTime.Now.AddDays(-5)));
        order.AddItem(_product, 5);
        Assert.AreEqual(60, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_descont_invalido_o_valor_do_pedido_deve_ser_60()
    {
        var order = new Order(_customer, 10, null);
        order.AddItem(_product, 5);
        Assert.AreEqual(60, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_desconto_valido_de_10_o_valor_do_pedido_deve_ser_50()
    {
        var order = new Order(_customer, 10, _discount);
        order.AddItem(_product, 5);
        Assert.AreEqual(50, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_uma_taxa_de_entrega_de_10_o_valor_do_pedido_deve_ser_60()
    {
        var order = new Order(_customer, 10, null);
        order.AddItem(_product, 5);
        Assert.AreEqual(60, order.Total());
    }

    [TestMethod]
    [TestCategory("Domain")]
    public void Dado_um_pedido_sem_cliente_o_mesmo_deve_ser_invalido()
    {
        var order = new Order(null, 10, _discount);
        Assert.AreEqual(false, order.IsValid);
    }
}
namespace Store.Domain.Enums
{
    public enum EOrderStatus
    {
        WaitingPayment = 1,
        WaitingDelivery = 2,
        Delivered = 3,
        Canceled = 4
    }
}
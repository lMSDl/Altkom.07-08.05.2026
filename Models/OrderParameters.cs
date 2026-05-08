namespace Models
{
    [Flags]
    public enum OrderParameters
    {
        None = 1 << 0,
        GiftWrapped = 1 << 1,
        ExpressDelivery = 1 << 2,
        IncludeInvoice = 1 << 3,
        Fragile = 1 << 4,
        SignatureRequired = 1 << 5,
    }
}

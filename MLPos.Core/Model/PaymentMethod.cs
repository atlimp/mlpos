namespace MLPos.Core.Model;

public class PaymentMethod : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public bool InvoiceOnPost { get; set; } = false;
}
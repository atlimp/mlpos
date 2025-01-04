using MLPos.Core.Enums;

namespace MLPos.Core.Model;

public class Product : Entity
{
    public string Description { get; set; } = string.Empty;
    public ProductType Type { get; set; }
    public string Image { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
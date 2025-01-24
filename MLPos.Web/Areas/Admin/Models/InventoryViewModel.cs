using MLPos.Core.Model;

namespace MLPos.Web.Models
{
    public class InventoryViewModel
    {
        public ProductInventory Inventory { get; set; }
        public int CountedQuantity { get; set; }
    }
}

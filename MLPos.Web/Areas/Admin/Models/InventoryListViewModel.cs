using MLPos.Core.Model;

namespace MLPos.Web.Models
{
    public class InventoryListViewModel
    {
        public IEnumerable<ProductInventory> Inventory { get; set; }
    }
}

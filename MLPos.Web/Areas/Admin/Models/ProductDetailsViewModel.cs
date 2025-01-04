using MLPos.Core.Model;

namespace MLPos.Web.Models;

public class ProductDetailsViewModel
{
    public bool Editing { get; set; }
    public bool NewProduct { get; set; }
    public Product Product { get; set; }
    public IEnumerable<ValidationError> ValidationErrors { get; set; }
}
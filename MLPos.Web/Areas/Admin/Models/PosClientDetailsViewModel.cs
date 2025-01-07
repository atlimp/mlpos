using MLPos.Core.Model;

namespace MLPos.Web.Models;

public class PosClientDetailsViewModel
{
    public bool Editing { get; set; }
    public bool NewPosClient { get; set; }
    public PosClient PosClient { get; set; }
    public IEnumerable<ValidationError> ValidationErrors { get; set; }
}
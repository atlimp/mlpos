namespace MLPos.Core.Model;

public abstract class Entity
{
    public long Id { get; set; }
    public DateTime DateInserted { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool ReadOnly { get; set; } = false;
}
namespace MLPos.Core.Model;

public record ValidationError
{
    public string Error { get; set; } = string.Empty;
}
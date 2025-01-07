namespace MLPos.Web.Models
{
    public class CreateTransactionRequest
    {
        public long PosClientId { get; set; }
        public long CustomerId { get; set; }
    }
}

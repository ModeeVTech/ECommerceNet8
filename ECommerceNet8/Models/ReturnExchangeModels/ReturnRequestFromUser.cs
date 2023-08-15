using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceNet8.Models.ReturnExchangeModels
{
    public class ReturnRequestFromUser
    {
        public int Id { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentifier { get; set; }
        [Column(TypeName ="datetime")]
        public DateTime ExchangeRequestTime { get; set; }
        public string UserId { get; set; } = "0";
        public string Email { get; set; } 
        public string PhoneNumber { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Message { get; set; }
    }
}

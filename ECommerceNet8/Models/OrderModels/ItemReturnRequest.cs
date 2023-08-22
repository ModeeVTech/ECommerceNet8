using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.OrderModels
{
    public class ItemReturnRequest
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order order { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentifier { get; set; }
        public string AdminId { get; set; }
        public string AdminFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserBankName { get; set; }
        public string UserBankAccount { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ExchangeRequestTime { get; set; }

        public ICollection<ItemGoodForRefund> itemsGoodForRefund { get; set; }
        public ICollection<ItemBadForRefund> itemsBadForRefund { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal? totalRequestForRefund { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? totalAmountNotRefunded { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? totalAmountRefunded { get; set; }
        public bool RequestRefunded { get; set; }
        public bool RequestClosed { get; set; }
    }
}

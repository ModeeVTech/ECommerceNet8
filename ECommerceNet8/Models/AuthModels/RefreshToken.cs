namespace ECommerceNet8.Models.AuthModels
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }

        public DateTime AddedDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}

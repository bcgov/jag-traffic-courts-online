namespace DisputeApi.Web.Features.TokenService.Models
{
    public class Token
    {

        public string JwtToken { get; set; }

        public Token(string token)
        {
            this.JwtToken = token;
        }
    }
}

namespace DisputeApi.Web.Features.TokenService.Model
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
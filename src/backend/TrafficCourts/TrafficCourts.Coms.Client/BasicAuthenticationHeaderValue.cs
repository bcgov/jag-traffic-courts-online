using System.Net.Http.Headers;
using System.Text;

namespace TrafficCourts.Coms.Client;

/// <summary>
/// HTTP Basic Authentication authorization header
/// </summary>
/// <see cref="https://github.com/IdentityModel/IdentityModel/blob/main/src/Client/BasicAuthenticationHeaderValue.cs"/>
/// <seealso cref="System.Net.Http.Headers.AuthenticationHeaderValue" />
internal class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicAuthenticationHeaderValue"/> class.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    public BasicAuthenticationHeaderValue(string userName, string password)
        : base("Basic", EncodeCredential(userName, password))
    { }

    /// <summary>
    /// Encodes the credential.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">userName</exception>
    public static string EncodeCredential(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
        password ??= string.Empty;

        Encoding encoding = Encoding.UTF8;
        string credential = String.Format("{0}:{1}", userName, password);

        return Convert.ToBase64String(encoding.GetBytes(credential));
    }

}
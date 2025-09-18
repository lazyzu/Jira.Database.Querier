using System;
using System.Net.Http;
using System.Text;

namespace lazyzu.Jira.Database.Querier.RestApi
{
    public interface IAuthenticator
    {
        void Authenticate(HttpClient client, HttpRequestMessage request);
    }

    

    public class BasicAuthenticator : IAuthenticator
    {
        private readonly string username;
        private readonly string password;
        private readonly Encoding encoding;

        public BasicAuthenticator(string username, string password, Encoding encoding)
        {
            this.username = username;
            this.password = password;
            this.encoding = encoding;
        }

        public BasicAuthenticator(string username, string password)
        {
            this.username = username;
            this.password = password;
            this.encoding = Encoding.UTF8;
        }

        public void Authenticate(HttpClient client, HttpRequestMessage request)
        {
            var accessToken = Convert.ToBase64String(encoding.GetBytes(username + ":" + password));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", accessToken);
        }
    }

    public class JwtAuthenticator : IAuthenticator
    {
        private readonly string accessToken;

        public JwtAuthenticator(string accessToken) 
        {
            this.accessToken = accessToken?.Trim();
        }

        public void Authenticate(HttpClient client, HttpRequestMessage request)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}

namespace lazyzu.Jira.Database.Querier
{
    public static class Authenticator
    {
        public static RestApi.IAuthenticator FromBasic(string username, string password)
            => new RestApi.BasicAuthenticator(username, password);

        public static RestApi.IAuthenticator FromJwt(string accessToken)
            => new RestApi.JwtAuthenticator(accessToken);
    }
}

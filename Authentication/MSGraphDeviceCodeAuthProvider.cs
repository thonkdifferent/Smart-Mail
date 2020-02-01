using Microsoft.Graph;
using Microsoft.Identity.Client;
using SmartMail.ExceptionHandler;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SmartMail.Authentication
{
    class MSGraphDeviceCodeAuthProvider : IAuthenticationProvider
    {
        private IPublicClientApplication _msalClient;
        private string[] _scopes;
        private IAccount _userAccount;

        public MSGraphDeviceCodeAuthProvider(string appId, string[] scopes)
        {
            _scopes = scopes;

            _msalClient = PublicClientApplicationBuilder
                .Create(appId)
                .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount, true)
                .Build();
        }

        public async Task<string> GetAccessToken()
        {
            //If there is no saved user account, then the user must sign-in
            if (_userAccount == null)
            {
                Logging.Log("No account detected, prompting to sign-in", 'i');
                Console.WriteLine("No accounts detected! Would you like to sign in?[y/n]\n");
                char ToLogIn = char.Parse(Console.ReadLine());

                if (ToLogIn == 'y')
                {
                    try
                    {
                        //Invoke device code flow so user can sign in with a browser
                        var result = await _msalClient.AcquireTokenWithDeviceCode(_scopes, callback =>
                        {
                            Console.WriteLine(callback.Message);
                            return Task.FromResult(0);
                        }).ExecuteAsync();

                        _userAccount = result.Account;
                        return result.AccessToken;
                    }
                    catch (Exception exception)
                    {
                        Logging.Log($"Error getting access token: {exception.Message}", 'e');
                        return null;
                    }
                }
                else
                    return null;

            }
            else
            {
                // If there is an account, call AcquireTokenSilent
                // By doing this, MSAL will refresh the token automatically if
                // it is expired. Otherwise it returns the cached token.
                var result = await _msalClient
                    .AcquireTokenSilent(_scopes, _userAccount)
                    .ExecuteAsync();

                return result.AccessToken;
            }
        }
        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetAccessToken());
        }
    }
}

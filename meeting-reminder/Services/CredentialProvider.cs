using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingReminder.Services
{
    public interface ICredentialProvider
    {
        Task<UserCredential> GetUserCredentialAsync();
    }

    internal class CredentialProvider : ICredentialProvider
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };

        public async Task<UserCredential> GetUserCredentialAsync()
        {
            UserCredential credential;

            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            string credPath = "token.json";
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromFile("credentials.json").Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));

            Console.WriteLine("Credential file saved to: " + credPath);
            return credential;
        }
    }
}

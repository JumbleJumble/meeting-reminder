using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingReminder.Services;

public interface ICredentialProvider
{
    Task<UserCredential> GetUserCredentialAsync();
}

public class CredentialProvider : ICredentialProvider
{
    private readonly IConfigProvider configProvider;
    private static readonly string[] scopes = { CalendarService.Scope.CalendarReadonly };

    public CredentialProvider(IConfigProvider configProvider)
    {
        this.configProvider = configProvider;
    }

    public async Task<UserCredential> GetUserCredentialAsync()
    {
        UserCredential credential;

        var secretsFile = configProvider.Config.SecretsLocation;
        var secretsContainer = await GoogleClientSecrets.FromFileAsync(secretsFile);
        var credPath = "token.json";
        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secretsContainer.Secrets,
            scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true));

        Console.WriteLine("Credential file saved to: " + credPath);
        return credential;
    }
}

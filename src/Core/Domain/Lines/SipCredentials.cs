namespace CalloraVoipSdk.Core.Domain.Lines;

/// <summary>Immutable SIP authentication credentials.</summary>
public sealed record SipCredentials
{
    public string Username { get; }
    public string Password { get; }
    public string Realm    { get; }

    public SipCredentials(string username, string password, string realm = "")
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username required.", nameof(username));
        Username = username;
        Password = password ?? throw new ArgumentNullException(nameof(password));
        Realm    = realm;
    }
}

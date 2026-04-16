namespace CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

internal readonly record struct SipUriComponents(
    string Scheme,
    string User,
    string Host,
    int? Port,
    string Params,
    string Headers);

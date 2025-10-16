using Library.DTO;

namespace Library.Helper;

public static class AppleConstHelper
{
    public readonly static string TeamId = "JZD73SH4K5";
    public readonly static string KeyId = "P3T9747ZU9";
    public readonly static string AppSharedSecret = "d6368d390a2249a2a4c6d4fb4686554f";    
    public readonly static TimeSpan ExpiredMinutes = TimeSpan.FromMinutes(55);

    public static string GetBundleId(UserDeviceType userDeviceType)
    {
        var clientId = userDeviceType switch
        {
            UserDeviceType.Android => "com.crazydiamond.poppin.login",
            UserDeviceType.PC => "com.crazydiamond.poppin.login",
            UserDeviceType.iOS => "com.crazydiamond.poppin",
            _ => string.Empty
        };
        return clientId;
    }    
}

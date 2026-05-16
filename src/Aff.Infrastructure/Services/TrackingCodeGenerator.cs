namespace Aff.Infrastructure.Services;

public static class TrackingCodeGenerator
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string Generate(int length = 8)
    {
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => Chars[random.Next(Chars.Length)])
            .ToArray());
    }
}

namespace Wisp.Framework.Util;

public class Random
{
    private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();

    public static string RandomString(int length = 16)
    {
        var buffer = new char[length];

        for (var i = 0; i < length; i++)
        {
            buffer[i] = chars[System.Random.Shared.Next(chars.Length-1)];
        }
        
        return new string(buffer);
    }
}
namespace Wisp.Framework.Http;

public class File
{
    public string Filename { get; set; } = "";

    public string ContentType { get; set; } = "";

    public byte[] Data { get; set; } = [];
}
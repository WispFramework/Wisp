namespace Wisp.Extensions.Admin.Data;

public interface ICrudModel
{
    Guid Id { get; set; }

    string[] GetColumnValues();
}
using System.ComponentModel.DataAnnotations;

namespace Wisp.Extensions.Admin.Data;

public class CrudModel : ICrudModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public virtual string[] GetColumnValues()
    {
        return [];
    }
}
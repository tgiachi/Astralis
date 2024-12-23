using System.ComponentModel.DataAnnotations.Schema;
using Astralis.Core.Server.Interfaces.Entities;

namespace Astralis.Server.Entities;

[Table("accounts")]
public class AccountEntity : IBaseDbEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projector.DataBase;

[Table("Users")]
public class User
{
    [Key]
    [Column("id",Order=1)]
    public Guid Id { get; set; }

    [Required]
    [Column("name")]
    public String Name { get; set; } = "";

    [Timestamp]
    [Column("created")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime Created { get; set; }

    [Timestamp]
    [Column("modified")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime Modified { get; set; }
}
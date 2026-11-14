using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = null!;
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}

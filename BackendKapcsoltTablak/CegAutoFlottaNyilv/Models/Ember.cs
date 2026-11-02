using System.ComponentModel.DataAnnotations;

namespace CegAutoFlottaNyilv.Models;

public class Ember
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Nev { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string Osztaly { get; set; } = string.Empty;
    public List<Auto> Autok { get; set; } = new();
}

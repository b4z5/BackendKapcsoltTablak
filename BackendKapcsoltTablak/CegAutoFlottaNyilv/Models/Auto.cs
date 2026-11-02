using System.ComponentModel.DataAnnotations;

namespace CegAutoFlottaNyilv.Models;

public class Auto
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string Marka { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string Modell { get; set; } = string.Empty;
    [Required, StringLength(15)]
    public string Rendszam { get; set; } = string.Empty;
    [Range(1950, 2100)]
    public int Evjarat { get; set; }
    public int? EmberId { get; set; }
    public Ember? Ember { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Konyvtar.Modellek;

public class Olvaso
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Nev { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    public List<Konyv> Konyvek { get; set; } = new();
}

public class Konyv
{
    public int Id { get; set; }
    [Required, MaxLength(150)]
    public string Cim { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Szerzo { get; set; } = string.Empty;
    [Required, MaxLength(20)]
    public string ISBN { get; set; } = string.Empty;
    public int KiadasEve { get; set; }
    public int? OlvasoId { get; set; }
    public Olvaso? Olvaso { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Konyvtar.Modellek;

namespace Konyvtar.Adatatvitel;

public class OlvasoBemenet
{
    [Required, MaxLength(100)] public string Nev { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Email { get; set; } = string.Empty;
}

public class KonyvBemenet
{
    [Required, MaxLength(150)] public string Cim { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Szerzo { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string ISBN { get; set; } = string.Empty;
    public int KiadasEve { get; set; }
    public int? OlvasoId { get; set; }
}

public record OlvasoRovid(int Id, string Nev, string Email);
public record KonyvRovid(int Id, string Cim, string Szerzo, string ISBN, int KiadasEve, int? OlvasoId);
public record OlvasoValasz(int Id, string Nev, string Email, List<KonyvRovid> Konyvek);
public record KonyvValasz(int Id, string Cim, string Szerzo, string ISBN, int KiadasEve, int? OlvasoId, OlvasoRovid? Olvaso);

public static class Valasz
{
    public static KonyvRovid KonyvRovid(Konyv k) =>
        new(k.Id, k.Cim, k.Szerzo, k.ISBN, k.KiadasEve, k.OlvasoId);

    public static OlvasoValasz Olvaso(Olvaso o) =>
        new(o.Id, o.Nev, o.Email, o.Konyvek.OrderBy(k => k.Id).Select(KonyvRovid).ToList());

    public static KonyvValasz Konyv(Konyv k) =>
        new(k.Id, k.Cim, k.Szerzo, k.ISBN, k.KiadasEve, k.OlvasoId,
            k.Olvaso is null ? null : new OlvasoRovid(k.Olvaso.Id, k.Olvaso.Nev, k.Olvaso.Email));
}

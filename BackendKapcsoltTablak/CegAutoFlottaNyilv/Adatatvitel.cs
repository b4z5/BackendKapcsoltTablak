using System.ComponentModel.DataAnnotations;
using CegAutoFlottaNyilv.Models;

namespace CegAutoFlottaNyilv.Adatatvitel;

public class EmberBemenet
{
    [Required, StringLength(100)]
    public string Nev { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string Osztaly { get; set; } = string.Empty;
}

public class AutoBemenet
{
    [Required, StringLength(50)]
    public string Marka { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string Modell { get; set; } = string.Empty;
    [Required, StringLength(15)]
    public string Rendszam { get; set; } = string.Empty;
    [Range(1950, 2100)]
    public int Evjarat { get; set; }
    public int? EmberId { get; set; }
}

public record EmberRovid(int Id, string Nev, string Osztaly);
public record AutoRovid(int Id, string Marka, string Modell, string Rendszam, int Evjarat, int? EmberId);
public record EmberValasz(int Id, string Nev, string Osztaly, List<AutoRovid> Autok);
public record AutoValasz(int Id, string Marka, string Modell, string Rendszam, int Evjarat, int? EmberId, EmberRovid? Ember);

public static class Valasz
{
    public static AutoRovid AutoRovid(Auto a) =>
        new(a.Id, a.Marka, a.Modell, a.Rendszam, a.Evjarat, a.EmberId);

    public static EmberValasz Ember(Ember e) =>
        new(e.Id, e.Nev, e.Osztaly, e.Autok.OrderBy(a => a.Id).Select(AutoRovid).ToList());

    public static AutoValasz Auto(Auto a) =>
        new(a.Id, a.Marka, a.Modell, a.Rendszam, a.Evjarat, a.EmberId,
            a.Ember is null ? null : new EmberRovid(a.Ember.Id, a.Ember.Nev, a.Ember.Osztaly));
}

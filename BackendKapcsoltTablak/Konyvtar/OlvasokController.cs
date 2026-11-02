using Konyvtar.Adatatvitel;
using Konyvtar.Adatok;
using Konyvtar.Modellek;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Konyvtar.Vezerlok;

[ApiController]
[Route("api/olvasok")]
public class OlvasokController(KonyvtarContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Osszes()
    {
        var olvasok = await db.Olvasok.AsNoTracking()
            .Include(o => o.Konyvek).OrderBy(o => o.Id).ToListAsync();
        return Ok(olvasok.Select(Valasz.Olvaso));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Egy(int id)
    {
        var olvaso = await db.Olvasok.AsNoTracking()
            .Include(o => o.Konyvek).FirstOrDefaultAsync(o => o.Id == id);
        return olvaso is null ? NotFound() : Ok(Valasz.Olvaso(olvaso));
    }

    [HttpGet("{id:int}/konyvek")]
    public async Task<IActionResult> OlvasoKonyvei(int id)
    {
        if (!await db.Olvasok.AnyAsync(o => o.Id == id)) return NotFound();
        var konyvek = await db.Konyvek.AsNoTracking()
            .Where(k => k.OlvasoId == id).OrderBy(k => k.Id).ToListAsync();
        return Ok(konyvek.Select(Valasz.KonyvRovid));
    }

    [HttpGet("kolcsonzok")]
    public async Task<IActionResult> Kolcsonzok()
    {
        var olvasok = await db.Olvasok.AsNoTracking()
            .Where(o => o.Konyvek.Any()).Include(o => o.Konyvek)
            .OrderBy(o => o.Id).ToListAsync();
        return Ok(olvasok.Select(Valasz.Olvaso));
    }

    [HttpPost]
    public async Task<IActionResult> Felvetel([FromBody] OlvasoBemenet adat)
    {
        var olvaso = new Olvaso { Nev = adat.Nev.Trim(), Email = adat.Email.Trim() };
        db.Olvasok.Add(olvaso);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Egy), new { id = olvaso.Id }, Valasz.Olvaso(olvaso));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Modositas(int id, [FromBody] OlvasoBemenet adat)
    {
        var olvaso = await db.Olvasok.FindAsync(id);
        if (olvaso is null) return NotFound();
        olvaso.Nev = adat.Nev.Trim();
        olvaso.Email = adat.Email.Trim();
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Torles(int id)
    {
        var olvaso = await db.Olvasok.FindAsync(id);
        if (olvaso is null) return NotFound();
        db.Olvasok.Remove(olvaso);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

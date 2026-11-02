using Konyvtar.Adatatvitel;
using Konyvtar.Adatok;
using Konyvtar.Modellek;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Konyvtar.Vezerlok;

[ApiController]
[Route("api/konyvek")]
public class KonyvekController(KonyvtarContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Osszes()
    {
        var konyvek = await db.Konyvek.AsNoTracking().Include(k => k.Olvaso)
            .OrderBy(k => k.Id).ToListAsync();
        return Ok(konyvek.Select(Valasz.Konyv));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Egy(int id)
    {
        var konyv = await db.Konyvek.AsNoTracking().Include(k => k.Olvaso)
            .FirstOrDefaultAsync(k => k.Id == id);
        return konyv is null ? NotFound() : Ok(Valasz.Konyv(konyv));
    }

    [HttpGet("szabad")]
    public async Task<IActionResult> Szabad()
    {
        var konyvek = await db.Konyvek.AsNoTracking().Where(k => k.OlvasoId == null)
            .OrderBy(k => k.Id).ToListAsync();
        return Ok(konyvek.Select(Valasz.Konyv));
    }

    [HttpGet("kereses")]
    public async Task<IActionResult> Kereses([FromQuery] string? szoveg)
    {
        if (string.IsNullOrWhiteSpace(szoveg)) return BadRequest("A szoveg paraméter kötelező.");
        var minta = $"%{szoveg.Trim()}%";
        var konyvek = await db.Konyvek.AsNoTracking().Include(k => k.Olvaso)
            .Where(k => EF.Functions.Like(k.Cim, minta) || EF.Functions.Like(k.Szerzo, minta))
            .OrderBy(k => k.Id).ToListAsync();
        return Ok(konyvek.Select(Valasz.Konyv));
    }

    [HttpPost]
    public async Task<IActionResult> Felvetel([FromBody] KonyvBemenet adat)
    {
        if (adat.OlvasoId is int olvasoId && !await db.Olvasok.AnyAsync(o => o.Id == olvasoId))
            return BadRequest("A megadott OlvasoId-hoz nem tartozik olvasó.");

        var isbn = adat.ISBN.Trim();
        if (await db.Konyvek.AnyAsync(k => k.ISBN == isbn))
            return BadRequest("Ez az ISBN már szerepel az adatbázisban.");

        var konyv = new Konyv
        {
            Cim = adat.Cim.Trim(), Szerzo = adat.Szerzo.Trim(),
            ISBN = isbn, KiadasEve = adat.KiadasEve, OlvasoId = adat.OlvasoId
        };
        db.Konyvek.Add(konyv);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteErrorCode: 19 })
        {
            return BadRequest("ISBN-ütközés vagy érvénytelen hozzárendelés.");
        }
        if (konyv.OlvasoId.HasValue)
            konyv.Olvaso = await db.Olvasok.AsNoTracking().FirstOrDefaultAsync(o => o.Id == konyv.OlvasoId.Value);
        return CreatedAtAction(nameof(Egy), new { id = konyv.Id }, Valasz.Konyv(konyv));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Modositas(int id, [FromBody] KonyvBemenet adat)
    {
        var konyv = await db.Konyvek.FindAsync(id);
        if (konyv is null) return NotFound();
        if (adat.OlvasoId is int olvasoId && !await db.Olvasok.AnyAsync(o => o.Id == olvasoId))
            return BadRequest("A megadott OlvasoId-hoz nem tartozik olvasó.");
        if (konyv.OlvasoId.HasValue && adat.OlvasoId.HasValue && konyv.OlvasoId != adat.OlvasoId)
            return BadRequest("A könyv már ki van kölcsönözve. Előbb vissza kell hozni (OlvasoId = null).");

        var isbn = adat.ISBN.Trim();
        if (await db.Konyvek.AnyAsync(k => k.Id != id && k.ISBN == isbn))
            return BadRequest("Ez az ISBN már szerepel az adatbázisban.");

        konyv.Cim = adat.Cim.Trim();
        konyv.Szerzo = adat.Szerzo.Trim();
        konyv.ISBN = isbn;
        konyv.KiadasEve = adat.KiadasEve;
        konyv.OlvasoId = adat.OlvasoId;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteErrorCode: 19 })
        {
            return BadRequest("ISBN-ütközés vagy érvénytelen hozzárendelés.");
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Torles(int id)
    {
        var konyv = await db.Konyvek.FindAsync(id);
        if (konyv is null) return NotFound();
        db.Konyvek.Remove(konyv);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

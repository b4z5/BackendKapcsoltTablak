using CegAutoFlottaNyilv.Adatatvitel;
using CegAutoFlottaNyilv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CegAutoFlottaNyilv.Controllers;

[ApiController]
[Route("api/autok")]
public class AutokController(CegAutoFlottaNyilvContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Osszes()
    {
        var autok = await db.Auto.AsNoTracking().Include(a => a.Ember)
            .OrderBy(a => a.Id).ToListAsync();
        return Ok(autok.Select(Valasz.Auto));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Egy(int id)
    {
        var auto = await db.Auto.AsNoTracking().Include(a => a.Ember)
            .FirstOrDefaultAsync(a => a.Id == id);
        return auto is null ? NotFound() : Ok(Valasz.Auto(auto));
    }

    [HttpGet("kiosztatlan")]
    public async Task<IActionResult> Kiosztatlan()
    {
        var autok = await db.Auto.AsNoTracking().Where(a => a.EmberId == null)
            .OrderBy(a => a.Id).ToListAsync();
        return Ok(autok.Select(Valasz.Auto));
    }

    [HttpGet("kereses")]
    public async Task<IActionResult> Kereses([FromQuery] string? szoveg)
    {
        if (string.IsNullOrWhiteSpace(szoveg)) return BadRequest("A szoveg parameter kotelezo.");
        szoveg = szoveg.Trim();
        var autok = await db.Auto.AsNoTracking().Include(a => a.Ember)
            .Where(a => a.Marka.Contains(szoveg) ||
                        a.Modell.Contains(szoveg) ||
                        a.Rendszam.Contains(szoveg))
            .OrderBy(a => a.Id).ToListAsync();
        return Ok(autok.Select(Valasz.Auto));
    }

    [HttpPost]
    public async Task<IActionResult> Felvetel([FromBody] AutoBemenet adat)
    {
        if (adat.EmberId is int emberId && !await db.Ember.AnyAsync(e => e.Id == emberId))
            return BadRequest("A megadott EmberId-hoz nem tartozik ember.");

        var rendszam = adat.Rendszam.Trim();
        if (await db.Auto.AnyAsync(a => a.Rendszam == rendszam))
            return BadRequest("Ez a rendszam mar szerepel az adatbazisban.");

        var auto = new Auto
        {
            Marka = adat.Marka.Trim(),
            Modell = adat.Modell.Trim(),
            Rendszam = rendszam,
            Evjarat = adat.Evjarat,
            EmberId = adat.EmberId
        };
        db.Auto.Add(auto);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 or 547 })
        {
            return BadRequest("Rendszam-utkozes vagy ervenytelen hozzarendeles.");
        }
        if (auto.EmberId.HasValue)
            auto.Ember = await db.Ember.AsNoTracking().FirstOrDefaultAsync(e => e.Id == auto.EmberId.Value);
        return CreatedAtAction(nameof(Egy), new { id = auto.Id }, Valasz.Auto(auto));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Modositas(int id, [FromBody] AutoBemenet adat)
    {
        var auto = await db.Auto.FindAsync(id);
        if (auto is null) return NotFound();
        if (adat.EmberId is int emberId && !await db.Ember.AnyAsync(e => e.Id == emberId))
            return BadRequest("A megadott EmberId-hoz nem tartozik ember.");

        var rendszam = adat.Rendszam.Trim();
        if (await db.Auto.AnyAsync(a => a.Id != id && a.Rendszam == rendszam))
            return BadRequest("Ez a rendszam mar szerepel az adatbazisban.");

        auto.Marka = adat.Marka.Trim();
        auto.Modell = adat.Modell.Trim();
        auto.Rendszam = rendszam;
        auto.Evjarat = adat.Evjarat;
        auto.EmberId = adat.EmberId;
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 or 547 })
        {
            return BadRequest("Rendszam-utkozes vagy ervenytelen hozzarendeles.");
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Torles(int id)
    {
        var auto = await db.Auto.FindAsync(id);
        if (auto is null) return NotFound();
        db.Auto.Remove(auto);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

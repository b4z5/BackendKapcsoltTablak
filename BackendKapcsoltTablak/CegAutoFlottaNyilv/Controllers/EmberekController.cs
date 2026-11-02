using CegAutoFlottaNyilv.Adatatvitel;
using CegAutoFlottaNyilv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegAutoFlottaNyilv.Controllers;

[ApiController]
[Route("api/emberek")]
public class EmberekController(CegAutoFlottaNyilvContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Osszes()
    {
        var emberek = await db.Ember.AsNoTracking()
            .Include(e => e.Autok).OrderBy(e => e.Id).ToListAsync();
        return Ok(emberek.Select(Valasz.Ember));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Egy(int id)
    {
        var ember = await db.Ember.AsNoTracking()
            .Include(e => e.Autok).FirstOrDefaultAsync(e => e.Id == id);
        return ember is null ? NotFound() : Ok(Valasz.Ember(ember));
    }

    [HttpGet("{id:int}/autok")]
    public async Task<IActionResult> EmberAutoi(int id)
    {
        if (!await db.Ember.AnyAsync(e => e.Id == id)) return NotFound();
        var autok = await db.Auto.AsNoTracking()
            .Where(a => a.EmberId == id).OrderBy(a => a.Id).ToListAsync();
        return Ok(autok.Select(Valasz.AutoRovid));
    }

    [HttpPost]
    public async Task<IActionResult> Felvetel([FromBody] EmberBemenet adat)
    {
        var ember = new Ember { Nev = adat.Nev.Trim(), Osztaly = adat.Osztaly.Trim() };
        db.Ember.Add(ember);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Egy), new { id = ember.Id }, Valasz.Ember(ember));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Modositas(int id, [FromBody] EmberBemenet adat)
    {
        var ember = await db.Ember.FindAsync(id);
        if (ember is null) return NotFound();
        ember.Nev = adat.Nev.Trim();
        ember.Osztaly = adat.Osztaly.Trim();
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Torles(int id)
    {
        var ember = await db.Ember.FindAsync(id);
        if (ember is null) return NotFound();
        db.Ember.Remove(ember);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

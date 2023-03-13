using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoteApi.Data;
using SharedLib;

namespace QuoteApi.Controllers;

[Route("[controller]")]
[ApiController]
public class QuotesController : ControllerBase
{
    private readonly QuoteContext _context;

    public QuotesController(QuoteContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuoteDTO>>> Get()
    {
        var quotes = await _context.Quotes
            .OrderByDescending(i => i.QuoteCreateDate)
            .Take(5)
            .Select(i => MapToQuoteDto(i))
            .ToListAsync();

        return quotes;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<List<QuoteDTO>>> Get(string name)
    {
        var quotes = await _context.Quotes
            .Where(i => i.QuoteCreatorNormalized == name.ToUpper())
            .Select(i => MapToQuoteDto(i))
            .ToListAsync();

        return quotes;
    }

    [HttpGet("{name}/{id}")]
    public async Task<ActionResult<QuoteDTO>> Get(string name, int id)
    {
        var quote = await _context.Quotes
            .FirstOrDefaultAsync(i => i.QuoteCreatorNormalized == name.ToUpper() && i.Id == id);

        if (quote == null)
        {
            return NotFound();
        }

        return MapToQuoteDto(quote);
    }

    [HttpPost("{name}")]
    public async Task<ActionResult<QuoteDTO>> Post(string name, QuoteDTO quote)
    {
        var id = _context.Quotes.Max(i => i.Id) + 1;
        var q = new Quote
        {
            Id = id,
            QuoteCreateDate = DateTime.Now,
            QuoteCreator = name,
            QuoteCreatorNormalized = name.ToUpper(),
            TheQuote = quote.Quote,
            WhenWasSaid = quote.When,
            WhoSaid = quote.SaidBy
        };

        _context.Quotes.Add(q);
        await _context.SaveChangesAsync();

        return CreatedAtAction("Get", new { id = q.Id, name = q.QuoteCreator }, MapToQuoteDto(q));
    }

    [HttpPut("{name}/{id}")]
    public async Task<IActionResult> Put(string name, int id, QuoteDTO quoteInput)
    {
        var quote = await _context.Quotes
            .FirstOrDefaultAsync(i => i.Id == id && i.QuoteCreatorNormalized == name.ToUpper());

        if (quote == null)
        {
            return NotFound();
        }

        quote.TheQuote = quoteInput.Quote;
        quote.WhoSaid = quoteInput.SaidBy;
        quote.WhenWasSaid = quoteInput.When;

        _context.Entry(quote).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{name}/{id}")]
    public async Task<IActionResult> Delete(string name, int id)
    {
        var quote = await _context.Quotes
            .FirstOrDefaultAsync(i => i.Id == id && i.QuoteCreatorNormalized == name.ToUpper());

        if (quote == null)
        {
            return NotFound();
        }

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static QuoteDTO MapToQuoteDto(Quote quote)
    {
        return new QuoteDTO
        {
            Id = quote.Id,
            Quote = quote.TheQuote,
            SaidBy = quote.WhoSaid,
            When = quote.WhenWasSaid
        };
    }
}
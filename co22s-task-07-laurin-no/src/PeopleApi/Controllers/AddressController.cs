using Microsoft.AspNetCore.Mvc;
using PeopleLib;
using PeopleApi.Data;
using Microsoft.EntityFrameworkCore;

namespace PeopleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressController : ControllerBase
{
    private readonly ILogger<AddressController> _logger;
    private readonly PeopleContext _db;

    public AddressController(ILogger<AddressController> logger, PeopleContext context)
    {
        _logger = logger;
        _db = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Address>> Get()
    {
        return await _db.Addresses.ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Address model)
    {
        _db.Addresses.Add(model);
        await _db.SaveChangesAsync();

        // Note that 200 return value is not the proper returned value from HTTP POST action.
        // This is a valid return value for common http request.
        // This is simplified so that answer for Task 06 is not shown here.
        return Ok(); // return 200 Ok response
    }

}

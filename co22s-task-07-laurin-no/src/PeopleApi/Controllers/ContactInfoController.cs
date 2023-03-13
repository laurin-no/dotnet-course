using Microsoft.AspNetCore.Mvc;
using PeopleLib;
using PeopleApi.Data;
using Microsoft.EntityFrameworkCore;

namespace PeopleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactInfoController : ControllerBase
{
    private readonly ILogger<ContactInfoController> _logger;
    private readonly PeopleContext _db;

    public ContactInfoController(ILogger<ContactInfoController> logger, PeopleContext context)
    {
        _logger = logger;
        _db = context;
    }

    [HttpGet]
    public async Task<IEnumerable<ContactInfo>> Get()
    {
        return await _db.ContactInfos.ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ContactInfo model)
    {
        _db.ContactInfos.Add(model);
        await _db.SaveChangesAsync();

        // Note that 200 return value is not the proper returned value from HTTP POST action.
        // This is a valid return value for common http request.
        // This is simplified so that answer for Task 06 is not shown here.
        return Ok(); // return 200 Ok response
    }

}

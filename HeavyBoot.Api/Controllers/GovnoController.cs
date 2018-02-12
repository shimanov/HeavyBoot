using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HeavyBoot.Api.Model;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HeavyBoot.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Govno")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
    public class GovnoController : Controller
    {
        ConnectionToDb connectionDb;

        public GovnoController(ConnectionToDb connection)
        {
            connectionDb = connection;
        }

        [HttpGet]
        public IEnumerable<HBDataTable> Get()
        {
            return connectionDb.HbDataTables.ToList();
        }

        [HttpGet("{pcname}", Name = "GetPcname")]
        public async Task<IActionResult> GetByPcname(string pcname)
        {
            var result = await connectionDb.HbDataTables.FirstAsync(x => x.Pcname == pcname);
            if (result == null)
            {
                return NotFound();
            }
            return new OkObjectResult(result);
        }

        [HttpPut("{pcname}")] //put
        public async Task<IActionResult> Update(string pcname, [FromBody] HBDataTable table)
        {
            if (table == null /*|| table.Pcname != pcname*/)
            {
                return BadRequest();
            }

            var result = await connectionDb.HbDataTables.FirstOrDefaultAsync(x => x.Pcname == pcname);
            if (result == null)
            {
                return NotFound();
            }

            result.ClientTime = table.ClientTime;
            result.DateServer = table.DateServer;
            result.ExportTime = table.ExportTime;
            result.ImportTime = table.ImportTime;
            result.TimeOut = table.TimeOut;
            result.IsChecked = table.IsChecked;

            connectionDb.HbDataTables.Update(result);
            connectionDb.SaveChanges();

            return new NoContentResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HBDataTable table)
        {
            if (table == null)
            {
                return BadRequest();
            }
            connectionDb.HbDataTables.Add(table);
            await connectionDb.SaveChangesAsync();

            return CreatedAtRoute(new { pcname = table.Pcname }, table);
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HeavyBoot.Api.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HeavyBoot.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Govno")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
    public class GovnoController : Controller
    {
        private readonly ConnectionToDb _connectionDb;
        private readonly IHostingEnvironment _environment;

        public GovnoController(ConnectionToDb connection, IHostingEnvironment environment)
        {
            _connectionDb = connection;
            _environment = environment;
        }

        [HttpGet]
        public IEnumerable<HBDataTable> Get()
        {
            return _connectionDb.HbDataTables.ToList();
        }

        [HttpGet("{pcname}", Name = "GetPcname")]
        public async Task<IActionResult> GetByPcname(string pcname)
        {
            var result = await _connectionDb.HbDataTables.FirstAsync(x => x.Pcname == pcname);
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

            var result = await _connectionDb.HbDataTables.FirstOrDefaultAsync(x => x.Pcname == pcname);
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

            _connectionDb.HbDataTables.Update(result);
            _connectionDb.SaveChanges();

            return new NoContentResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HBDataTable table)
        {
            if (table == null)
            {
                return BadRequest();
            }
            _connectionDb.HbDataTables.Add(table);
            await _connectionDb.SaveChangesAsync();

            return CreatedAtRoute(new { pcname = table.Pcname }, table);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                string path = $"//Files//{file.FileName}";
                using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            return NoContent();
        }
    }
}
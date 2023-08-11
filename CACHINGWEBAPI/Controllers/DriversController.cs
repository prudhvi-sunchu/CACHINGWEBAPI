using CACHINGWEBAPI.Data;
using CACHINGWEBAPI.Models;
using CACHINGWEBAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CACHINGWEBAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriversController : ControllerBase
    {


        private readonly ILogger<DriversController> _logger;
        private readonly ICacheService _cacheService;  // injected cacheservice in our application dbcontext so that we can utilize it
        private readonly AppDbContext _context;

        public DriversController(      //we just injecting a new interface in our controller
            ILogger<DriversController> logger,
            ICacheService cacheService,
            AppDbContext context)
        {
            _logger = logger;
            _cacheService = cacheService;
            _context = context;
        }

        [HttpGet("drivers")]  //building end points 
        public async Task<IActionResult> Get() 
        {
            //check cache data
            var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers"); //basically we need to connect the cache service need to get the data and passing the annomous type a list of drivers and iam giving the key which gonna be drivers

            if (cacheData != null && cacheData.Count() > 0)
                return Ok(cacheData); // all we are doing is basically checking our cache does this information exist in our cache yes or no incase yes make sure not null and we need to check it is not empty string and based on that we are serializing and returning every thing back to the user

            //so the next step is if it fails we need to get every thing form the database

            cacheData = await _context.Drivers.ToListAsync();


            //Set expiry time
            var expiryTime = DateTimeOffset.Now.AddDays(1);
            _cacheService.SetData<IEnumerable<Driver>>("drivers", cacheData, expiryTime); //(cacheData is the result that we wanna store)

            return Ok(cacheData);
        }

        //Add the data
        [HttpPost("AddDriver")]
        public async Task<IActionResult> Post(Driver value)
        {
            var addedObj = await _context.Drivers.AddAsync(value);

            var expiryTime = DateTimeOffset.Now.AddDays(1);
            _cacheService.SetData<Driver>($"driver{value.Id}", addedObj.Entity, expiryTime);

            await _context.SaveChangesAsync();

            return Ok(addedObj.Entity);
        }


        //Delete from cache
        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult>  Delete(int id)
        {
            var exist = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id); //first we just check weather it exist or not

            if (exist != null)
            {
                _context.Remove(exist);
                _cacheService.RemoveData($"driver{id}");
                await _context.SaveChangesAsync();
                
                return NoContent();
            }

            return NotFound();
        }

    }
}
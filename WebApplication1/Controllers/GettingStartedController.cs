using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static ConnectionMultiplexer _connectionMultiplexer;
        private static IDatabase _db;
        static ValuesController()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect("localhost:6379");
            _db = _connectionMultiplexer.GetDatabase();
            _db.StringGetSet("Behnam", 1);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var taskid = Guid.NewGuid().ToString();
            try
            {
                var expiry = TimeSpan.FromSeconds(1);
                var wait = TimeSpan.FromSeconds(1);
                var retry = TimeSpan.FromSeconds(1);

                var multiplexers = new List<RedLockMultiplexer>
            {
                _connectionMultiplexer
            };
                var redLockFactory = RedLockFactory.Create(multiplexers);

                using var redLock = await redLockFactory.CreateLockAsync("Behnam", expiry, wait, retry);

                var id = "";
                if (redLock.IsAcquired)
                {
                    Console.WriteLine($"Enter:{taskid}");
                    id = _db.StringGet("Behnam");
                    id = (Convert.ToInt32(id) + 1).ToString();
                    _db.StringGetSet("Behnam", id);
                    Console.WriteLine($"EnterValue:{taskid}-{id}");
                }
                else
                {
                    Console.WriteLine($"Wait:{taskid}");
                }




                return Ok();
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}

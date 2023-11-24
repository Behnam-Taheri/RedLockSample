using Microsoft.AspNetCore.Mvc;
using RedLockNet;
using StackExchange.Redis;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedLocksController : ControllerBase
    {
        private readonly IDistributedLockFactory _lockFactory;
        private readonly IDatabase _database;

        public RedLocksController(IDistributedLockFactory lockFactory, IDatabase database)
        {
            _lockFactory = lockFactory;
            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid resourceId)
        {
            var taskid = Guid.NewGuid().ToString();

            using var redLock = await _lockFactory.CreateLockAsync(resourceId.ToString(), LockOption.Expiry, LockOption.Wait, LockOption.Retry);

            var id = "";
            if (redLock.IsAcquired)
            {
                Console.WriteLine($"Enter:{taskid}");
                id = await _database.StringGetAsync(resourceId.ToString());
                if (string.IsNullOrEmpty(id))
                    id = "0";

                id = (Convert.ToInt32(id) + 1).ToString();
                await _database.StringGetSetAsync(resourceId.ToString(), id);
                Console.WriteLine($"EnterValue:{taskid}-{id}");
            }
            else
                Console.WriteLine($"Wait:{taskid}");

            return Ok();
        }
    }
}

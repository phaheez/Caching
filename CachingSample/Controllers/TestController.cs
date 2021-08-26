using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CachingSample.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CachingSample.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IMemoryCache _memoryCache;

        public TestController(ILogger<TestController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet("cache/get/countries")]
        public async Task<IActionResult> GetCountries()
        {
            const string cacheKey = "ListOfCountries";

            if (!_memoryCache.TryGetValue(cacheKey, out List<Country> countries))
            {
                countries = await CountriesService.GetAllCountries();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(6), //expire in 6 hours
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(10), //evict if not accessed for 10 minutes
                    Size = 1024 //size 1MB
                };

                _memoryCache.Set(cacheKey, countries, cacheEntryOptions);
            }

            return Ok(countries);
        }

        [HttpPost("cache/add")]
        public async Task<IActionResult> AddCache()
        {
            var countries = await CountriesService.GetAllCountries();

            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromMinutes(2),
                Size = 1024,
            };
            _memoryCache.Set("ListOfCountries", countries, cacheExpiryOptions);
            _memoryCache.TryGetValue("ListOfCountries", out List<Country> values);

            return Ok(values);
        }

        [HttpGet("cache/get")]
        public IActionResult GetCache()
        {
            _memoryCache.TryGetValue("ListOfCountries", out List<Country> values);

            return Ok(values);
        }
        
        [HttpPost("cache/remove")]
        public IActionResult RemoveCache()
        {
            _memoryCache.Remove("ListOfCountries");
            _memoryCache.TryGetValue("ListOfCountries", out List<Country> values);

            return Ok(values);
        }
    }
}

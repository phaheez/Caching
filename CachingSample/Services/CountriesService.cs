using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CachingSample.Services
{
    public static class CountriesService
    {
        public static async Task<List<Country>> GetAllCountries()
        {
            var file = await System.IO.File.ReadAllTextAsync("countries.json");
            var countries = JsonConvert.DeserializeObject<List<Country>>(file);

            return countries;
        }
    }
}

using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IDistributedCache _distributedCache; 

        public ProductsController(IDistributedCache distributedCache)
        {
                _distributedCache = distributedCache;
        }
        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddMinutes(1);
            //_distributedCache.SetString("name", "barno", options);
            //await _distributedCache.SetStringAsync("surname", "dogan", options);

            Product product = new Product { Id = 1, Name = "Pen", Price = 100 };

            string jsonProduct = JsonConvert.SerializeObject(product);
            Byte[] byteCache = Encoding.UTF8.GetBytes(jsonProduct);
            _distributedCache.Set("product:1", byteCache);
            //await _distributedCache.SetStringAsync("product:1", jsonProduct, options);

            return View();
        }

        public IActionResult Show()
        {
            Byte[] byteProduct = _distributedCache.Get("product:1");
            string jsonProduct = Encoding.UTF8.GetString(byteProduct);  
            Product p = JsonConvert.DeserializeObject<Product>(jsonProduct);
            ViewBag.Show = p;
            //ViewBag.Show = _distributedCache.GetString("name");
            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("resim");
            return View();
        }

        public IActionResult ImageUrl()
        {
            byte[] resimByte = _distributedCache.Get("resim");
            if (resimByte != null)
                return File(resimByte, "image/jpg");

            else
                return View();
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/indir.jpg");
            byte[] imageByte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("resim", imageByte);
            return View();
        }
    }
}

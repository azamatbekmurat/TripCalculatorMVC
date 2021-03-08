using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TripCalculatorMVC.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TripCalculatorMVC.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
               
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //Method to make calculation. Calculations are made per session
        public IActionResult Index()
        {
            ViewData["luiName"] = "Luis";
            ViewData["carterName"] = "Carter";
            ViewData["davidName"] = "David";

            List<Double> luisList = HttpContext.Session.GetObjectFromJson<List<Double>>("Luis") ?? new List<double>();
            List<Double> carterList = HttpContext.Session.GetObjectFromJson<List<Double>>("Carter") ?? new List<double>();
            List<Double> davidList = HttpContext.Session.GetObjectFromJson<List<Double>>("David") ?? new List<double>();

            Double luisTotal = luisList.Sum();
            Double carterTotal = carterList.Sum();
            Double davidTotal = davidList.Sum();

            Double overallSum = luisTotal + carterTotal + davidTotal;
            Double eachPersonPay = overallSum / 3;

            Double luisPay = luisTotal - eachPersonPay;
            Double carterPay = carterTotal - eachPersonPay;
            Double davidPay = davidTotal - eachPersonPay;

            ViewData["lui"] = luisPay;
            ViewData["carter"] = carterPay;
            ViewData["david"] = davidPay;

            ViewData["luiABS"] = Math.Truncate(Math.Abs(luisPay) * 100) / 100;
            ViewData["carterABS"] = Math.Truncate(Math.Abs(carterPay) * 100) / 100;
            ViewData["davidABS"] = Math.Truncate(Math.Abs(davidPay) * 100) / 100;


            return View(new TripCalculatorDetails());
        }

        //All data is stored into List<Double> within HttpContext.Session. 
        [HttpPost]
        public IActionResult Create(TripCalculatorDetails model)
        {
            List<Double> luisList = HttpContext.Session.GetObjectFromJson<List<Double>>("Luis")??new List<double>();
            List<Double> carterList = HttpContext.Session.GetObjectFromJson<List<Double>>("Carter") ?? new List<double>();
            List<Double> davidList = HttpContext.Session.GetObjectFromJson<List<Double>>("David") ?? new List<double>();

            luisList.Add(model.LuisExpense);
            carterList.Add(model.CarterExpense);
            davidList.Add(model.DavidExpense);

            HttpContext.Session.SetObjectAsJson("Luis",luisList);
            HttpContext.Session.SetObjectAsJson("Carter", carterList);
            HttpContext.Session.SetObjectAsJson("David", davidList);

            return RedirectToAction("Index","Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

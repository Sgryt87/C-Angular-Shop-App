using System;
using System.Linq;
using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DutchTreat.Controllers
{
    public class AppController : Controller
    {
        private readonly IMailService _mailService;
        public readonly DutchContext _context;

        public AppController(IMailService mailService, DutchContext context)
        {
            _mailService = mailService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("contact")]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                _mailService.SendMessage(
                    "sergiigrytsaienko@gmail.com",
                    model.Subject,
                    $"From: {model.Name} - {model.Email}, Message: {model.Message}"
                );
                ViewBag.UserMessage = "Mail Sent";
                ModelState.Clear();
            }

            return View();
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            ViewBag.Title = "About Us";
            return View();
        }

        public IActionResult Shop()
        {
            ViewBag.Title = "About Us";
            var results = _context.Products
                .OrderBy(p => p.Category)
                .ToList();

//            2nd option            
//            var results = from p in _context.Products
//                orderby p.Category
//                select p;

            return View(results.ToList());
        }
    }
}
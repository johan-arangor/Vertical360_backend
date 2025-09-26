using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Domain.Enums;
using Vertical360_back.Infrastructure.Persistence;
using Vertical360_back.Infrastructure.Security;
using Vertical360_back.Models;

namespace Vertical360_back.Api.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var modelo = await ConstructHomeIndex();

        return View(modelo);
    }

    [HttpPost]
    [HasPermission(Permissions.Product_Create)]
    public async Task<IActionResult> Index(Products products)
    {
        _context.Add(products);
        await _context.SaveChangesAsync();

        var modelo = await ConstructHomeIndex();

        return View(modelo);
    }

    private async Task<HomeIndexViewModel> ConstructHomeIndex()
    {
        var products = await _context.Products.ToListAsync();
        var countries = await _context.Countries.ToListAsync();

        var modelo = new HomeIndexViewModel();
        modelo.Products = products;
        modelo.Countries = countries;

        return modelo;
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

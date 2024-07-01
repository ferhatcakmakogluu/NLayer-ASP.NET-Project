using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _services;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService services, ICategoryService categoryService, IMapper mapper)
        {
            _services = services;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            //service de customeResponseDto döndüğümüz için
            //sadece dataları aldık return içinde
            var customeResponse = await _services.GetProductsWithCategory();
            return View(customeResponse.Data);
        }

        public async Task<IActionResult> Save()
        {
            var categories = await _categoryService.GetAllAsync();
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());

            //ViewBag ile bir liste aç, bu liste dtodan gelcek, tıklandıgında id yi alcak, kullanıcılar name görcek
            ViewBag.categories = new SelectList(categoriesDto, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
            if(ModelState.IsValid)
            {
                await _services.AddAsync(_mapper.Map<Product>(productDto));
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllAsync();
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());
            ViewBag.categories = new SelectList(categoriesDto, "Id", "Name");
            return View();
        }
    }
}

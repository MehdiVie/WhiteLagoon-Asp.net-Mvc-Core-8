using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AmenityController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork= unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var villas = _unitOfWork.Amenity.GetAll(includeProperties: "Villa");
            return View(villas);
        }
        public IActionResult Create()
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            //ViewData["VillaList"] = list;
            //ViewBag.VillaList = list;

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityVM obj)
        {

            // ModelState.Remove("Villa");
            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Add(obj.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "Amenity has been created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Amenity could not be created!";
            return View();

        }
        public IActionResult Update(int amenityId)
        {
            Amenity? obj = _unitOfWork.Amenity.Get(u => u.Id == amenityId);

            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }

            AmenityVM amenityVM = new()
            {
                Amenity = obj,
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM obj)
        {
            bool check = _unitOfWork.Amenity.Any(u => u.Id == obj.Amenity.Id);

            if (ModelState.IsValid && check)
            {
                _unitOfWork.Amenity.Update(obj.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "Amenity has been updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Amenity could not be updated!";
            return View();
        }

        public IActionResult Delete(int amenityId)
        {
            Amenity? obj = _unitOfWork.Amenity.Get(u => u.Id == amenityId);

            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }

            AmenityVM amenityVM = new()
            {
                Amenity = obj,
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM obj)
        {
            bool check = _unitOfWork.Amenity.Any(u => u.Id == obj.Amenity.Id);

            if (check)
            {
                _unitOfWork.Amenity.Remove(obj.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "Amenity has been deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Villa could not be deleted!";
            return View();
        }
    }
}

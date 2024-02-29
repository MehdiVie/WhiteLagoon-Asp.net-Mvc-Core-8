using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var villas = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa").OrderBy(x => x.VillaId);
            return View(villas);
        }
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            //ViewData["VillaList"] = list;
            //ViewBag.VillaList = list;

            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            //ModelState.Remove("Villa");
            bool roomNumberExists = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

            if (roomNumberExists)
            {
                TempData["error"] = "This VillaNumber exists!";
                obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.VillaNumber.Add(obj.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "VillaNumber has been created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "VillaNumber could not be created!";
            return View();
            
        }
        public IActionResult Update(int villaNumberId)
        {
            VillaNumber? obj = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }

            VillaNumberVM villaNumberVM = new()
            {
                VillaNumber = obj,
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM obj)
        {
            

            if (ModelState.IsValid)
            {
                _unitOfWork.VillaNumber.Update(obj.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "VillaNumber has been updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);

        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumber? obj = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId);
            if (obj is null)
            {
                 return RedirectToAction("Error", "Home");
            }

            VillaNumberVM villaNumberVM = new()
            {
                VillaNumber = obj,
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM obj)
        {
            VillaNumber? objFromDb = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == obj.VillaNumber.Villa_Number);

            if (objFromDb is not null)
            {
                _unitOfWork.VillaNumber.Remove(objFromDb);
                _unitOfWork.Save();
                TempData["success"] = "VillaNumber has been deleted successfully!";
                return RedirectToAction(nameof(Index));

            }
            TempData["error"] = "VillaNumber could not be deleted!";
            return View();

        }
    }
}

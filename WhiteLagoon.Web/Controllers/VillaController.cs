using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork= unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
            return View(villas);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if (obj.Name.ToLower().ToString() == obj.Description?.ToLower().ToString())
            {
                ModelState.AddModelError("Name","Description and Name could not be the same!");
            }
            if (ModelState.IsValid)
            {
                if(obj.Image is not null)
                {
                    
                    string fileName= Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Static_Files\Images\Villa-Images");

                    using var fileStream = new FileStream(Path.Combine(imagePath,fileName),FileMode.Create);

                    obj.Image.CopyTo(fileStream);
                    obj.ImageUrl = @"Static_Files\Images\Villa-Images\" + fileName;

                }
                else
                {
                    obj.ImageUrl= @"Static_Files\Images\Villa-Images\default.jpg";
                }

                _unitOfWork.Villa.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Villa has been created successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Villa could not be created!";
            return View();
            
        }
        public IActionResult Update(int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(obj);
        }
        [HttpPost]
        public IActionResult Update(Villa obj)
        {
            if (obj.Name.ToLower().ToString() == obj.Description?.ToLower().ToString())
            {
                ModelState.AddModelError("Name", "Description and Name could not be the same!");
                return View(obj);
            }
            if (ModelState.IsValid && obj.Id>0)
            {
                if (obj.Image is not null)
                {
                    
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Static_Files\Images\Villa-Images");

                    if (obj.ImageUrl != "Static_Files\\Images\\Villa-Images\\default.jpg")
                    {
                        var oldImagePath= Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl);

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);

                    obj.Image.CopyTo(fileStream);
                    obj.ImageUrl = @"Static_Files\Images\Villa-Images\" + fileName;

                }
                
                _unitOfWork.Villa.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Villa has been updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Villa could not be updated!";
            return View();
        }

        public IActionResult Delete(int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            Villa? objFromDb = _unitOfWork.Villa.Get(u => u.Id == obj.Id);

            if (objFromDb is not null)
            {
                _unitOfWork.Villa.Remove(objFromDb);
                _unitOfWork.Save();
                TempData["success"] = "Villa has been deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Villa could not be deleted!";
            return View();
        }
    }
}

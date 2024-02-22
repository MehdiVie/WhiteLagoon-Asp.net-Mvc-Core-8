using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class AmenityVM
    {
        public Amenity? Amenity { get; set; }
        public IEnumerable<SelectListItem>? VillaList { get; set; }
    }
}
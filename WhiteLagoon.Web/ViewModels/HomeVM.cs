using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Villa>? VillaList { get; set; }
        public DateOnly CheckInData { get; set; }
        public DateOnly? CheckOutData { get; set; }
        public int Nights { get; set; }
    }
}

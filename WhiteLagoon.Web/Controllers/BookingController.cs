using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult FinalizeBooking(int villaId, string checkIn, int nights)
        {
            DateOnly checkInDate=DateOnly.Parse(checkIn);
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _unitOfWork.User.Get(u=>u.Id == UserId);
            
            Booking booking = new()
            {
                UserId = UserId,
                VillaId = villaId,
                Villa = _unitOfWork.Villa.Get(u=>u.Id==villaId, includeProperties: "VillaAmenity"),
                Nights = nights,
                CheckInDate = checkInDate,
                CheckOutDate = checkInDate.AddDays(nights),
                Phone = user.PhoneNumber,
                Email=user.Email,
                Name = user.Name,   
            };
            
            booking.TotalCost = booking.Villa.Price * nights;

            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _unitOfWork.Villa.Get(u => u.Id == booking.VillaId);

            booking.TotalCost = villa.Price * booking.Nights;

            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            List<VillaNumber> villaNumbersList=_unitOfWork.VillaNumber.GetAll().ToList();
            List<Booking> bookedVillas= _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusCheckedIn ||
            u.Status == SD.StatusApproved).ToList();

            int roomsAvailable = SD.VillaRoomsAvailable_Count(villa.Id, villaNumbersList, booking.CheckInDate, 
                                                                booking.Nights, bookedVillas);

            if(roomsAvailable==0)
            {
                TempData["error"] = "Room has been sold out!";

                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkIn = booking.CheckInDate.ToString(),
                    nights = booking.Nights
                });
            }

            _unitOfWork.Booking.Add(booking);

            _unitOfWork.Save();

            var domain = Request.Scheme+"://"+Request.Host.Value;
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"/booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"/booking/FinalizeBooking?villaId={booking.VillaId}&checkIn={booking.CheckOutDate.ToString()}&"
                                   + $"nights={booking.Nights}",
            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(booking.TotalCost * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = villa.Name,
                            //Images = new List<string> { domain + villa.ImageUrl },
                        },
                    },
                    Quantity=1,
            });

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.Booking.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();


            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            
        }


        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)

        {
            
            Booking bookingFromDb = _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Villa");

            if (bookingFromDb.Status == SD.StatusPending)
            {
                var service = new SessionService();
                Session session = service.Get(bookingFromDb.StripeSessionId);

                if(session.PaymentStatus == "paid") 
                {
                    _unitOfWork.Booking.UpdateStaturs(bookingFromDb.Id, SD.StatusApproved,0);
                    _unitOfWork.Booking.UpdateStripePaymentID(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }

            return View(bookingId);
        }

        [Authorize]
        public IActionResult BookingDetails(int bookingId)
        {
            Booking objBookings = _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Villa");

            

            if (objBookings.VillaNumber==0 && objBookings.Status==SD.StatusApproved)
            {
                var availableVillaNumbers = AssignAvailabeVillaNumberByVillaId(objBookings.VillaId);

                objBookings.VillaNumbers=_unitOfWork.VillaNumber.GetAll(u=>u.VillaId==objBookings.VillaId &&
                                                                 availableVillaNumbers.Any(x=>x==u.Villa_Number)).ToList();
            }

            return View(objBookings);
        }


        [HttpPost]
        [Authorize(Roles =SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _unitOfWork.Booking.UpdateStaturs(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
            _unitOfWork.Save();

            TempData["success"] = "Booking CheckedIn Successfully!";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _unitOfWork.Booking.UpdateStaturs(booking.Id, SD.StatusCompleted, booking.VillaNumber);
            _unitOfWork.Save();

            TempData["success"] = "Booking CheckedOut Successfully!";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize]
        public IActionResult CancelBooking(Booking booking)
        {
            _unitOfWork.Booking.UpdateStaturs(booking.Id, SD.StatusCancelled, 0);
            _unitOfWork.Save();

            TempData["success"] = "Booking Cancelled Successfully!";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        #region API Calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;

            if(User.IsInRole(SD.Role_Admin))
            {
                objBookings=_unitOfWork.Booking.GetAll(includeProperties:"User,Villa");
            }
            else 
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var UserId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objBookings = _unitOfWork.Booking.GetAll(u=>u.UserId == UserId, includeProperties: "User,Villa");
            }
            if(!string.IsNullOrEmpty(status))
            {
                objBookings = objBookings.Where(u => u.Status.ToLower().Equals(status.ToLower()));
            }
            return Json (new { data = objBookings });
        }

        private List<int> AssignAvailabeVillaNumberByVillaId(int villaId)
        {
            List<int> availableVillaNumbers = new();

            List<int> villaNumbers = _unitOfWork.VillaNumber.GetAll(u => u.VillaId == villaId).Select(x => x.Villa_Number).ToList();

            List<int> bookedVillaNumbers = _unitOfWork.Booking.GetAll(u => u.VillaId == villaId &&
                                           (u.Status==SD.StatusApproved || u.Status == SD.StatusCheckedIn))
                                           .Select(x => x.VillaNumber).ToList();

            foreach (var villanumber in villaNumbers)
            {
                if (!bookedVillaNumbers.Contains(villanumber))
                {
                    availableVillaNumbers.Add(villanumber);
                }
            }

            return availableVillaNumbers;
        }
        #endregion
    }
}

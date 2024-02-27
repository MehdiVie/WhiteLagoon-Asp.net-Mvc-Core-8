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
                    _unitOfWork.Booking.UpdateStaturs(bookingFromDb.Id, SD.StatusApproved);
                    _unitOfWork.Booking.UpdateStripePaymentID(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }

            return View(bookingId);
        }

    }
}

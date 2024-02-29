using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckedIn = "CheckedIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static int VillaRoomsAvailable_Count(int villaId, List<VillaNumber> villaNumberList,
            DateOnly checkInDate, int nights, List<Booking> bookings)
        {
            int j;
            int finalAvailabelRoomsForAllNights = int.MaxValue;

            int roomsInVilla = villaNumberList.Where(u=>u.VillaId == villaId).Count();

            List<int> bookInDate = new ();

            for (int i = 0; i < nights; i++)
            {
                var villasBooked = bookings.Where(u => u.CheckInDate <= checkInDate.AddDays(i)
                                            && u.CheckOutDate > checkInDate.AddDays(i) && u.VillaId == villaId);

                foreach (var booking in villasBooked)
                {
                    if(!bookInDate.Contains(booking.Id))
                    {
                        bookInDate.Add(booking.Id);
                    }
                }



                int totalAvailableRooms = roomsInVilla - bookInDate.Count();
                if (totalAvailableRooms==0)
                {
                    return 0;
                }
                else
                {
                    if(finalAvailabelRoomsForAllNights > totalAvailableRooms)
                    {
                        finalAvailabelRoomsForAllNights = totalAvailableRooms;
                    }
                }

            }


            return finalAvailabelRoomsForAllNights;

        }
    }
}

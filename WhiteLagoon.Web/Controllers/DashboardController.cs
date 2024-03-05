using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        static int previousMonth = (DateTime.Now.Month == 1) ? 12 : (DateTime.Now.Month - 1);
        static int previousYear = (DateTime.Now.Month == 1) ? (DateTime.Now.Year - 1) : DateTime.Now.Year;

        readonly DateTime previousMonthStartDate = new(previousYear, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month , 1);

        //private readonly IUnitOfWork _unitOfWork;
        //public DashboardController(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}

        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DashboardController(IUnitOfWork unitOfWork,
                                RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()

        {
            

            var bookingData = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusCancelled || u.Status != SD.StatusPending);

            var countByCurrentMonth= bookingData.Count(u=> u.BookingDate >= currentMonthStartDate &&
                                                       u.BookingDate <= DateTime.Now );

            var countByPrevioustMonth = bookingData.Count(u => u.BookingDate >= previousMonthStartDate &&
                                                        u.BookingDate <= currentMonthStartDate);

            return Json(GetRadialChartDataModel(bookingData.Count(), countByCurrentMonth, countByPrevioustMonth));

        }
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            

            var totalUsers = _unitOfWork.User.GetAll();
            
            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate &&
                                                       u.CreatedAt <= DateTime.Now);

            var countByPrevioustMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate &&
                                                        u.CreatedAt <= currentMonthStartDate);


            return Json(GetRadialChartDataModel(totalUsers.Count(), countByCurrentMonth, countByPrevioustMonth));
            
        }

		public async Task<IActionResult> GetRevenuChartData()
		{

			var bookingData = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusCancelled || u.Status != SD.StatusPending);

            var totalRevenu = Convert.ToUInt32(bookingData.Sum(u => u.TotalCost));
            
			var countByCurrentMonth = bookingData.Where(u => u.BookingDate >= currentMonthStartDate &&
													   u.BookingDate <= DateTime.Now).Sum(u=>u.TotalCost);

			var countByPrevioustMonth = bookingData.Where(u => u.BookingDate >= previousMonthStartDate &&
														u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);


			return Json(GetRadialChartDataModel((int)totalRevenu, countByCurrentMonth, countByPrevioustMonth));

		}

        public async Task<IActionResult> GetBookingPieChartData()
        {

            var bookingData = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
            (u.Status != SD.StatusCancelled || u.Status != SD.StatusPending));

            var customerWithOneBooking= bookingData.GroupBy(u => u.UserId).Where(g => g.Count()==1).Select(x=>x.Key).ToList();

            var bookingsByNewCustomer = customerWithOneBooking.Count();
            var bookingsByReturningCustomer = bookingData.Count()- bookingsByNewCustomer;

            PieChartVM pieChartVM = new()
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
            };

            return Json(pieChartVM);

        }

        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            var bookingData =_unitOfWork.Booking.GetAll(u=>u.BookingDate>=DateTime.Now.AddDays(-30))
                                                .GroupBy(b=>b.BookingDate.Date)
                                                .Select(x=> new
                                                {
                                                    DateTime = x.Key,
                                                    NewBookingCount = x.Count()
                                                });

            var customerData = _unitOfWork.User.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30))
                                               .GroupBy(b => b.CreatedAt.Date)
                                               .Select(x => new
                                               {
                                                    DateTime = x.Key,
                                                    NewCustomerCount = x.Count()
                                               });

            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime, customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewCustomerCount = customer.Select(u => u.NewCustomerCount).FirstOrDefault()
                });

            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime,
                    NewBookingCount = booking.Select(u => u.NewBookingCount).FirstOrDefault(),
                    customer.NewCustomerCount
                    
                });

            var mergeData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newBookingData = mergeData.Select(x=>x.NewBookingCount).ToArray();
            var newCustomerData = mergeData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergeData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();

            List<ChartData> chartDataList = new()
            {
                new ChartData
                {
                    Name = "New Bookings",
                    Data=newBookingData
                },
                new ChartData
                {
                    Name = "New Members",
                    Data=newCustomerData
                }
            };

            LineChartVM lineChartVM = new()
            {
                Categories = categories,
                Series = chartDataList
            };


            return Json(lineChartVM);
        }

        private RadialBarChartVM GetRadialChartDataModel(int totalCount,double currentMonthCount,double previousMonthCount)
        {
            RadialBarChartVM radialBarChartVM = new();

            int increaseDecreaseRatio = 100;

            if (previousMonthCount != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32(((double)(currentMonthCount - previousMonthCount) / (double)previousMonthCount) * 100);
            }

            radialBarChartVM.TotalCount = totalCount;
            radialBarChartVM.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);
            radialBarChartVM.HasRatioIncreased = currentMonthCount > previousMonthCount;
            radialBarChartVM.Series = increaseDecreaseRatio;

            return radialBarChartVM;
        }
        

    }
}

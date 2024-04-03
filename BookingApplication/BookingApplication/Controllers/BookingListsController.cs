using BookingApplication.Data;
using BookingApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingApplication.Controllers
{
    public class BookingListsController : Controller
    {
        public readonly ApplicationDbContext _context;

        public BookingListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = _context.Users
                .Include(x => x.BookingList)
                .Include("BookingList.BookReservations")
                .Include("BookingList.BookReservations.Reservation")
                .Include("BookingList.BookReservations.Reservation.Apartment")
                .FirstOrDefault(x => x.Id == userId);

            var userBookingList = loggedInUser?.BookingList;
            var allBookReservations = userBookingList?.BookReservations?.ToList();

            var totalPrice = allBookReservations.Select(x => (x.Reservation.Apartment.Price_per_night * x.NumberOfNights)).Sum();

            BookingListDto dto = new BookingListDto
            {
                BookReservations = allBookReservations,
                TotalPrice = totalPrice
            };

            return View(dto);
            //return (IActionResult)dto;
        }


        public IActionResult DeleteFromShoppingCart(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = _context.Users
                .Include(x => x.BookingList)
                .Include("BookingList.BookReservations")
                .Include("BookingList.BookReservations.Reservation")
                .Include("BookingList.BookReservations.Reservation.Apartment")
                .FirstOrDefault(x => x.Id == userId);


            var userBookingList = loggedInUser?.BookingList;
            var reservation = userBookingList?.BookReservations.Where(x => x.ReservationId == id).FirstOrDefault();

            userBookingList.BookReservations.Remove(reservation);
            _context.BookingLists.Update(userBookingList);
            _context.SaveChanges();
            return RedirectToAction("Index", "BookingLists");

        }

        public IActionResult Order()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = _context.Users
                .Include(x => x.BookingList)
                .Include("BookingList.BookReservations")
                .Include("BookingList.BookReservations.Reservation")
                .Include("BookingList.BookReservations.Reservation.Apartment")
                .FirstOrDefault(x => x.Id == userId);

            var userBookingList = loggedInUser?.BookingList;

            Order order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                User = loggedInUser
            };

            List<ReservationInOrder> reservationInOrders = new List<ReservationInOrder>();

            var rez = userBookingList.BookReservations.Select(
                z => new ReservationInOrder
                {
                    Id = Guid.NewGuid(),
                    ReservationId = z.Reservation.Id,
                    Reservation = z.Reservation,
                    OrderId = order.Id,
                    Order = order,
                    NumberOfNights = z.NumberOfNights
                }).ToList();
            reservationInOrders.AddRange(rez);

            foreach (var reservation in reservationInOrders)
            {
                _context.ReservationInOrders.Add(reservation);
            }
            loggedInUser.BookingList.BookReservations.Clear();
            _context.Users.Update(loggedInUser);
            _context.SaveChanges();

            return RedirectToAction("Index", "BookingLists");

        }
    }
}

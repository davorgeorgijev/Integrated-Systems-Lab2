using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Models
{
    public class ReservationInOrder
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        public Guid ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public int NumberOfNights { get; set; }

    }
}

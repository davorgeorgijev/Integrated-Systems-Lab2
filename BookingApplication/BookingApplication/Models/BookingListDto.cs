namespace BookingApplication.Models
{
    public class BookingListDto
    {
        public List<BookReservation> BookReservations { get; set; }
        public int TotalPrice { get; set; }
    }
}

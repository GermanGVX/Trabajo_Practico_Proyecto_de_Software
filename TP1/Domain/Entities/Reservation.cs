using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum ReservationStatus
    {
        Pending,
        Paid,
        Expired
    }
    public class Reservation
    {
        public Guid Id {  get; set; }
        public int UserId { get; set; }
        public int SeatId { get; set; }
        public ReservationStatus status { get; set; } = ReservationStatus.Pending;
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public Seat seat { get; set; } = null!;

        public User user { get; set; } = null!;


    }
}

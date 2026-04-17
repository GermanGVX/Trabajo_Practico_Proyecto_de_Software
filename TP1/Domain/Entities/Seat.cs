using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public enum SeatStatus
    {
        Available,
        Reserved,
        Sold

    }
    public class Seat
    {
        public Guid id {  get; set; }
        public int SectorId { get; set; }
        public string Rowldentifier { get; set; }
        public int SeatNumber { get; set; }
        public SeatStatus status { get; set; } = SeatStatus.Available;
        public int Version { get; set; }

        public Sector sector { get; set; } = null!;
        public Reservation? Activereservation { get; set; }

    }
}

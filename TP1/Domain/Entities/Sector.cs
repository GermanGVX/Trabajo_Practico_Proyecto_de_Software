using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Sector
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }

        //El =null! es para asegurarle al compilador que EF Core va a llenar este dato cuando se cargue desde la BD
        public Event Events { get; set; } = null!;
        public List<Seat> Seats { get; set; }= new List<Seat>();

    }
}

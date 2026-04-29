using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Events.Commands
{
    public class CreateReservationCommand
    {
        public Guid SeatId { get; set; }
        public int? UserId { get; set; }
    }
}

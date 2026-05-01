using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Reservation.Commands
{
    public class ConfirmPaymentCommand
    {
        public Guid ReservationId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Events.Commands;
using Domain.Entities;
using Application.DTOs;


namespace Application.Interfaces
{
    public interface ICreateReservationCommandHandler
    {
        Task<ReservationResponseDto> CreateReservation(CreateReservationCommand command);
    }
}

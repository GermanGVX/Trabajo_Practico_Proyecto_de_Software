using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Events.Commands;

namespace Application.Interfaces
{
    public interface ICreateUserCommandHandler
    {
        Task<int> CreateUser(CreateUserCommand command);
    }
}

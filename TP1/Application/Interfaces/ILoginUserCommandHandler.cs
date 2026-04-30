using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Users.Commands;

namespace Application.Interfaces
{
    public interface ILoginUserCommandHandler
    {
        Task<int> LoginUser(LoginUserCommand command);
    }
}

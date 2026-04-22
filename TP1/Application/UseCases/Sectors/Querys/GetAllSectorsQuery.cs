using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.UseCases.Events.Querys
{
    public class GetAllSectorsQuery : IServicesGetAll
    {
        public object GetAll()
        {
            return new { name = "string" };
        }
    }
}

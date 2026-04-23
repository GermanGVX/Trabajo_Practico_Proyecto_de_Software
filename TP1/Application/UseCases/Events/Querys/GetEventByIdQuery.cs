using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Events.Querys
{
    public class GetEventByIdQuery
    {
        //public int id {  get; set; }
        public string Name { get; set; }
       
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
    }
}

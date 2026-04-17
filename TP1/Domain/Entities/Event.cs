using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Event
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public string Vanue { get; set; } 
        public string Status { get; set; } = "Active";

        public List<Sector> sectors { get; set; } = new List<Sector>();
    }
}

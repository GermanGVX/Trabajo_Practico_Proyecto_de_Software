using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Events.Querys
{
    public class GetPagedEventsQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public GetPagedEventsQuery()
        {
            Page = 1;
            PageSize = 10;
        }

        public GetPagedEventsQuery(int page, int pageSize)
        {
            Page = page > 0 ? page : 1;
            PageSize = pageSize > 0 ? pageSize : 10;
        }
    }
}

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

        // 1. ESTE ES EL ARREGLO: El constructor vacío que exige ASP.NET Core
        public GetPagedEventsQuery()
        {
            // Le damos valores por defecto por si desde el frontend no mandan parámetros
            Page = 1;
            PageSize = 10;
        }

        // 2. Tu constructor original (lo dejamos por si lo usás manualmente en otra parte de tu código)
        public GetPagedEventsQuery(int page, int pageSize)
        {
            Page = page > 0 ? page : 1;
            PageSize = pageSize > 0 ? pageSize : 10;
        }
    }
}

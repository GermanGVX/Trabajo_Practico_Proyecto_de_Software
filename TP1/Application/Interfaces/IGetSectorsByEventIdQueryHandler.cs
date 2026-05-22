using Application.UseCases.Sectors.Querys;

namespace Application.Interfaces
{
    public interface IGetSectorsByEventIdQueryHandler
    {
        Task<List<GetSectorsByEventIdQuery>> GetSectorByEventId(int eventId);
    }
}

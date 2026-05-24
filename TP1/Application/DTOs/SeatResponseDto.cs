using Domain.Enums;

namespace Application.DTOs
{
    public class SeatResponseDto
    {
        public Guid Id { get; set; }
        public int SeatNumber { get; set; }
        public string RowIdentifier { get; set; } = string.Empty;

        public SeatStatus Status { get; set; }
    }

    public class GroupedSeatsResponseDto
    {
        public string Row { get; set; } = string.Empty;
        public List<SeatResponseDto> Seats { get; set; } = new List<SeatResponseDto>();
    }
}

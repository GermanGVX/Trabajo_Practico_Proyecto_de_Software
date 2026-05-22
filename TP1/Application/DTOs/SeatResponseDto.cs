namespace Application.DTOs
{
    public class SeatResponseDto
    {
        public Guid Id { get; set; }
        public int SeatNumber { get; set; }
        public string RowIdentifier { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}

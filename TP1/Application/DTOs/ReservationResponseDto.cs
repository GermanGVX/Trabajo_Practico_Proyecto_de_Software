using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class ReservationResponseDto
    {
        public Guid Id { get; set; }
        public Guid SeatId { get; set; }
        public string Status { get; set; } = string.Empty;
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime ReservedAt { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToUniversalTime().ToString("o")); // "o" = ISO 8601 con 'Z'
    }
}

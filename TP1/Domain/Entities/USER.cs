namespace Domain.Entities
{
    public class USER
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public List<RESERVATION> reserva { get; set; } = new List<RESERVATION>();
    }
}

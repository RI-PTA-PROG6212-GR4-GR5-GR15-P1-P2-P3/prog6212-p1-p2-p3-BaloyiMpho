namespace RaceDay.API.Models
{
    public static class TicketPaymentStatus
    {
        public const string Pending = "Pending";
        public const string Unpaid = "Unpaid";
        public const string Paid = "Paid";
        public const string Cancelled = "Cancelled";
    }

    public static class TicketTypes
    {
        public const string Standard = "Standard";
        public const string Vip = "VIP";
        public const string Student = "Student";
    }

    public static class TicketPrices
    {
        public const decimal StandardPrice = 150.00m;
        public const decimal VipPrice = 1000.00m;
        public const decimal StudentPrice = 130.00m;

        public static void UpdateTicketPrice(Ticket ticket)
        {
            ticket.Price = GetPrice(ticket.TicketType);
        }

        public static decimal GetPrice(string ticketType)
        {
            return ticketType switch
            {
                TicketTypes.Standard => StandardPrice,
                TicketTypes.Vip => VipPrice,
                TicketTypes.Student => StudentPrice,
                _ => throw new ArgumentException($"Invalid ticket type '{ticketType}'.")
            };
        }
    }
}

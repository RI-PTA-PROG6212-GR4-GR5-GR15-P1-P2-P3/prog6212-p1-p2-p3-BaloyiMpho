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
            switch (ticket.TicketType)
            {
                case TicketTypes.Standard:
                    ticket.Price = TicketPrices.StandardPrice;
                    break;
                case TicketTypes.Vip:
                    ticket.Price = TicketPrices.VipPrice;
                    break;
                case TicketTypes.Student:
                    ticket.Price = TicketPrices.StudentPrice;
                    break;
                default:
                    throw new ArgumentException("Invalid ticket type");
            }
        }

        internal static decimal GetPrice(string ticketType)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Race_Day.Models.DTOs
{
    public class NewEnrol
    {
        public required Guid UserId { get; set; }
        [ForeignKey("Event")]
        public required Guid EventId { get; set; }
        [ForeignKey("Category")]
        public required Guid CategoryId { get; set; }

        public required DateOnly enrolment_date { get; set; }
        public required string enrolment_status { get; set; }
        public required string payment_status { get; set; }
        public required decimal amount_paid { get; set; }
        public required DateOnly paid_date { get; set; }
    }
}

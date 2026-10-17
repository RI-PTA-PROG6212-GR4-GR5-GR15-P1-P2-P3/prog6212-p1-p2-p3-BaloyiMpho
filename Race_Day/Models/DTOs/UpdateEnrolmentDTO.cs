namespace Race_Day.Models.DTOs
{
    public class UpdateEnrolmentDTO
    {
        public required DateOnly enrolment_date { get; set; }
        public required string enrolment_status { get; set; }
        public required string payment_status { get; set; }
        public decimal? amount_paid { get; set; }
        public required DateOnly paid_date { get; set; }
    }
}

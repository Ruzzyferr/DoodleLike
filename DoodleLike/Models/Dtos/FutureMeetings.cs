namespace DoodleLike.Models.Dtos
{
    public class FutureMeetings
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SelectedDate { get; set; }

        public DateTime Deadline { get; set; }
        public int ExpectedParticipantCount { get; set; }

        public int MeetingId { get; set; }
    }

}

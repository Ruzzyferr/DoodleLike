namespace DoodleLike.Models.Dtos
{
    public class OngoingMeetingsModel
    {

        public string MeetingName { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }

        public string UniqueId { get; set; }

    }
}

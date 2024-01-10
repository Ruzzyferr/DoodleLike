using System.ComponentModel.DataAnnotations.Schema;

namespace DoodleLike.Models.entity
{
    public class Meeting
    {
        public int MeetingID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CreatorID { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime Deadline { get; set; }

        public List<TimeSlot> TimeSlots { get; set; }

        public int MeetingDurationMinutes { get; set; }

        public String UniqueId { get; set; }
    }

}

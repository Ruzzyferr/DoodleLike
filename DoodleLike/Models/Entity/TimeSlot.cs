namespace DoodleLike.Models.entity
{
    public class TimeSlot
    {
        public int TimeSlotID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int Vote{ get; set; }
        public int MeetingID { get; set; }
    }

}

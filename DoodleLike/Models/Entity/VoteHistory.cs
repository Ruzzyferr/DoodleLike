using System.ComponentModel.DataAnnotations;

namespace DoodleLike.Models.Entity
{
    public class VoteHistory
    {

        [Key]
        public int Id { get; set; }

        public int MeetingId { get; set; }

        public int UserId { get; set; }

    }
}

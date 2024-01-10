using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoodleLike.Models.Dtos
{
    public class MeetingCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        
        [BindNever]
        public List<SelectListItem> AvailableTimeSlots { get; set; }

        public List<string> SelectedDates { get; set; } // Birden fazla seçilen tarihleri tutmak için dizi veya liste

        public int Deadline { get; set; }

        public int MeetingDurationMinutes { get; set; }

    }

}

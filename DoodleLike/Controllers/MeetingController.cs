using DoodleLike.Models;
using DoodleLike.Models.Dtos;
using DoodleLike.Models.entity;
using DoodleLike.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace DoodleLike.Controllers
{
    public class MeetingController : Controller
    {
        private readonly AppDbContext _context;

        public MeetingController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var meetingCreateDto = new MeetingCreateDto();

            var availableDates = Enumerable.Range(0, 30)
            .SelectMany(offset => Enumerable.Range(9, 8) // 9'dan başlayarak 8 saatlik aralığı temsil eder (09:00 - 16:00)
            .Select(hour => new DateTime(DateTime.Now.AddDays(offset ).Year, 
            DateTime.Now.AddDays(offset).Month, DateTime.Now.AddDays(offset + 1).Day, hour, 0, 0))) // Her gün 9:00 ile 16:00 arasındaki saatleri ekler
            .Select(date => new SelectListItem
            {
                Value = date.ToString("yyyy-MM-dd-HH"), // Saat bilgisi dahil edilmiş biçimlendirme
                Text = date.ToString("yyyy-MM-dd HH:mm") // Görüntülenen metin biçimi
            })
            .ToList();

            meetingCreateDto.AvailableTimeSlots = availableDates.ToList();
            return View(meetingCreateDto);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(MeetingCreateDto meetingCreateDto)
        {
            ModelState.Remove("AvailableTimeSlots");

            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kullanıcı e-postasından ID'sini bul
            var userId = _context.Users.FirstOrDefault(u => u.Email == userEmail).UserID;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return View(meetingCreateDto);
            }

            var meeting = new Meeting
            {
                Title = meetingCreateDto.Title,
                Description = meetingCreateDto.Description,
                UniqueId = Guid.NewGuid().ToString(),
                CreationDate = DateTime.Now,
                Deadline = DateTime.Now.AddDays(Convert.ToInt32(meetingCreateDto.Deadline)),
                MeetingDurationMinutes = meetingCreateDto.MeetingDurationMinutes,
                TimeSlots = new List<TimeSlot>(),
                CreatorID = userId
                
            };

            // Her bir seçilen tarih için TimeSlot oluşturuluyor
            foreach (var selectedDate in meetingCreateDto.SelectedDates)
            {
                var dateFormat = "yyyy-MM-dd-HH"; // Örnek tarih
                var startTime = DateTime.ParseExact(selectedDate, dateFormat, CultureInfo.InvariantCulture);

                var timeSlot = new TimeSlot
                {
                    StartTime = startTime, // Seçilen tarih başlangıç zamanı
                    EndTime = startTime.AddMinutes(meetingCreateDto.MeetingDurationMinutes), // Başlangıç zamanı + Toplantı süresi
                    Vote = 0 // Başlangıçta oy sayısı 0
                };

                meeting.TimeSlots.Add(timeSlot); 
            }

            _context.Meetings.Add(meeting);
            _context.SaveChanges();


            return RedirectToAction("MeetingPage", "Meeting", new { uniqueId = meeting.UniqueId });
        }


        public IActionResult MeetingPage(string uniqueId)
        {
            var meeting = _context.Meetings
                .Include(m => m.TimeSlots)
                .FirstOrDefault(m => m.UniqueId == uniqueId);

            if (meeting == null)
            {
                return NotFound(); 
            }

            if (meeting.Deadline < DateTime.Now)
            {
                return RedirectToAction("VotingEnded"); 
            }

            return View(meeting); 
        }

        public IActionResult MeetingPageWithoutLink(string uniqueId)
        {
            var meeting = _context.Meetings
                .Include(m => m.TimeSlots)
                .FirstOrDefault(m => m.UniqueId == uniqueId);

            if (meeting == null)
            {
                return NotFound();
            }

            if (meeting.Deadline < DateTime.Now)
            {
                return RedirectToAction("VotingEnded");
            }

            return View(meeting);
        }

        [Authorize]
        [HttpGet]
        public IActionResult VoteForMeeting(string uniqueId)
        {
            var meeting = _context.Meetings.FirstOrDefault(m => m.UniqueId == uniqueId);
            
            if (meeting == null)
            {
                return NotFound(); 
            }
            meeting.TimeSlots = _context.TimeSlots
                                .Where(ts => ts.MeetingID == meeting.MeetingID)
                                .ToList();

            return View(meeting);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Vote(List<int> selectedDates, string uniqueId)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kullanıcı e-postasından ID'sini bul
            var userId = _context.Users.FirstOrDefault(u => u.Email == userEmail).UserID;

            if (userId == null)
            {
                return BadRequest("Kullanıcı bulunamadı.");
            }

            var meeting = _context.Meetings.FirstOrDefault(m => m.UniqueId == uniqueId);

            if (meeting == null)
            {
                return NotFound();
            }

            // VoteHistory tablosunda ilgili kayıt var mı kontrol et
            var existingVote = _context.VoteHistory.FirstOrDefault(vh => vh.MeetingId == meeting.MeetingID && vh.UserId == userId);

            if (existingVote != null)
            {
                return BadRequest("Zaten oy kullandınız.");
            }

            var voteHistory = new VoteHistory
            {
                UserId = userId,
                MeetingId = meeting.MeetingID
            };

            _context.VoteHistory.Add(voteHistory);

            foreach (var dateId in selectedDates)
            {
                var timeSlot = _context.TimeSlots.FirstOrDefault(ts => ts.TimeSlotID == dateId);
                if (timeSlot != null)
                {
                    timeSlot.Vote++;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("MeetingPageWithoutLink", "Meeting", new { uniqueId = uniqueId });
        }




    }
}

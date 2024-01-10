using DoodleLike.Models;
using DoodleLike.Models.Dtos;
using DoodleLike.Models.entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace DoodleLike.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _context;


        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult HomePage()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult PastMeetings()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kullanıcı e-postasından ID'sini bul
            var userId = _context.Users.FirstOrDefault(u => u.Email == userEmail)?.UserID;

            if (userId == null)
            {
                return BadRequest("Kullanıcı bulunamadı.");
            }

            var pastMeetings = _context.VoteHistory
                .Where(vh => vh.UserId == userId)
                .Join(_context.Meetings,
                      vh => vh.MeetingId,
                      m => m.MeetingID,
                      (vh, m) => new
                      {
                          Meeting = m,
                          VoteHistory = vh
                      })
                .Where(joined => joined.Meeting.TimeSlots.OrderByDescending(ts => ts.Vote).FirstOrDefault().StartTime < DateTime.Now)
                .Select(joined => new PastMeetingsModel
                {
                    MeetingName = joined.Meeting.Title,
                    Description = joined.Meeting.Description,
                    MeetingDate = joined.Meeting.TimeSlots.OrderByDescending(ts => ts.Vote).FirstOrDefault().StartTime
                })
                .ToList();

            return View(pastMeetings);
        }


        [Authorize]
        public IActionResult FutureMeetings()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kullanıcı e-postasından ID'sini bul
            var userId = _context.Users.FirstOrDefault(u => u.Email == userEmail)?.UserID;

            if (userId == null)
            {
                return BadRequest("Kullanıcı bulunamadı.");
            }

            var futureMeetings = _context.VoteHistory
                .Where(vh => vh.UserId == userId)
                .Join(_context.Meetings,
                      vh => vh.MeetingId,
                      m => m.MeetingID,
                      (vh, m) => new
                      {
                          Meeting = m,
                          VoteHistory = vh
                      })
                .Where(joined => joined.Meeting.Deadline < DateTime.Now &&
                                 joined.Meeting.TimeSlots.OrderByDescending(ts => ts.Vote).FirstOrDefault().StartTime > DateTime.Now)
                .Select(joined => new FutureMeetings
                {
                    Title = joined.Meeting.Title,
                    Description = joined.Meeting.Description,
                    SelectedDate = joined.Meeting.TimeSlots.OrderByDescending(ts => ts.Vote).FirstOrDefault().StartTime,
                    ExpectedParticipantCount = joined.Meeting.TimeSlots.Sum(ts => ts.Vote)
                })
                .ToList();

            return View(futureMeetings);
        }




        [Authorize]
        public IActionResult OngoingMeetings()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kullanıcı e-postasından ID'sini bul
            var userId = _context.Users.FirstOrDefault(u => u.Email == userEmail).UserID;

            var ongoingMeetings = _context.VoteHistory
                .Where(vh => vh.UserId == userId)
                .Join(_context.Meetings,
                      vh => vh.MeetingId,
                      m => m.MeetingID,
                      (vh, m) => new OngoingMeetingsModel
                      {
                          MeetingName = m.Title,
                          Description = m.Description,
                          Deadline = m.Deadline,
                          UniqueId = m.UniqueId // UniqueId'yi burada ata
                      })
                .Where(m => m.Deadline > DateTime.Now)
                .ToList();

            return View(ongoingMeetings);
        }


    }
}

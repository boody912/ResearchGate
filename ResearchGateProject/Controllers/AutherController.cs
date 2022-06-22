using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchGateProject.Data;
using ResearchGateProject.Data.Services;
using ResearchGateProject.Data.ViewModels;
using ResearchGateProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResearchGateProject.Controllers
{
    public class AutherController : Controller
    {
        private readonly IApplicationUsersService _service;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public AutherController(UserManager<ApplicationUser> userManager, AppDbContext context, IApplicationUsersService service)
        {
            _userManager = userManager;
            _context = context;
            _service = service;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString)
        {
            var allAuthers = await _service.GetAllAsync(n => n.ApplicationUsers_Papers);

            if (!string.IsNullOrEmpty(searchString))
            {
                //var filteredResult = allMovies.Where(n => n.Name.ToLower().Contains(searchString.ToLower()) || n.Description.ToLower().Contains(searchString.ToLower())).ToList();

                var filteredResultNew = allAuthers.Where(n => string.Equals(n.FirstName, searchString, StringComparison.CurrentCultureIgnoreCase) || string.Equals(n.Uni, searchString, StringComparison.CurrentCultureIgnoreCase) || string.Equals(n.Email, searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

                return View("Index", filteredResultNew);
            }

            return View("Index", allAuthers);
        }



        //Get: Actors/Details/1
        [AllowAnonymous]
        public async Task<IActionResult> Details()
        {
            var Id =  _userManager.GetUserId(User);
            var user = await _context.Users.SingleAsync(User => User.Id == Id);



            if (user == null ) return View("Login");
            return View(user);
        }

        [AllowAnonymous]
        public async Task<IActionResult> AutherInfo(string id)
        {
            var autherInfo = await _service.GetAuthersByIdAsync(id);

            var papers = await _context.Papers.ToListAsync();

            //comments.Where(c => c.PaperId == PaperDetail.Id);
            Paper_comentsVM AuthersPapers = new Paper_comentsVM
            {
                User = autherInfo,
                papers = papers               
            };

            return View(AuthersPapers);
        }

        //Get: Actors/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var actorDetails = await _service.GetByIdAsync(id);
            if (actorDetails == null) return View("NotFound");
            return View(actorDetails);
            
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("ProfilePictureURL,FirstName,LastName,Uni,Dept,Email")] ApplicationUser user)
        {
            var User = await _context.Users.SingleAsync(User => User.Id == id);

            if (!ModelState.IsValid)
            {
                return View(user);
            }
            User.ProfilePictureURL = user.ProfilePictureURL;
            User.FirstName = user.FirstName;
            User.LastName = user.LastName;
            User.Uni = user.Uni;
            User.Dept = user.Dept;
            User.Email = user.Email;

            //await _service.UpdateAsync(id, user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details));
        }

        
    }
}

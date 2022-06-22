using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResearchGateProject.Data;
using ResearchGateProject.Data.Services;
using ResearchGateProject.Data.Static;
using ResearchGateProject.Data.ViewModels;
using ResearchGateProject.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ResearchGate.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class CommentsController : Controller
    {
        private static string paperid;
        private readonly ICommentService _service;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentsController(ICommentService service, AppDbContext context , UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _service = service;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allComments = await _service.GetAllAsync();
            return View(allComments);
        }


        //Get: Cinemas/Create
        [HttpGet]
        public IActionResult Create(string Id)
        {
            paperid = Id;
            //Comment comment = new Comment();
            //comment.PaperId = PaperId;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(string id, NewCommentVM comment)
        {
            if (!ModelState.IsValid)
            {

                return View(comment);
            }
            var comments = _context.Comments.ToList();
            var NewId = comments.Count + 1;
            var AutherId = _userManager.GetUserId(User);
            comment.Id = NewId.ToString();
            comment.PaperId = paperid;
            comment.ApplicationUserId = AutherId;
            await _service.AddNewCommentAsync(comment);
            return RedirectToAction("Details", "Paper", new { id = paperid });
        }

        //Get: Cinemas/Details/1
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            var commentDetails = await _service.GetByIdAsync(id);
            if (commentDetails == null) return View("NotFound");
            return View(commentDetails);
        }

        //Get: Cinemas/Edit/1
        public async Task<IActionResult> Edit(string id)
        {
            var commentDetails = await _service.GetByIdAsync(id);
            if (commentDetails == null) return View("NotFound");
            return View(commentDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Body")] Comment comment)
        {
            if (!ModelState.IsValid) return View(comment);
            await _service.UpdateAsync(id, comment);
            return RedirectToAction(nameof(Index));
        }

        //Get: Cinemas/Delete/1
        public async Task<IActionResult> Delete(string id)
        {
            var commentDetails = await _service.GetByIdAsync(id);
            if (commentDetails == null) return View("NotFound");
            return View(commentDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var commentDetails = await _service.GetByIdAsync(id);
            if (commentDetails == null) return View("NotFound");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}

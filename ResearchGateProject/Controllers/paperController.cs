using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResearchGateProject.Data;
using ResearchGateProject.Data.Services;
using ResearchGateProject.Data.ViewModels;
using ResearchGateProject.Models;
using ResearchGateProject.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace ResearchGateProject.Controllers
{
    //[Authorize(Roles = UserRoles.Admin)]
    public class paperController : Controller
    {

        //private readonly IPapersService _service;

        //public paperController(IPapersService service)
        //{
        //    _service = service;
        //}
        
        private static string paperid;
        private readonly IPapersService _service;
        private readonly ICommentService _CommentService;
        private readonly ILikeService _Likeservice;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public paperController(UserManager<ApplicationUser> userManager, AppDbContext context, IPapersService service , ICommentService CommentService , ILikeService Likeservice)
        {
            _userManager = userManager;
            _context = context;
            _service = service;
            _CommentService = CommentService;
            _Likeservice = Likeservice;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        //Get: Cinemas/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //await
            var PaperDropdownsData = await _service.GetNewPaperDropdownsValues();
         
            ViewBag.ApplicationUsers = new SelectList(PaperDropdownsData.ApplicationUsers, "Id", "FirstName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewPaperVM paper)
        {
            if (!ModelState.IsValid)
            {
                var PaperDropdownsData = await _service.GetNewPaperDropdownsValues();

                ViewBag.ApplicationUsers = new SelectList(PaperDropdownsData.ApplicationUsers, "Id", "FirstName");

                return View(paper);
            }
            var papers = _context.Papers.ToList();
            var NewId = papers.Count + 1;
            paper.Id = NewId.ToString();
            await _service.AddNewPaperAsync(paper);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            var PaperDetail = await _service.GetPaperByIdAsync(id);
            var comments = await _context.Comments.Where(c => c.PaperId == PaperDetail.Id).ToListAsync();
            var authers = await _context.ApplicationUsers.ToListAsync();
            var AutherId = _userManager.GetUserId(User);
            //comments.Where(c => c.PaperId == PaperDetail.Id);
            var TypeLike = await _context.Likes.Where(c => c.Type == "like" &  c.PaperId == PaperDetail.Id).ToListAsync();
            var TypeDisLike = await _context.Likes.Where(c => c.Type == "Dislike" & c.PaperId == PaperDetail.Id).ToListAsync();
          
            Paper_comentsVM PaperComment = new Paper_comentsVM 
            { 
                paper = PaperDetail,
                coments = comments,
                ApplicationUser = authers,
                NumbersOfLike = TypeLike.Count,
                NumbersOfDisLike = TypeDisLike.Count,
                ApplicationUserId=AutherId

            };
            return View(PaperComment);
        }

        [HttpGet]
        public IActionResult Like(string Id)
        {
            paperid = Id;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Like(string id, Like like)
        {
            
            var likes = _context.Likes.ToList();
            var AutherId = _userManager.GetUserId(User);
            var TypeLikeAuther = await _context.Likes.Where(c => c.Type == "like" & c.PaperId == id & c.ApplicationUserId == AutherId).CountAsync();
            if(TypeLikeAuther == 0)
            {
                if( likes.Count == 0) 
                {
                    var NewId1 = likes.Count + 1;
                    like.Id = NewId1.ToString();
                }
                else
                {
                       var NewId = likes.TakeLast(1);
                var lastId =int.Parse( NewId.Single().Id) + 1; 
                like.Id = lastId.ToString();
              

                }
                like.Type = "like";
                like.PaperId = id;
                like.ApplicationUserId = AutherId;
                await _Likeservice.AddNewLikeAsync(like);

                var InsertLikeAuther1 = await _context.Likes.Where(c => c.Type == "Dislike" & c.PaperId == id & c.ApplicationUserId == AutherId).CountAsync();
                if (InsertLikeAuther1 == 1)
                {
                    var InsertLikeAutherdis = await _context.Likes.SingleAsync(c => c.Type == "Dislike" & c.PaperId == id & c.ApplicationUserId == AutherId);
                    await _Likeservice.DeleteAsync(InsertLikeAutherdis.Id);
                }

            }
            else
            {
                var InsertLikeAuther = await _context.Likes.SingleAsync(c => c.Type == "like" & c.PaperId == id & c.ApplicationUserId == AutherId);
                await _Likeservice.DeleteAsync(InsertLikeAuther.Id);
            }
            
           
            return RedirectToAction("Details" , new { id = id });
        }

       
        [HttpGet]
        public IActionResult DisLike(string Id)
        {
            paperid = Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DisLike(string id, Like like)
        {           
            var likes = _context.Likes.ToList();
            var AutherId = _userManager.GetUserId(User);
            var TypeLikeAuther = await _context.Likes.Where(c => c.Type == "Dislike" & c.PaperId == id & c.ApplicationUserId == AutherId).CountAsync();
            if (TypeLikeAuther == 0)
            {
                if (likes.Count == 0)
                {
                    var NewId1 = likes.Count + 1;
                    like.Id = NewId1.ToString();
                }
                else
                {
                    var NewId = likes.TakeLast(1);
                    var lastId = int.Parse(NewId.Single().Id) + 1;
                    like.Id = lastId.ToString();


                }
                like.Type = "Dislike";
                like.PaperId = id;
                like.ApplicationUserId = AutherId;
                await _Likeservice.AddNewLikeAsync(like);

                var InsertLikeAuther1 = await _context.Likes.Where(c => c.Type == "like" & c.PaperId == id & c.ApplicationUserId == AutherId).CountAsync();
                if (InsertLikeAuther1 == 1)
                {
                    var InsertLikeAutherdis = await _context.Likes.SingleAsync(c => c.Type == "like" & c.PaperId == id & c.ApplicationUserId == AutherId);
                    await _Likeservice.DeleteAsync(InsertLikeAutherdis.Id);
                }

            }
            else
            {
                var InsertLikeAuther = await _context.Likes.SingleAsync(c => c.Type == "Dislike" & c.PaperId == id & c.ApplicationUserId == AutherId);
                await _Likeservice.DeleteAsync(InsertLikeAuther.Id);
            }          
            return RedirectToAction("Details", new { id = id });

        }



    }
}

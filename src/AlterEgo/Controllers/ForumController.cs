using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlterEgo.Data;
using AlterEgo.Models;
using AlterEgo.Models.ForumViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var categories =
                _context.Categories.Include(category => category.Forums)
                    .ThenInclude(forum => forum.Threads)
                    .ThenInclude(thread => thread.Posts)
                    .ToList();

            var model = new IndexViewModel
            {
                Categories = categories
            };

            return View(model);
        }

        //
        // GET: /forum/threads/4
        public async Task<IActionResult> Threads(int id)
        {
            var threads =
                await
                    _context.Threads.Include(t => t.Author)
                        .Include(t => t.Editor)
                        .Include(t => t.Posts)
                        .Where(thread => thread.ForumId == id)
                        .ToListAsync();

            var forum = await _context.Forums.Include(f => f.Category).SingleOrDefaultAsync(f => f.ForumId == id);
            var model = new ThreadsViewModel { Threads = threads, Forum = forum };

            return View(model);
        }

        //
        // GET: /forum/thread/4
        public async Task<IActionResult> Thread(int id)
        {
            var thread =
                await
                    _context.Threads.Include(t => t.Author)
                        .Include(t => t.Posts)
                        .ThenInclude(p => p.Author)
                        .SingleOrDefaultAsync(t => t.ThreadId == id);

            var model = new ThreadViewModel { Thread = thread };

            return View(model);
        }

        // 
        // GET: /forum/newthread
        public async Task<IActionResult> NewThread(int id)
        {
            var forum = await _context.Forums.Include(f => f.Category).SingleOrDefaultAsync(f => f.ForumId == id);
            var model = new NewThreadViewModel { Forum = forum };

            return View(model);
        }

        // 
        // GET: /forum/newthread
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewThread(NewThreadViewModel model)
        {
            var postDateTime = DateTime.Now;
            var author = await _userManager.GetUserAsync(HttpContext.User);
            var forum = await _context.Forums.Include(f => f.Category).SingleOrDefaultAsync(f => f.ForumId == model.ForumId);

            var thread = new Thread
            {
                Forum = forum,
                ForumId = forum.ForumId,
                Name = model.ThreadSubject,
                CreatedAt = postDateTime,
                AuthorUserId = author.Id,
                Author = author
            };

            var post = new Post
            {
                Thread = thread,
                Content = model.PostContent,
                PostedAt = postDateTime,
                AuthorUserId = author.Id,
                Author = author
            };

            _context.Threads.Add(thread);
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Admin()
        {

            return View();
        }
    }
}

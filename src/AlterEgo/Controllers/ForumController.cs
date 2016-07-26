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
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
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
                await
                    _context.Categories
                        .Include(category => category.Forums)
                            .ThenInclude(forum => forum.Threads)
                            .ThenInclude(thread => thread.Posts)
                        .Include(category => category.Forums)
                            .ThenInclude(forum => forum.LatestPost)
                            .ThenInclude(post => post.Thread)
                            .ThenInclude(post => post.Author)
                        .ToListAsync();

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
                        .Include(t => t.Forum).ThenInclude(f => f.Category)
                        .Include(t => t.Posts)
                        .ThenInclude(p => p.Author)
                        .SingleOrDefaultAsync(t => t.ThreadId == id);

            thread.Posts.ForEach(post =>
            {
                post.FormattedContent = CommonMark.CommonMarkConverter.Convert(post.Content);
            });

            var model = new ThreadViewModel { Thread = thread };

            thread.Views++;
            _context.Threads.Update(thread);
            await _context.SaveChangesAsync();

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
        // POST: /forum/newthread
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewThread(NewThreadViewModel model)
        {
            var postDateTime = DateTime.Now;
            var author = await _userManager.GetUserAsync(HttpContext.User);
            var forum =
                await _context.Forums.Include(f => f.Category).SingleOrDefaultAsync(f => f.ForumId == model.ForumId);

            var thread = new Thread
            {
                Forum = forum,
                ForumId = forum.ForumId,
                Name = model.ThreadSubject,
                CreatedAt = postDateTime,
                AuthorUserId = author.Id,
                Author = author,
                IsStickied = model.StickyThread,
                IsLocked = model.LockThread
            };

            var post = new Post
            {
                Thread = thread,
                Content = model.PostContent,
                PostedAt = postDateTime,
                AuthorUserId = author.Id,
                Author = author
            };

            thread.LatestPostTime = postDateTime;
            forum.LatestPost = post;

            _context.Threads.Add(thread);
            _context.Posts.Add(post);
            _context.Forums.Update(forum);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 
        // GET: /forum/newreply/threadid
        public async Task<IActionResult> NewReply(int id)
        {
            var thread =
                await
                    _context.Threads.Include(t => t.Author)
                        .Include(t => t.Forum).ThenInclude(f => f.Category)
                        .Include(t => t.Posts).ThenInclude(p => p.Author)
                        .SingleOrDefaultAsync(t => t.ThreadId == id);

            var model = new NewReplyViewModel { Thread = thread };

            return View(model);
        }

        //
        // POST: /forum/newreply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewReply(NewReplyViewModel model)
        {
            var postDateTime = DateTime.Now;
            var author = await _userManager.GetUserAsync(HttpContext.User);
            var thread = await _context.Threads.SingleOrDefaultAsync(t => t.ThreadId == model.ThreadId);
            var forum = await _context.Forums.SingleOrDefaultAsync(f => f.ForumId == thread.ForumId);

            var post = new Post
            {
                Thread = thread,
                Content = model.Content,
                Author = author,
                AuthorUserId = author.Id,
                PostedAt = postDateTime
            };

            thread.LatestPostTime = postDateTime;
            forum.LatestPost = post;

            _context.Posts.Add(post);
            _context.Threads.Update(thread);
            _context.Forums.Update(forum);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id = thread.ThreadId });
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using AlterEgo.Data;
using AlterEgo.Helpers;
using AlterEgo.Models;
using AlterEgo.Models.ForumViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Controllers
{
    [Authorize]
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
                        .Include(category => category.Forums)
                            .ThenInclude(forum => forum.LatestPost)
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
                            .ThenInclude(post => post.Author)
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



            thread.Posts.ForEach(post => post.FormattedContent = MarkdownHelper.Transform(post.Content));

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
                Name = model.Subject,
                CreatedAt = postDateTime,
                AuthorUserId = author.Id,
                Author = author,
                IsStickied = model.StickyThread,
                IsLocked = model.LockThread
            };

            var post = new Post
            {
                Thread = thread,
                Content = model.Content,
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

            return RedirectToAction(nameof(Thread), new { id = thread.ThreadId });
        }

        //
        // GET: /forum/editthread/threadid
        public async Task<IActionResult> EditThread(int id)
        {
            var thread = await _context.Threads.SingleAsync(t => t.ThreadId == id);
            var post = await _context.Posts.FirstAsync(p => (p.ThreadId == id && p.PostedAt == thread.CreatedAt));

            var model = new EditThreadViewModel
            {
                Thread = thread,
                ThreadId = thread.ThreadId,
                PostId = post.PostId,
                Content = post.Content,
                Subject = thread.Name
            };

            return View(model);
        }

        //
        // POST: /forum/editthread/threadid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditThread(EditThreadViewModel model)
        {
            if (ModelState.IsValid)
            {
                var author = await _userManager.GetUserAsync(HttpContext.User);
                var editTime = DateTime.Now;

                var thread = await _context.Threads.SingleAsync(t => t.ThreadId == model.ThreadId);
                var post = await _context.Posts.FirstAsync(p => p.PostId == model.PostId);

                thread.Name = model.Subject;
                thread.Editor = author;
                thread.ModifiedAt = editTime;

                post.Content = model.Content;
                post.Editor = author;
                post.EditedAt = editTime;

                _context.Threads.Update(thread);
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Thread), new { id = model.ThreadId });
        }

        //
        // GET: /forum/editpost/postid
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.Posts.Include(p => p.Thread).SingleAsync(p => p.PostId == id);
            var model = new EditPostViewModel
            {
                Post = post,
                Content = post.Content,
                PostId = post.PostId
            };

            return View(model);
        }

        //
        // POST: /forum/editpost/postid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(EditPostViewModel model)
        {
            Post post = null;

            if (ModelState.IsValid)
            {
                var author = await _userManager.GetUserAsync(HttpContext.User);
                var editTime = DateTime.Now;

                post = await _context.Posts.Include(p => p.Thread).SingleOrDefaultAsync(p => p.PostId == model.PostId);

                post.Content = model.Content;
                post.Editor = author;
                post.EditedAt = editTime;

                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Thread), new { id = post?.Thread.ThreadId });
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

        #region Administration

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewCategory(AdminToolboxViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.CategoryName,
                    ReadableBy = model.CategoryReadableBy
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewForum(AdminToolboxViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.SingleAsync(c => c.CategoryId == model.CategoryId);

                var forum = new Forum
                {
                    Name = model.ForumName,
                    Description = model.ForumDescription,
                    Category = category,
                    CategoryId = category.CategoryId,

                    ReadableBy = model.ForumReadableBy,
                    WritableBy = model.ForumWritableBy,

                    CanLockThreads = model.ForumCanLockThreads,
                    CanStickyThreads = model.ForumCanStickyThreads,
                    CanEditThreads = model.ForumCanEditThreads,
                    CanDeleteThreads = model.ForumCanDeleteThreads
                };

                _context.Forums.Add(forum);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}

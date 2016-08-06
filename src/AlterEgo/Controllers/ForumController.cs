using System;
using System.Collections.Generic;
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
using Sakura.AspNetCore;

namespace AlterEgo.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly int _postsPageSize = 25;
        private readonly int _threadsPageSize = 100;

        public ForumController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            // Update user activity
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(user.AccessToken))
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            await UpdateUserActivity(user);

            var categories =
                await
                    _context.Categories
                        .Include(category => category.Forums)
                            .ThenInclude(forum => forum.Threads)
                            .ThenInclude(thread => thread.Posts)
                        .Where(category => (user.Rank <= category.ReadableBy))
                        .ToListAsync();

            var latestPosts = new Dictionary<int, Post>();
            categories.ForEach(c => c.Forums.ForEach(f =>
            {
                var latestPost = _context.Posts
                    .Include(p => p.Thread).ThenInclude(t => t.Forum)
                    .Include(p => p.Author)
                    .Where(p => p.Thread.Forum.ForumId == f.ForumId && !p.IsDeleted && !p.Thread.IsDeleted)
                    .OrderBy(p => p.PostId)
                    .LastOrDefault();

                latestPosts.Add(f.ForumId, latestPost);
            }));

            var activeUsers = await _context.Users.Where(u => u.LastActivity > DateTime.UtcNow.Subtract(new TimeSpan(0, 15, 0))).ToListAsync();
            var totalThreads = _context.Threads.Count(t => !t.IsDeleted);
            var totalPosts = _context.Posts.Count(p => !p.IsDeleted) - totalThreads;
            var totalMembers = _context.Users.Count();

            var model = new IndexViewModel
            {
                Categories = categories,
                LatestPosts = latestPosts,
                ActiveUsers = activeUsers,
                TotalMembers = totalMembers,
                TotalPosts = totalPosts,
                TotalThreads = totalThreads
            };

            return View(model);
        }

        // 
        // GET: /forum/latestpost/{threadId}
        public async Task<IActionResult> LatestPost(int id)
        {
            var latestPostId = await GetLatestPostIdAsync(id);
            var pageNo = await GetPostPageNoAsync(latestPostId);

            return new RedirectResult(Url.Action(nameof(Thread), new { id, page = pageNo }) + $"#post{latestPostId}");
        }

        //
        // GET: /forum/searchresults/{searchterm}
        public async Task<IActionResult> SearchResults(string term, int? page)
        {
            var model = new SearchResultsViewModel
            {
                FoundThreads = new List<Thread>()
            };

            if (term.Length >= 3)
            {
                var foundThreads = await _context.Threads
                    .Include(thread => thread.Author)
                    .Include(thread => thread.Posts)
                    .Where(thread => thread.Name.ToLower().Contains(term.ToLower()) ||
                                     thread.Posts.Any(post => post.Content.ToLower().Contains(term) && !post.IsDeleted) &&
                                     !thread.IsDeleted)
                    .ToListAsync();

                model.FoundThreads = foundThreads;
                model.PagedFoundThreads = foundThreads.ToPagedList(100, page ?? 1);
            }

            ViewBag.SearchTerm = term;

            return View(model);
        }

        //
        // GET: /forum/threads/4
        public async Task<IActionResult> Threads(int id, int? page)
        {
            // Update user activity
            var user = await _userManager.GetUserAsync(User);
            await UpdateUserActivity(user);

            var threadActivities = _context.ThreadActivities.Where(t => t.ApplicationUserId == user.Id).ToList();
            ViewBag.ThreadActivities = threadActivities;

            var threads =
                await
                    _context.Threads.Include(t => t.Author)
                        .Include(t => t.Editor)
                        .Include(t => t.Posts)
                            .ThenInclude(post => post.Author)
                        .Where(thread => thread.ForumId == id && !thread.IsDeleted)
                        .OrderByDescending(t => t.IsStickied)
                        .ThenByDescending(t => t.Posts.Where(p => !p.IsDeleted).Max(x => x.PostId))
                        .ToListAsync();

            var forum = await _context.Forums.Include(f => f.Category).SingleOrDefaultAsync(f => f.ForumId == id);
            var model = new ThreadsViewModel
            {
                PagedThreads = threads.ToPagedList(_threadsPageSize, page ?? 1),
                Threads = threads,
                Forum = forum
            };

            return View(model);
        }

        //
        // GET: /forum/thread/4
        public async Task<IActionResult> Thread(int id, int? page)
        {
            // Update user activity
            var user = await _userManager.GetUserAsync(User);
            await UpdateUserActivity(user);
            await UpdateUserThreadActivity(user, id);

            var thread =
                await
                    _context.Threads.Include(t => t.Author)
                        .Include(t => t.Forum).ThenInclude(f => f.Category)
                        .Include(t => t.Posts)
                        .ThenInclude(p => p.Author)
                        .SingleOrDefaultAsync(t => t.ThreadId == id);

            var posts = await _context.Posts
                .Include(post => post.Author)
                .Include(post => post.Editor)
                .Where(post => post.ThreadId == id && !post.IsDeleted)
                .OrderBy(post => post.PostId)
                .ToListAsync();

            thread.Posts.ForEach(post => post.FormattedContent = MarkdownHelper.Transform(post.Content));

            var model = new ThreadViewModel
            {
                Thread = thread,
                Posts = posts,
                PagedPosts = posts.ToPagedList(_postsPageSize, page ?? 1)
            };

            ViewBag.Page = page ?? 1;

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
            var postDateTime = DateTime.UtcNow;
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
                Author = author,
                IsFirstPost = true
            };

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
                var editTime = DateTime.UtcNow;

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
                var editTime = DateTime.UtcNow;

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
            var postDateTime = DateTime.UtcNow;
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

            _context.Posts.Add(post);
            _context.Threads.Update(thread);
            _context.Forums.Update(forum);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(LatestPost), new { id = thread.ThreadId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserLatestReadPost(int threadId, int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var threadActivity = _context.ThreadActivities.SingleOrDefault(t => t.ThreadId == threadId && t.ApplicationUserId == user.Id);

            if (threadActivity == null)
            {
                threadActivity = new ThreadActivity
                {
                    ApplicationUser = user,
                    ApplicationUserId = user.Id,
                    LastRead = DateTime.UtcNow,
                    LastReadPostId = postId,
                    ThreadId = threadId
                };

                _context.Add(threadActivity);
            }
            else
            {
                if (threadActivity.LastReadPostId == 0)
                {
                    threadActivity.LastReadPostId = postId;
                }
                else
                {
                    if (threadActivity.LastReadPostId < postId)
                    {
                        threadActivity.LastReadPostId = postId;
                    }
                }

                _context.Update(threadActivity);
            }
          
            await _context.SaveChangesAsync();

            return Ok();
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
                    CanStartThreads = model.ForumCanStartThreads,
                    CanReplyToThreads = model.ForumCanReplyToThreads,

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

        public async Task<IActionResult> LockThread(int id)
        {
            var thread = await _context.Threads.SingleOrDefaultAsync(t => t.ThreadId == id);
            thread.IsLocked = true;
            _context.Threads.Update(thread);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id });
        }

        public async Task<IActionResult> UnlockThread(int id)
        {
            var thread = await _context.Threads.SingleOrDefaultAsync(t => t.ThreadId == id);
            thread.IsLocked = false;
            _context.Threads.Update(thread);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id });
        }

        public async Task<IActionResult> DeleteThread(int id)
        {
            var thread = await _context.Threads.SingleOrDefaultAsync(t => t.ThreadId == id);
            thread.IsDeleted = true;

            _context.Threads.Update(thread);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Threads), new { id = thread.ForumId });
        }

        public async Task<IActionResult> UndeleteThread(int id)
        {
            var thread = await _context.Threads.SingleOrDefaultAsync(t => t.ThreadId == id);
            thread.IsDeleted = false;
            _context.Threads.Update(thread);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Threads), new { id = thread.ForumId });
        }

        public async Task<IActionResult> StickyThread(int id)
        {
            var thread = await _context.Threads.SingleOrDefaultAsync(t => t.ThreadId == id);
            thread.IsStickied = true;
            _context.Threads.Update(thread);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id });
        }

        public async Task<IActionResult> UnstickyThread(int id)
        {
            var thread = await _context.Threads.SingleOrDefaultAsync(t => t.ThreadId == id);
            thread.IsStickied = false;

            _context.Threads.Update(thread);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id });
        }

        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.Include(p => p.Thread).ThenInclude(t => t.Forum)
                .SingleOrDefaultAsync(p => p.PostId == id);
            post.IsDeleted = true;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id = post.ThreadId });
        }

        #endregion

        #region Private Functions

        private async Task<int> GetLatestPostIdAsync(int threadId)
        {
            var user = await _userManager.GetUserAsync(User);
            var threadActivity = _context.ThreadActivities.SingleOrDefault(t => t.ThreadId == threadId && t.ApplicationUserId == user.Id);

            if (threadActivity == null || threadActivity.LastReadPostId == 0)
            {
                var latestPost = await _context.Posts
                    .Include(post => post.Thread)
                    .Where(post => post.ThreadId == threadId && !post.Thread.IsDeleted && !post.IsDeleted)
                    .OrderBy(post => post.PostedAt)
                    .LastAsync();

                return latestPost.PostId;
            }

            return threadActivity.LastReadPostId;
        }

        private async Task<int> GetPostPageNoAsync(int postId)
        {
            var post = await _context.Posts.SingleOrDefaultAsync(p => p.PostId == postId);
            var posts = await _context.Posts.Where(p => p.ThreadId == post.ThreadId && !post.IsDeleted).ToListAsync();

            var postIndex = posts.IndexOf(post);

            return ((postIndex / _postsPageSize) + 1);
        }

        private async Task UpdateUserActivity(ApplicationUser user)
        {
            user.LastActivity = DateTime.UtcNow;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateUserThreadActivity(ApplicationUser user, int threadId)
        {
            var threadActivity = _context.ThreadActivities.SingleOrDefault(t => t.ApplicationUserId == user.Id && t.ThreadId == threadId);

            if (threadActivity == null)
            {
                threadActivity = new ThreadActivity
                {
                    ApplicationUserId = user.Id,
                    ApplicationUser = user,
                    LastRead = DateTime.UtcNow,
                    LastReadPostId = _context.Posts.Single(p => p.IsFirstPost && p.ThreadId == threadId).PostId,
                    ThreadId = threadId
                };

                _context.Add(threadActivity);
            }
            else
            {
                threadActivity.LastRead = DateTime.UtcNow;
                _context.Update(threadActivity);
            }

            await _context.SaveChangesAsync();
        }

        #endregion
    }
}

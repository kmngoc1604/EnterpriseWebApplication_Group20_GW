using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseWebApplication.Data;
using EnterpriseWebApplication.Models.Entities;
using System.Security.Claims;

namespace EnterpriseWebApplication.Controllers.User
{
    public class SubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Submissions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Submissions.ToListAsync());
        }

        // GET: Submissions/Details/5
        public async Task<IActionResult> Details(int? submissionid)
        {
            if (submissionid == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(m => m.Id == submissionid);
            if (submission == null)
            {
                return NotFound();
            }

            ViewData["Ideas"] = await _context.Ideas.Include(i => i.Reactions).Where(i => i.SubmissionId == submissionid).ToListAsync();

            return View(submission);
        }
        // GET: Idea/Create
        public IActionResult AddIdea(int submissionid)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SubmissionId"] = submissionid;
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIdea(NIdea nIdea, bool isAcceptTerms)
        {
            //if (ModelState.IsValid)
            if (isAcceptTerms)
            {
                var submission = await _context.Submissions.FindAsync(nIdea.SubmissionId);

                if (submission.Deadline_1 >= DateTime.Now)
                {
                    _context.Add(nIdea);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { submissionid = nIdea.SubmissionId });
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SubmissionId"] = nIdea.SubmissionId;
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(nIdea);
        }

        public async Task<IActionResult> IdeaDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nIdea = await _context.Ideas.Include(n => n.Category).Include(n => n.Submission).FirstOrDefaultAsync(m => m.Id == id);

            if (nIdea == null)
            {
                return NotFound();
            }

            ViewData["Comments"] = await _context.Comments.Where(c => c.IdeaId == id).ToListAsync();

            nIdea.View += 1;

            _context.Update(nIdea);
            await _context.SaveChangesAsync();

            return View(nIdea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(string Content, int IdeaId)
        {
            var comment = new Comment();
            comment.Content = Content;
            comment.IdeaId = IdeaId;
            comment.CreatedDate = DateTime.Now;
            comment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IdeaDetails), new { id = IdeaId });
        }

        public async Task<IActionResult> Like(int ideaid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reaction = await _context.Reactions.Where(r => r.IdeaId == ideaid && r.UserId == userId).FirstOrDefaultAsync();

            if (reaction == null)
            {
                reaction = new Reaction();
                reaction.Type = 1;
                reaction.UserId = userId;
                reaction.IdeaId = ideaid;

                _context.Add(reaction);
                await _context.SaveChangesAsync();
            }

            else
            {
                if (reaction.Type == 1)
                {
                    reaction.Type = 0;
                }
                else
                {
                    reaction.Type = 1;
                }

                _context.Update(reaction);
                await _context.SaveChangesAsync();
            }

            var idea = await _context.Ideas.FindAsync(ideaid);

            return RedirectToAction(nameof(Details), new { submissionid = idea.SubmissionId });
        }

        public async Task<IActionResult> Dislike(int ideaid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reaction = await _context.Reactions.Where(r => r.IdeaId == ideaid && r.UserId == userId).FirstOrDefaultAsync();

            if (reaction == null)
            {
                reaction = new Reaction();
                reaction.Type = 2;
                reaction.UserId = userId;
                reaction.IdeaId = ideaid;

                _context.Add(reaction);
                await _context.SaveChangesAsync();
            }

            else
            {
                if (reaction.Type == 2)
                {
                    reaction.Type = 0;
                }
                else
                {
                    reaction.Type = 2;
                }

                _context.Update(reaction);
                await _context.SaveChangesAsync();
            }

            var idea = await _context.Ideas.FindAsync(ideaid);

            return RedirectToAction(nameof(Details), new { submissionid = idea.SubmissionId });
        }
    }
}

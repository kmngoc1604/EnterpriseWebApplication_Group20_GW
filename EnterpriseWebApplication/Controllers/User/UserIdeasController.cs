using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseWebApplication.Data;
using EnterpriseWebApplication.Models.Entities;

namespace EnterpriseWebApplication.Controllers.User
{
    public class UserIdeasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserIdeasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserIdeas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ideas.Include(n => n.Category).Include(n => n.Submission);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserIdeas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nIdea = await _context.Ideas
                .Include(n => n.Category)
                .Include(n => n.Submission)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nIdea == null)
            {
                return NotFound();
            }

            return View(nIdea);
        }

        // GET: UserIdeas/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id");
            return View();
        }

        // POST: UserIdeas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Brief,Content,FilePath,View,SubmissionId,CategoryId,UserId,Id,CreatedDate,UpdatedDate")] NIdea nIdea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nIdea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", nIdea.CategoryId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id", nIdea.SubmissionId);
            return View(nIdea);
        }

        // GET: UserIdeas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nIdea = await _context.Ideas.FindAsync(id);
            if (nIdea == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", nIdea.CategoryId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id", nIdea.SubmissionId);
            return View(nIdea);
        }

        // POST: UserIdeas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Brief,Content,FilePath,View,SubmissionId,CategoryId,UserId,Id,CreatedDate,UpdatedDate")] NIdea nIdea)
        {
            if (id != nIdea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nIdea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NIdeaExists(nIdea.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", nIdea.CategoryId);
            ViewData["SubmissionId"] = new SelectList(_context.Submissions, "Id", "Id", nIdea.SubmissionId);
            return View(nIdea);
        }

        // GET: UserIdeas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nIdea = await _context.Ideas
                .Include(n => n.Category)
                .Include(n => n.Submission)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nIdea == null)
            {
                return NotFound();
            }

            return View(nIdea);
        }

        // POST: UserIdeas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nIdea = await _context.Ideas.FindAsync(id);
            _context.Ideas.Remove(nIdea);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NIdeaExists(int id)
        {
            return _context.Ideas.Any(e => e.Id == id);
        }
    }
}

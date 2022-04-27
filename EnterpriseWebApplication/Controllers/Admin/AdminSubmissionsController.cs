using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseWebApplication.Data;
using EnterpriseWebApplication.Models.Entities;

namespace EnterpriseWebApplication.Controllers.Admin
{
    public class AdminSubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSubmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminSubmissions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Submissions.ToListAsync());
        }

        // GET: AdminSubmissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // GET: AdminSubmissions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminSubmissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Deadline_1,Deadline_2,Id,CreatedDate,UpdatedDate")] Submission submission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(submission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(submission);
        }

        // GET: AdminSubmissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound();
            }
            return View(submission);
        }

        // POST: AdminSubmissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Deadline_1,Deadline_2,Id,CreatedDate,UpdatedDate")] Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
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
            return View(submission);
        }

        // GET: AdminSubmissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // POST: AdminSubmissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submission = await _context.Submissions.FindAsync(id);
            _context.Submissions.Remove(submission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubmissionExists(int id)
        {
            return _context.Submissions.Any(e => e.Id == id);
        }
    }
}

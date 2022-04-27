using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnterpriseWebApplication.Data;
using EnterpriseWebApplication.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EnterpriseWebApplication.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            var items = _context.Categories
                .Include(c => c.CategoryChildren)  
                .AsEnumerable()
                .Where(c => c.ParentCategory == null)
                .ToList();


            return View(items);

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public async Task<IActionResult> Create()
        {
            // ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Slug");
            var listcategory = await _context.Categories.ToListAsync();
            listcategory.Insert(0, new Category()
            {
                Name = "No parents cae",
                Id = -1
            });
            ViewData["ParentCategoryId"] = new SelectList(await GetItemsSelectCategorie(), "Id", "Name", -1);
            return View();
        }

        async Task<IEnumerable<Category>> GetItemsSelectCategorie()
        {

            var items = await _context.Categories
                                .Include(c => c.CategoryChildren)
                                .Where(c => c.ParentCategory == null)
                                .ToListAsync();



            List<Category> resultitems = new List<Category>() {
                new Category() {
                    Id = -1,
                    Name = "No parents category"
                }
            };
            Action<List<Category>, int> _ChangeNameCategory = null;
            Action<List<Category>, int> ChangeNameCategory = (items, level) => {
                string prefix = String.Concat(Enumerable.Repeat("—", level));
                foreach (var item in items)
                {
                    item.Name = prefix + " " + item.Name;
                    resultitems.Add(item);
                    if ((item.CategoryChildren != null) && (item.CategoryChildren.Count > 0))
                    {
                        _ChangeNameCategory(item.CategoryChildren.ToList(), level + 1);
                    }

                }

            };

            _ChangeNameCategory = ChangeNameCategory;
            ChangeNameCategory(items, 0);

            return resultitems;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ParentCategoryId,Content,Slug,Id,CreatedDate,UpdatedDate")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId.Value == -1)
                    category.ParentCategoryId = null;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "Slug", category.ParentCategoryId);
            var listcategory = await _context.Categories.ToListAsync();
            listcategory.Insert(0, new Category()
            {
                Name = "No parents category",
                Id = -1
            });
            ViewData["ParentCategoryId"] = new SelectList(await GetItemsSelectCategorie(), "Id", "Name", category.ParentCategoryId);
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewData["ParentCategoryId"] = new SelectList(await GetItemsSelectCategorie(), "Id", "Name", category.ParentCategoryId);

            return View(category);
        }

        // POST: Admin/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,ParentCategoryId,Content,Slug,Id,CreatedDate,UpdatedDate")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (category.ParentCategoryId == -1)
                    {
                        category.ParentCategoryId = null;
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            var listcategory = await _context.Categories.ToListAsync();
            listcategory.Insert(0, new Category()
            {
                Name = "No parents category",
                Id = -1
            });
            ViewData["ParentCategoryId"] = new SelectList(listcategory, "Id", "Name", category.ParentCategoryId);
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

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
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Net.Mime;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;
using MailKit.Net.Smtp;

namespace EnterpriseWebApplication.Controllers.User
{
    public class SubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly IEmailSender _emailSender;

        public SubmissionsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            //_emailSender = emailSender;
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
        public async Task<IActionResult> AddIdea(NIdea nIdea, IFormFile file, bool isAcceptTerms)
        {
            //if (ModelState.IsValid)
            if (isAcceptTerms)
            {
                var submission = await _context.Submissions.FindAsync(nIdea.SubmissionId);

                if (submission.Deadline_1 >= DateTime.Now)
                {
                    
                    _context.Add(nIdea);
                    await _context.SaveChangesAsync();

                    nIdea.FilePath = UploadFile(file, submission.Id, nIdea.Id);

                    _context.Update(nIdea);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { submissionid = nIdea.SubmissionId });
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SubmissionId"] = nIdea.SubmissionId;
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(nIdea);
        }

        public string UploadFile(IFormFile file, int submissionId, int ideaId)
        {

            var path = "";
            if (file.Length > 0)
            {
                path = Path.Combine("file", "submission_" + submissionId, "idea_" + ideaId);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);             

                path = Path.Combine(path, file.FileName);
                using var stream = new FileStream(path, FileMode.Create);
                file.CopyTo(stream);
            }

            return path;
        }

        public IActionResult Download(int submissionId)
        {
            ExportZIP(submissionId);
            //await ExportExcel(submissionId);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ExportZIP(int submissionId)
        {
            var path = Path.Combine("file", "submission_" + submissionId);

            if (Directory.Exists(path))
            {
                var zipPath = Path.Combine("file", "submission_" + submissionId + ".zip");

                ZipFile.CreateFromDirectory(path, zipPath);
                
                byte[] fileBytes = System.IO.File.ReadAllBytes(zipPath);

                System.IO.File.Delete(zipPath);
                return File(fileBytes, MediaTypeNames.Application.Zip, Path.GetFileName(zipPath));
            }

            return NoContent();
        }

        public async Task<IActionResult> ExportExcel(int submissionId)
        {
            var path = Path.Combine("file", $"submission_{submissionId}.xlsx");
            FileInfo file = new FileInfo(path);

            //var memory = new MemoryStream();

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var ideas = await _context.Ideas.Include(i => i.Reactions).Where(r => r.SubmissionId == submissionId).ToListAsync();

                int rowNum = 0;
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Idea List");

                IRow row = excelSheet.CreateRow(rowNum++);
                row.CreateCell(0).SetCellValue("No. ");
                row.CreateCell(1).SetCellValue("Title");
                row.CreateCell(2).SetCellValue("Brief");
                row.CreateCell(3).SetCellValue("Content");
                row.CreateCell(4).SetCellValue("Views");
                row.CreateCell(5).SetCellValue("Likes");
                row.CreateCell(6).SetCellValue("Dislikes");

                foreach (var idea in ideas)
                {
                    row = excelSheet.CreateRow(rowNum);

                    row.CreateCell(0).SetCellValue(rowNum);
                    row.CreateCell(1).SetCellValue(idea.Title);
                    row.CreateCell(2).SetCellValue(idea.Brief);
                    row.CreateCell(3).SetCellValue(idea.Content);
                    row.CreateCell(4).SetCellValue(idea.View);
                    row.CreateCell(5).SetCellValue(idea.Reactions.Where(i => i.Type == 1).Count().ToString());
                    row.CreateCell(6).SetCellValue(idea.Reactions.Where(i => i.Type == 2).Count().ToString());

                    ++rowNum;
                }

                workbook.Write(fs);
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            System.IO.File.Delete(path);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));

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
            var idea = await _context.Ideas.FindAsync(IdeaId);
            var submission = await _context.Submissions.FindAsync(idea.SubmissionId);
            
            if(submission.Deadline_2 >= DateTime.Now)
            {
                var comment = new Comment();
                comment.Content = Content;
                comment.IdeaId = IdeaId;
                comment.CreatedDate = DateTime.Now;
                comment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(comment);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(comment.UserId);

                string subject = $"Thank you for your comment'{idea.Title}'";
                string content = $"Hello {user.UserName},\n\n" + $"Thank you for your comment on '{idea.Title}' in '{submission.Name}'.\n\n" + $"Best Regards.";
                SendMail(idea.UserId, content, subject);
            }

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

        [BindProperty]
        public SubmissionsController Input { get; set; }
        public class InputModel
        {
            public string Email { get; set; }
        }

        public async void SendMail(int userId, string content, string subject)
        {
            var user = await _context.Users.FindAsync(userId);
            MailboxAddress from = new MailboxAddress("Idea Management System", "kminhngoc1604@gmail.com");
            MailboxAddress to = new MailboxAddress(user.UserName, user.Email);

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = content;

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(from);
            mimeMessage.To.Add(to);
            mimeMessage.Subject = subject;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, true);
            client.Authenticate("kminhngoc1604@gmail.com", "Ngoc16041998JBK");
            client.Send(mimeMessage);
            client.Disconnect(true);
            client.Dispose();

        }
    }
}

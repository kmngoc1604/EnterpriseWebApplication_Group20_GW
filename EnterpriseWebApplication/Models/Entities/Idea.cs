using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseWebApplication.Models.Entities
{
    public class NIdea : BaseEntity
    {
        public string Title { get; set; }
        public string Brief { get; set; }
        public string Content { get; set; }
        public string FilePath { get; set; }
        public int View { get; set; }

        public int SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int UserId { get; set; }
        public virtual AppUser User { get; set; }

        public virtual ICollection<Reaction> Reactions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}

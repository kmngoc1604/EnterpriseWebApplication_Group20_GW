using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseWebApplication.Models.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }

        public int IdeaId { get; set; }
        public virtual NIdea Idea { get; set; }
    }
}

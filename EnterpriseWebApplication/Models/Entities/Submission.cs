using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseWebApplication.Models.Entities
{
    public class Submission : BaseEntity
    {
        public string Name { get; set; }
        public DateTime Deadline_1 { get; set; }
        public DateTime Deadline_2 { get; set; }

        public virtual ICollection<NIdea> Ideas { get; set; }
    }
}

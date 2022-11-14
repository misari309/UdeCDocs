using System;
using System.Collections.Generic;

namespace UdeCDocsMVC.Models
{
    public partial class TypeVote
    {
        public TypeVote()
        {
            Votes = new HashSet<Vote>();
        }

        public int IdtypeVote { get; set; }
        public string? Type { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}

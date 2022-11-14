using System;
using System.Collections.Generic;

namespace UdeCDocsMVC.Models
{
    public partial class Vote
    {
        public int Idvote { get; set; }
        public int Value { get; set; }
        public int Iduser { get; set; }
        public int Iddocument { get; set; }
        public int IdtypeVote { get; set; }

        public virtual Document IddocumentNavigation { get; set; } = null!;
        public virtual TypeVote IdtypeVoteNavigation { get; set; } = null!;
        public virtual User IduserNavigation { get; set; } = null!;
    }
}

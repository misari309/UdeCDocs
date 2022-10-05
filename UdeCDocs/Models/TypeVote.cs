using System;
using System.Collections.Generic;

namespace UdeCDocs.Models
{
    public partial class TypeVote
    {
        public int IdtypeVote { get; set; }
        public bool Type { get; set; }

        public virtual Vote? Vote { get; set; }
    }
}

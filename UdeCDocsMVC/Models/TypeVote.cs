using System;
using System.Collections.Generic;

namespace UdeCDocsMVC.Models
{
    public partial class TypeVote
    {
        public int IdtypeVote { get; set; }
        public string? Type { get; set; }

        public virtual Vote? Vote { get; set; }
    }
}

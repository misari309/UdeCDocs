using System;
using System.Collections.Generic;

namespace UdeCDocs.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
        }

        public int Iduser { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Institution { get; set; }
        public string? City { get; set; }
        public string Password { get; set; } = null!;
        public int Idrol { get; set; }
        public int? Idfaculty { get; set; }

        public virtual Faculty? IdfacultyNavigation { get; set; }
        public virtual Rol IdrolNavigation { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
    }
}

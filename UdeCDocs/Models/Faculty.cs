using System;
using System.Collections.Generic;

namespace UdeCDocs.Models
{
    public partial class Faculty
    {
        public Faculty()
        {
            Fields = new HashSet<Field>();
            Users = new HashSet<User>();
        }

        public int Idfaculty { get; set; }
        public string Faculty1 { get; set; } = null!;

        public virtual ICollection<Field> Fields { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}

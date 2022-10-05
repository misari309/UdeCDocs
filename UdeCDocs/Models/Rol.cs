using System;
using System.Collections.Generic;

namespace UdeCDocs.Models
{
    public partial class Rol
    {
        public Rol()
        {
            Users = new HashSet<User>();
        }

        public int Idrol { get; set; }
        public string Role { get; set; } = null!;

        public virtual ICollection<User> Users { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace UdeCDocsMVC.Models
{
    public partial class Document
    {
        public Document()
        {
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
        }

        public int Iddocument { get; set; }
        public string Name { get; set; } = null!;
        public string Abstract { get; set; } = null!;
        public string Keywords { get; set; } = null!;
        public DateTime PublicationDate { get; set; }
        public string Authors { get; set; } = null!;
        public string Direction { get; set; } = null!;
        public int Idfield { get; set; }
        public int Iduser { get; set; }

        public virtual Field IdfieldNavigation { get; set; } = null!;
        public virtual User IduserNavigation { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
    }
}

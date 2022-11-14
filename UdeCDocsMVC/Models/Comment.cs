using System;
using System.Collections.Generic;

namespace UdeCDocsMVC.Models
{
    public partial class Comment
    {
        public int Idcomment { get; set; }
        public string Body { get; set; } = null!;
        public DateTime Date { get; set; }

        public string UserW { get; set; } = null!;
        public int Iduser { get; set; }
        public int Iddocument { get; set; }

        public virtual Document IddocumentNavigation { get; set; } = null!;
        public virtual User IduserNavigation { get; set; } = null!;
    }
}

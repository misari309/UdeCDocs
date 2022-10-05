using System;
using System.Collections.Generic;

namespace UdeCDocs.Models
{
    public partial class Field
    {
        public Field()
        {
            Documents = new HashSet<Document>();
        }

        public int Idfield { get; set; }
        public string Field1 { get; set; } = null!;
        public int Idfaculty { get; set; }

        public virtual Faculty IdfacultyNavigation { get; set; } = null!;
        public virtual ICollection<Document> Documents { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace UdeCDocsMVC.Models.SysModels
{
    public partial class CDocument
    {
        public string Name { get; set; } = null!;
        public string Abstract { get; set; } = null!;
        public string Keywords { get; set; } = null!;
        public DateTime PublicationDate { get; set; }
        public string Authors { get; set; } = null!;
        public IFormFile Direction { get; set; } = null!;
        public int Idfield { get; set; }
        public int Iduser { get; set; }

    }
}

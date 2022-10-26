using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace UdeCDocsMVC.Models
{
    public partial class CUser
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? City { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}

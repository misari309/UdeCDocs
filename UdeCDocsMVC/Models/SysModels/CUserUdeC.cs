namespace UdeCDocsMVC.Models.SysModels
{
    public class CUserUdeC
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Institution { get; set; }
        public string? City { get; set; }
        public string Password { get; set; } = null!;
        public int? Idfaculty { get; set; }
    }
}

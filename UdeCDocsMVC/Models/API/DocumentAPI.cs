namespace UdeCDocsMVC.Models.API
{
    public class DocumentAPI
    {
        public DocumentAPI()
        {
            Comments = new HashSet<CommentAPI>();
        }
        public string Name { get; set; } = null!;
        public string Abstract { get; set; } = null!;
        public string Keywords { get; set; } = null!;
        public DateTime PublicationDate { get; set; }
        public string Authors { get; set; } = null!;
        public string Direction { get; set; } = null!;
        public int Upvotes { get; set; } = 0;
        public int Downvotes { get; set; } = 0;

        public virtual ICollection<CommentAPI> Comments { get; set; }
    }
}

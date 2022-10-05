namespace UdeCDocs.Models.Response
{
    public class Response
    {
        public int State { get; set; } 

        public string Message { get; set; }

        public object Data { get; set; }

        public Response()
        { 
            this.State = 0;
        }
    }
}

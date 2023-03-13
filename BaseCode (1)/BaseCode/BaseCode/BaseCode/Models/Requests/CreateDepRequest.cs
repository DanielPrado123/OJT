namespace BaseCode.Models.Requests
{
    public class CreateDepRequest
    {
        public int DepId { get; set; }
        public string DepName { get; set; }
        public string DepDescription { get; set; }
        public string DepStatus { get; set; }
    }
}

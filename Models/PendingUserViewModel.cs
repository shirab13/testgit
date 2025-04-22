namespace GitGenius.Models
{
    public class PendingUserViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string IdNumber { get; set; }
        public string Role { get; set; }
        public string ApprovalFilePath { get; set; }
    }
}

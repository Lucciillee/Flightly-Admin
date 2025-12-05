namespace ProjectWebApp.ViewModels
{
    public class AdminUserVM
    {
        public string IdentityUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsDeleted { get; set; }
    }
}

namespace ProjectWebApp.ViewModels
{
    public class CreateEmployeeVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string TempPassword { get; set; }
        public string Role { get; set; }  // Admin or Sub-Admin
    }
}

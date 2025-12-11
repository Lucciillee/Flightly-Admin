using System.ComponentModel.DataAnnotations;

namespace ProjectWebApp.ViewModels
{
    public class CreateEmployeeVM
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Temporary password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string TempPassword { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }  // Admin or Sub-Admin
    }
}

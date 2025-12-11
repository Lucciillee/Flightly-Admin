using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebApp.ViewModels
{
    public class EditAirlineVM
    {
        public int AirlineId { get; set; }

        [Required(ErrorMessage = "Airline name is required")]
        [StringLength(100)]
        public string AirlineName { get; set; }

        [Required(ErrorMessage = "Airline code is required")]
        [StringLength(10)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int StatusId { get; set; }

        public string ExistingLogoUrl { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Statuses { get; set; }
    }
}

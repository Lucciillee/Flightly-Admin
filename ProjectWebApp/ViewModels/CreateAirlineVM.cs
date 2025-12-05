using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebApp.ViewModels
{
    public class CreateAirlineVM
    {
        [Required]
        public string AirlineName { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public int CountryId { get; set; }

        [Required]
        public int StatusId { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Statuses { get; set; }
    }
}

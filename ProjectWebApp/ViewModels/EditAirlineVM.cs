using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectWebApp.ViewModels
{
    public class EditAirlineVM
    {
        public int AirlineId { get; set; }

        public string AirlineName { get; set; }
        public string Code { get; set; }
        public int CountryId { get; set; }
        public int StatusId { get; set; }

        public string ExistingLogoUrl { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Statuses { get; set; }
    }
}

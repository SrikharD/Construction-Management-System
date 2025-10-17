using Microsoft.AspNetCore.Mvc;
using Dogiparthy_Spring25.ViewModels;
using Dogiparthy_Spring25.Models;

namespace Dogiparthy_Spring25.ViewModels
{
    public class ProjectSiteSearchViewModel 
    {
        public ProjectSite ProjectSite { get; set; }
        public string SearchError { get; set; }
        public List<ProjectSite> ResultList { get; set;}
        
        public ProjectSiteSearchViewModel()
        {
            ResultList = new List<ProjectSite>();
        }

    }
}

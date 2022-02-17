using NewAddLogin.Data;
using System.Collections.Generic;

namespace NewAddLogin.Web.Models

{
    public class HomePageViewModel
    {
        public List <Ad> Ads {get; set;} 
        public int UserId {get; set;}
        public bool IsAuthenticated { get; set; }
    }
}

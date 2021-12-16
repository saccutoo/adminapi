using System;
using System.Collections.Generic;

namespace Admin.API.Infrastructure.Migrations
{
    public partial class SaLogActions
    {
        public decimal Id { get; set; }
        public string Functioncode { get; set; }
        public string Functionname { get; set; }
        public string Actiontype { get; set; }
        public string Actionname { get; set; }
        public string Actionbyuser { get; set; }
        public DateTime? Actiontime { get; set; }
        public string Objectcontentnew { get; set; }
        public string Objectcontentold { get; set; }
    }
}

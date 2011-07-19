using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class PageTracking : DomainObject
    {
        public PageTracking()
        {
            SetDefaults();
        }

        public PageTracking(string loginId, string location, string ipAddress)
        {
            LoginId = loginId;
            Location = location;
            IPAddress = ipAddress;

            SetDefaults();
        }

        private void SetDefaults()
        {
            DateTime = DateTime.Now;
        }

        public virtual string LoginId { get; set; }
        public virtual string Location { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual DateTime DateTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class ApplicationKey : DomainObject
    {
        public virtual string Application { get; set; }
        public virtual string Key { get; set; }
        public virtual bool IsActive { get; set; }
    }
}

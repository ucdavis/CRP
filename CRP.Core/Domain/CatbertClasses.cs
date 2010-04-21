using System;
using System.Collections.Generic;
using System.Text;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class Unit : DomainObject
    {
        public virtual string FullName { get; set; }
        public virtual string ShortName { get; set; }
        public virtual string PPS_Code { get; set; }
        public virtual string FIS_Code { get; set; }

        public virtual School School { get; set; }
    }

    public class School : DomainObjectWithTypedId<string>
    {
        public virtual string ShortDescription { get; set; }
        public virtual string LongDescription { get; set; }
        public virtual string Abbreviation { get; set; }
    }

    public class User : DomainObject
    {
        public virtual string LoginID { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string EmployeeID { get; set; }
        public virtual string SID { get; set; }
        public virtual Guid UserKey { get; set; }

        public virtual string FullName { 
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public virtual string SortableName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public virtual ICollection<Unit> Units { get; set; }
    }
}

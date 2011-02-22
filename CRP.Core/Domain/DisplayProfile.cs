using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class DisplayProfile : DomainObject
    {
        public DisplayProfile()
        {
            SetDefaults();
        }

        public DisplayProfile(string name)
        {
            Name = name;

            SetDefaults();
        }

        private void SetDefaults()
        {
            SchoolMaster = false;
        }

        /// <summary>
        /// Populates the complex logic fields.
        /// It needs to be done from both the 
        /// IsValid and ValidationResults to work
        /// correctly from both the Controller and Repository.
        /// </summary>
        private void PopulateComplexLogicFields()
        {
            DepartmentAndSchool = true;
            DepartmentOrSchool = true;
            SchoolMasterAndSchool = true;
            if (Unit != null && School != null)
            {
                DepartmentAndSchool = false;
            }
            if (Unit == null && School == null)
            {
                DepartmentOrSchool = false;
            }     
            if(School == null && SchoolMaster)
            {
                SchoolMasterAndSchool = false;
            }
        }

        [Required]
        public virtual string Name { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual School School { get; set; }
        public virtual byte[] Logo { get; set; }

        public virtual bool SchoolMaster { get; set; }

        [Length(50)]
        public virtual string HeaderColor { get; set; }

        public override bool IsValid()
        {
            PopulateComplexLogicFields();                   
            return base.IsValid();
        }

        public override System.Collections.Generic.ICollection<UCDArch.Core.CommonValidator.IValidationResult> ValidationResults()
        {
            PopulateComplexLogicFields();   
            return base.ValidationResults();
        }


        #region Fields ONLY used for complex validation, not in database
        
        [AssertTrue(Message = "Department and School cannot be selected together.")]
        private bool DepartmentAndSchool { get; set; }

        [AssertTrue(Message = "A Department or School must be specified.")]
        private bool DepartmentOrSchool { get; set; }
        
        [AssertTrue(Message = "SchoolMaster may only be true when School is selected.")]
        private bool SchoolMasterAndSchool { get; set; }

        #endregion Fields ONLY used for complex validation, not in database
    }
}

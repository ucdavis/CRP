using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;


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
            DepartmentAndSchool = false;
            DepartmentOrSchool = false;
            SchoolMasterAndSchool = false;
            if (Unit != null && School != null)
            {
                DepartmentAndSchool = true;
            }
            if (Unit == null && School == null)
            {
                DepartmentOrSchool = true;
            }     
            if(School == null && SchoolMaster)
            {
                SchoolMasterAndSchool = true;
            }
        }

        [Required]
        public virtual string Name { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual School School { get; set; }
        public virtual byte[] Logo { get; set; }

        public virtual bool SchoolMaster { get; set; }

        public virtual string CustomCss { get; set; }

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


        #region Fields ONLY used for complex validation, not in database (note, have to be public and inited in constructor)
        
        [AssertFalse(ErrorMessage = "Department and School cannot be selected together.")]
        public virtual bool DepartmentAndSchool { get; set; }

        [AssertFalse(ErrorMessage = "A Department or School must be specified.")]
        public virtual bool DepartmentOrSchool { get; set; }

        [AssertFalse(ErrorMessage = "SchoolMaster may only be true when School is selected.")]
        public virtual bool SchoolMasterAndSchool { get; set; }

        #endregion Fields ONLY used for complex validation, not in database
    }
}

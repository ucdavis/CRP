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
            UnitAndSchool = true;
            UnitOrSchool = true;
            SchoolMasterAndSchool = true;
            if (Unit != null && School != null)
            {
                UnitAndSchool = false;
            }
            if (Unit == null && School == null)
            {
                UnitOrSchool = false;
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
        
        [AssertTrue(Message = "Unit and School cannot be selected together.")]
        private bool UnitAndSchool { get; set; }
        
        [AssertTrue(Message = "A Unit or School must be specified.")]
        private bool UnitOrSchool { get; set; }
        
        [AssertTrue(Message = "SchoolMaster may only be true when School is selected.")]
        private bool SchoolMasterAndSchool { get; set; }

        #endregion Fields ONLY used for complex validation, not in database
    }
}

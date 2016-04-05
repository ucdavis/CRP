using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;


namespace CRP.Core.Domain
{
    public class Template : DomainObject
    {
        public Template()
        {
            SetDefaults();
        }

        public Template(string text)
        {
            Text = text;

            SetDefaults();
        }

        private void SetDefaults()
        {
            Default = false;
            //ItemAndDefault = false;
        }

        [Required]
        public virtual string Text { get; set; }
        public virtual Item Item { get; set; }
        public virtual bool Default { get; set; }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid()
        {
            //PopulateComplexLogicFields();
            return base.IsValid();
        }

        public override ICollection<UCDArch.Core.CommonValidator.IValidationResult> ValidationResults()
        {
            //PopulateComplexLogicFields();
            return base.ValidationResults();
        }

        /// <summary>
        /// Populates the complex logic fields.
        /// </summary>
        //private void PopulateComplexLogicFields()
        //{
        //    //ItemAndDefault = true;
        //    if (Default == false && Item == null)
        //    {
        //        //ItemAndDefault = false;
        //    }
        //}

        #region Fields ONLY used for complex validation, not in database
        [AssertFalse(ErrorMessage = "Item may not be empty when default not selected")]
        public virtual bool ItemAndDefault
        {
            get
            {
                if (Default == false && Item == null)
                {
                    return true;
                }
                return false;
            }
        }

        #endregion Fields ONLY used for complex validation, not in database
    }
}

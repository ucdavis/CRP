using System.Collections.Generic;
using CRP.Core.Helpers;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace CRP.Core.Domain
{
    public class ItemReport : DomainObject
    {
        public ItemReport()
        {
            SetDefaults();
        }

        public ItemReport(string name, Item item, User user)
        {
            Name = name;
            Item = item;
            User = user;

            SetDefaults();
        }

        private void SetDefaults()
        {
            SystemReusable = false;
            Columns = new List<ItemReportColumn>();
            ReportColumns = false;
        }

        [Required]
        [Length(50)]
        public virtual string Name{ get; set; }
        [NotNull]
        public virtual Item Item { get; set; }
        [NotNull]
        public virtual User User { get; set; }
        public virtual bool SystemReusable { get; set; }
        [NotNull]
        public virtual ICollection<ItemReportColumn> Columns { get; set; }      

        public virtual void AddReportColumn(ItemReportColumn itemReportColumn)
        {
            itemReportColumn.ItemReport = this;
            itemReportColumn.Order = Columns.Count + 1;
            Columns.Add(itemReportColumn);
        }

        public virtual void RemoveReportColumn(ItemReportColumn itemReportColumn)
        {
            Columns.Remove(itemReportColumn);
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid()
        {
            PopulateComplexLogicFields();
            return base.IsValid();
        }

        public override ICollection<UCDArch.Core.CommonValidator.IValidationResult> ValidationResults()
        {
            PopulateComplexLogicFields();
            return base.ValidationResults();
        }

        /// <summary>
        /// Populates the complex logic fields.
        /// We can't just put the [Valid] attribute on the Columns because it doesn't run the overridden isValid method. 
        /// </summary>
        private void PopulateComplexLogicFields()
        {
            ReportColumns = true;
            if (Columns != null && Columns.Count > 0)
            {
                foreach (var column in Columns)
                {
                    if (!column.IsValid())
                    {
                        ReportColumns = false;
                        break;
                    }
                }
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "One or more Report Columns is not valid")]
        public virtual bool ReportColumns { get; set; }
        #endregion Fields ONLY used for complex validation, not in database
    }

    public class ItemReportColumn : DomainObject
    {
        public ItemReportColumn()
        {
            SetDefaults();
        }

        public ItemReportColumn(string name, ItemReport itemReport)
        {
            Name = name;
            ItemReport = itemReport;

            SetDefaults();
        }

        private void SetDefaults()
        {
            Order = 0;
            Quantity = false;
            Transaction = false;
            Property = false;
            QuantityAndTransactionAndProperty = false;
        }

        [Required]
        [Length(200)]
        public virtual string Name { get; set; }
        [Length(50)]
        public virtual string Format { get; set; }
        [NotNull]
        public virtual ItemReport ItemReport { get; set; }
        public virtual int Order { get; set; }
        public virtual bool Quantity { get; set; }
        public virtual bool Transaction { get; set; }
        public virtual bool Property { get; set; }
        public virtual QuestionSet QuestionSet { get; set; }

        public override bool IsValid()
        {
            PopulateComplexLogicFields();
            return base.IsValid();
        }

        public override ICollection<UCDArch.Core.CommonValidator.IValidationResult> ValidationResults()
        {
            PopulateComplexLogicFields();
            return base.ValidationResults();
        }

        private void PopulateComplexLogicFields()
        {
            QuantityAndTransactionAndProperty = true;
            if(Quantity.ToInt() + Transaction.ToInt() + Property.ToInt() != 1)
            {
                QuantityAndTransactionAndProperty = false;
            }
        }

        #region Fields ONLY used for complex validation, not in database
        [AssertTrue(Message = "One and only one of these must be selected: Quantity, Transaction, Property")]
        public virtual bool QuantityAndTransactionAndProperty { get; set; }      
        #endregion Fields ONLY used for complex validation, not in database
    }
}

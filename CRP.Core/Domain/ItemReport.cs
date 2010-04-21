using System.Collections.Generic;
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
        }

        [Required]
        [Length(50)]
        public virtual string Name{ get; set; }
        [NotNull]
        public virtual Item Item { get; set; }
        [NotNull]
        public virtual User User { get; set; }
        public virtual bool SystemReusable { get; set; }

        public virtual ICollection<ItemReportColumn> Columns { get; set; }      

        public virtual void AddReportColumn(ItemReportColumn itemReportColumn)
        {
            itemReportColumn.ItemReport = this;
            itemReportColumn.Order = Columns.Count + 1;
            Columns.Add(itemReportColumn);
        }

        public virtual void RemoveReportColum(ItemReportColumn itemReportColumn)
        {
            Columns.Remove(itemReportColumn);
        }
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
        [Length(50)]
        public virtual string DisplayName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Mvc.Controllers.ViewModels
{
    public class ItemListView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual decimal CostPerItem { get; set; }
        public virtual int Quantity { get; set; }
        public virtual int Sold { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual bool Available { get; set; }
    }
}
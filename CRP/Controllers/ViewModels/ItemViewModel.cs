using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemViewModel
    {
        public bool IsNew { get; set; }
        public bool CanChangeFID { get; set; }

        public string PaidText { get; set; }
        public string UnpaidText { get; set; }
        public Template Template { get; set; }

        public static ItemViewModel Create(IRepository repository, IPrincipal principal, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ItemViewModel(){
                ItemTypes = repository.OfType<ItemType>().Queryable.Where(a => a.IsActive).ToList(),
                Users = repository.OfType<User>().Queryable.Where(a => a.ActiveUserId != null),
                CurrentUser = repository.OfType<User>().Queryable.Where(a => a.LoginID == principal.Identity.Name).FirstOrDefault(),
                TouchnetFIDs = repository.OfType<TouchnetFID>().GetAll()
            };

            viewModel.UserUnit = viewModel.CurrentUser.Units.FirstOrDefault();
            viewModel.CanChangeFID = false;

            if (principal.IsInRole(RoleNames.Admin))
            {
                viewModel.Units = repository.OfType<Unit>().GetAll();
                viewModel.CanChangeFID = true;
            }
            else
            {
                if (item != null && item.Unit != null && viewModel.CurrentUser.Units.Contains(item.Unit) == false)
                {
                    var tempUnits = viewModel.CurrentUser.Units.ToList();
                    tempUnits.Add(item.Unit);
                    viewModel.Units = tempUnits;
                }
                else
                {
                    viewModel.Units = viewModel.CurrentUser.Units;
                }
                if(item != null && item.Editors != null && item.Editors.Count > 0)
                {
                    var owner = item.Editors.Where(a => a.Owner).FirstOrDefault();
                    if(owner != null && owner.User != null && owner.User.LoginID == principal.Identity.Name)
                    {
                        viewModel.CanChangeFID = true;
                    }
                }
            }
            if(item != null)
            {
                viewModel.Item = item;

                viewModel.UnpaidText = string.Empty;
                viewModel.PaidText = string.Empty;
                if (item.Template != null && item.Template.Text.Contains(StaticValues.ConfirmationTemplateDelimiter))
                {
                    //var index = template.Text.IndexOf("<<PaidTextAbove>>");
                    var delimiter = new string[] { StaticValues.ConfirmationTemplateDelimiter };
                    var parse = item.Template.Text.Split(delimiter, StringSplitOptions.None);
                    viewModel.PaidText = parse[0];
                    viewModel.UnpaidText = parse[1];
                }
                else if (item.Template != null)
                {
                    viewModel.PaidText = item.Template.Text;
                    viewModel.UnpaidText = string.Empty;
                }
            }



            viewModel.IsNew = false; //Set to true in Create methods

            return viewModel;
        }

        public IQueryable<User> Users { get; set; }
        public IEnumerable<ItemType> ItemTypes { get; set; }
        public Item Item { get; set; }
        public User CurrentUser { get; set; }
        public IEnumerable<Unit> Units { get; set; }
        public Unit UserUnit { get; set; }
        public IEnumerable<TouchnetFID> TouchnetFIDs { get; set; }
    }
}

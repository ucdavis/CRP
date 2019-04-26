using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Mvc.Controllers.ViewModels.ItemManagement;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.ViewModels
{
    public class ItemViewModel
    {
        public bool IsNew { get; set; }

        public bool CanChangeFinanceAccount { get; set; }

        public string PaidText { get; set; }

        public string UnpaidText { get; set; }

        public Template Template { get; set; }

        public IQueryable<User> Users { get; set; }

        public IReadOnlyList<ItemType> ItemTypes { get; set; }

        public EditItemViewModel Item { get; set; }

        public User CurrentUser { get; set; }

        public IReadOnlyList<Unit> Units { get; set; }

        public Unit UserUnit { get; set; }

        public IReadOnlyList<FinancialAccount> FinancialAccounts { get; set; }

        public static ItemViewModel Create(IRepository repository, IPrincipal principal, Item item)
        {
            Check.Require(repository != null, "Repository is required.");

            var viewModel = new ItemViewModel() {
                ItemTypes         = repository.OfType<ItemType>().Queryable.Where(a => a.IsActive).ToList(),
                Users             = repository.OfType<User>().Queryable.Where(a => a.ActiveUserId != null),
                CurrentUser       = repository.OfType<User>().Queryable.Where(a => a.LoginID == principal.Identity.Name).FirstOrDefault(),
                FinancialAccounts = repository.OfType<FinancialAccount>().GetAll().ToList(),
            };

            viewModel.UserUnit = viewModel.CurrentUser.Units.FirstOrDefault();
            viewModel.CanChangeFinanceAccount = false;

            // setup dropdowns
            if (principal.IsInRole(RoleNames.Admin))
            {
                viewModel.Units = repository.OfType<Unit>().GetAll().ToList();
                viewModel.CanChangeFinanceAccount = true;
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
                    viewModel.Units = viewModel.CurrentUser.Units.ToList();
                }

                if (item != null && item.Editors != null && item.Editors.Count > 0)
                {
                    var owner = item.Editors.Where(a => a.Owner).FirstOrDefault();
                    if(owner != null && owner.User != null && owner.User.LoginID == principal.Identity.Name)
                    {
                        viewModel.CanChangeFinanceAccount = true;
                    }
                }
            }

            // setup model
            if(item != null)
            {
                viewModel.Item = new EditItemViewModel
                {
                    Id                       = item.Id,

                    Name                     = item.Name,
                    Description              = item.Description,
                    CheckPaymentInstructions = item.CheckPaymentInstructions,
                    CostPerItem              = item.CostPerItem,
                    Quantity                 = item.Quantity,
                    QuantityName             = item.QuantityName,
                    Expiration               = item.Expiration,
                    Link                     = item.Link,

                    DonationLinkInformation  = item.DonationLinkInformation,
                    DonationLinkLegend       = item.DonationLinkLegend,
                    DonationLinkLink         = item.DonationLinkLink,
                    DonationLinkText         = item.DonationLinkText,

                    Available                = item.Available,
                    Private                  = item.Private,
                    NotifyEditors            = item.NotifyEditors,
                    RestrictedKey            = item.RestrictedKey,
                    AllowCheckPayment        = item.AllowCheckPayment,
                    AllowCreditPayment       = item.AllowCreditPayment,
                    Summary                  = item.Summary,

                    ItemTypeId               = item.ItemType.Id,
                    UnitId                   = item.Unit?.Id ?? 0,
                    FinancialAccountId       = item.FinancialAccount?.Id ?? 0,

                    Tags                     = item.Tags.Select(t => t.Name).ToArray(),
                    ExtendedPropertyAnswers  = item.ExtendedPropertyAnswers.ToList(),
                    Coupons                  = item.Coupons.ToList(),
                    Editors                  = item.Editors.ToList(),
                    QuestionSets             = item.QuestionSets.ToList(),
                };

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
    }
}

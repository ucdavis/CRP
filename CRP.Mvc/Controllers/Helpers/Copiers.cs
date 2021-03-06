﻿using System;
using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check = UCDArch.Core.Utils.Check;
using System.Linq;
using CRP.Mvc.Controllers.ViewModels.ItemManagement;

namespace CRP.Controllers.Helpers
{
    public class Copiers
    {
        public static Item CopyItem(IRepository repository, EditItemViewModel src, Item dest, ExtendedPropertyParameter[] extendedProperties, string[] tags, string mapLink, bool CanChangeFinanceAccount)
        {
            Check.Require(repository != null, "Repository is required.");
            Check.Require(src != null, "Source item is required.");
            Check.Require(dest != null, "Destination item is required.");

            // copy the fields from the source
            dest.Name                     = src.Name;
            dest.Description              = src.Description;
            dest.CheckPaymentInstructions = src.CheckPaymentInstructions;
            dest.CostPerItem              = src.CostPerItem.Value;
            dest.Quantity                 = src.Quantity.Value;
            dest.QuantityName             = src.QuantityName; //Was missing this JCS 2010/06/29
            dest.Expiration               = src.Expiration;
            dest.Link                     = src.Link;

            dest.DonationLinkInformation = src.DonationLinkInformation;
            dest.DonationLinkLegend      = src.DonationLinkLegend;
            dest.DonationLinkLink        = src.DonationLinkLink;
            dest.DonationLinkText        = src.DonationLinkText;

            dest.Available          = src.Available;
            dest.Private            = src.Private;
            dest.NotifyEditors      = src.NotifyEditors;
            dest.RestrictedKey      = src.RestrictedKey;
            dest.AllowCheckPayment  = src.AllowCheckPayment;
            dest.AllowCreditPayment = src.AllowCreditPayment;
            dest.Summary            = src.Summary;

            if (src.Image != null)
            {
                //Only populate dest if an image was supplied 
                dest.Image = src.Image;
            }

            // lookup references
            var unit = repository.OfType<Unit>().GetNullableById(src.UnitId);
            dest.Unit = unit;

            if (CanChangeFinanceAccount)
            {
                var account = repository.OfType<FinancialAccount>().GetNullableById(src.FinancialAccountId);
                dest.FinancialAccount = account;
            }

            //Try to get item type.
            var itemType = repository.OfType<ItemType>().GetNullableById(src.ItemTypeId);
            dest.ItemType = itemType;

            PopulateItem(repository, dest, extendedProperties, tags, mapLink);

            return dest;
        }

        public static Item PopulateItem(IRepository repository, Item item, ExtendedPropertyParameter[] extendedProperties, string[] tags, string mapLink)
        {
            if (extendedProperties != null)
            {
                // go through and deal with the extended properties
                foreach (var exp in extendedProperties)
                {
                    // check to see if the extended property answer has already been created
                    var extendedProperty = repository.OfType<ExtendedProperty>().GetNullableById(exp.propertyId);
                    var epa =
                        item.ExtendedPropertyAnswers.Where(a => a.ExtendedProperty == extendedProperty).FirstOrDefault();

                    if (epa == null)
                    {
                        epa = new ExtendedPropertyAnswer(exp.value, item, extendedProperty);
                        item.AddExtendedPropertyAnswer(epa);
                    }
                    else
                    {
                        epa.Answer = exp.value;
                    }
                }
            }

            // go through and deal with the tags
            var existingTags = repository.OfType<Tag>().GetAll();

            if (tags != null)
            {
                foreach (var s in tags)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        // check to see if it's already associated with the item
                        var tag = item.Tags.FirstOrDefault(a => a.Name == s);
                        var existingTag = existingTags.FirstOrDefault(a => a.Name == s);

                        if (existingTag == null) // no tag with that text exists, create a new one and insert
                        {
                            item.AddTag(new Tag(s));
                        }
                        else if (existingTag != null && tag == null) // tag exists but isn't associated with the item
                        {
                            item.AddTag(existingTag);
                        }
                    }
                }

                // remove the tags that are no longer part of it
                var removeTags = item.Tags.Where(a => !tags.Contains(a.Name)).ToList(); //Can't use the IEnumerable otherwise it doesn't remove them all.

                for (int i = 0; i < removeTags.Count(); i++)
                {
                    item.Tags.Remove(removeTags[i]);
                }
            }

            if (!string.IsNullOrEmpty(mapLink))
            {
                item.MapLink = GoogleMapHelper.ParseEmbeddedLink(mapLink);
                item.LinkLink = GoogleMapHelper.ParseLinkLink(mapLink);
            }

            return item;
        }

        public static DisplayProfile CopyDisplayProfile(DisplayProfile src, DisplayProfile dest)
        {
            dest.Name = src.Name;
            dest.CustomCss = src.CustomCss;

            return dest;
        }

        public static OpenIdUser CopyOpenIdUser(OpenIdUser src, OpenIdUser dest)
        {
            Check.Require(src != null, "Source open id user is required.");
            Check.Require(dest != null, "Destination open id user is required.");

            dest.Email = src.Email;
            dest.FirstName = src.FirstName;
            dest.LastName = src.LastName;
            dest.StreetAddress = src.StreetAddress;
            dest.Address2 = src.Address2;
            dest.City = src.City;
            dest.State = src.State;
            dest.Zip = src.Zip;
            dest.PhoneNumber = src.PhoneNumber;

            return dest;
        }

        public static PaymentLog CopyCheckValues(PaymentLog src, PaymentLog dest)
        {
            Check.Require(src != null, "Source payment log is required.");
            Check.Require(dest != null, "Destination payment log is required.");

            dest.Name = src.Name;
            dest.CheckNumber = src.CheckNumber;
            dest.Amount = src.Amount;
            dest.DatePayment = src.DatePayment;
            dest.Notes = src.Notes;
            dest.Accepted = src.Accepted;

            return dest;
        }
    }
}

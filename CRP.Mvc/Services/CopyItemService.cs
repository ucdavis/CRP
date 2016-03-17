using System;
using System.Collections.Generic;
using System.Linq;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;
using CRP.Core.Resources;
using UCDArch.Core.PersistanceSupport;

namespace CRP.Services
{
    public interface ICopyItemService
    {
        Item Copy(Item sourceItem, IRepository repository, string currentUserName);
    }


    public class CopyItemService : ICopyItemService
    {
        public Item Copy(Item sourceItem, IRepository repository, string currentUserName)
        {
            var questionSetDict = new Dictionary<int, QuestionSet>();

            var rtItem = new Item(string.Format("{0} (copy)",sourceItem.Name), sourceItem.Quantity);

            CopyItemFields(sourceItem, rtItem);
            CopyTags(sourceItem, rtItem);
            CopyExtendedPropertyAnswers(sourceItem, rtItem);
            CopyCoupons(sourceItem, rtItem);
            CopyEditors(sourceItem, rtItem, repository, currentUserName);            
            CopyTemplates(sourceItem, rtItem);
            CopyMapPins(sourceItem, rtItem);
            CopyQuestionSets(sourceItem, rtItem, repository, questionSetDict);

            if (rtItem.IsValid())
            {
                repository.OfType<Item>().EnsurePersistent(rtItem);

                CopyReports(sourceItem, rtItem, repository, currentUserName, questionSetDict);

                
            }


            return rtItem;
        }

        private static void CopyReports(Item sourceItem, Item rtItem, IRepository repository, string currentUserName, Dictionary<int, QuestionSet> questionSetDict)
        {
            var user = repository.OfType<User>().Queryable.Where(a => a.LoginID == currentUserName).Single();
            foreach (var itemReport in sourceItem.Reports)
            {
                var rtItemReport = new ItemReport(itemReport.Name, rtItem, user);
                foreach (var itemReportColumn in itemReport.Columns)
                {
                    var rtColumn = new ItemReportColumn(itemReportColumn.Name, rtItemReport);
                    rtColumn.Order = itemReportColumn.Order;
                    rtColumn.Property = itemReportColumn.Property;
                    rtColumn.Quantity = itemReportColumn.Quantity;
                    if (itemReportColumn.QuestionSet != null)
                    {
                        rtColumn.QuestionSet = questionSetDict[itemReportColumn.QuestionSet.Id];
                    }
                    rtColumn.Transaction = itemReportColumn.Transaction;
                    rtColumn.Format = itemReportColumn.Format;
                    rtItemReport.AddReportColumn(rtColumn);
                }
                if (rtItemReport.IsValid())
                {
                    repository.OfType<ItemReport>().EnsurePersistent(rtItemReport);
                    rtItem.Reports.Add(rtItemReport);
                }
                
            }
        }

        private static void CopyTemplates(Item sourceItem, Item rtItem)
        {
            var template = new Template(sourceItem.Template.Text);
            rtItem.Template = template;
        }

        private static void CopyQuestionSets(Item sourceItem, Item rtItem, IRepository repository, Dictionary<int, QuestionSet> questionSetDict)
        {
            // add the required question set
            var questionSet = repository.OfType<QuestionSet>().Queryable
                .Where(a => a.Name == StaticValues.QuestionSet_ContactInformation).FirstOrDefault();
            rtItem.AddTransactionQuestionSet(questionSet);
            questionSetDict.Add(questionSet.Id, questionSet);

            foreach (var itemQuestionSet in sourceItem.QuestionSets)
            {
                if (itemQuestionSet.QuestionSet.Name == StaticValues.QuestionSet_ContactInformation)
                {
                    continue;
                }
                //if (!itemQuestionSet.QuestionSet.IsActive)
                //{
                //    continue; 
                //}
                var qSet = new QuestionSet(itemQuestionSet.QuestionSet.Name);
                qSet.IsActive = itemQuestionSet.QuestionSet.IsActive; 
                foreach (var question in itemQuestionSet.QuestionSet.Questions.OrderBy(a => a.Order))
                {
                    var rtQuestion = new Question(question.Name, question.QuestionType);
                    rtQuestion.Order = question.Order;
                    rtQuestion.QuestionSet = qSet;
                    foreach (var questionOption in question.Options)
                    {
                        var rtQuestionOption = new QuestionOption(questionOption.Name);
                        rtQuestion.AddOption(rtQuestionOption);
                    }
                    foreach (var validator in question.Validators)
                    {
                        rtQuestion.Validators.Add(validator);
                    }

                    qSet.AddQuestion(rtQuestion);                  
                }
                questionSetDict.Add(itemQuestionSet.QuestionSet.Id, qSet);
                var iQSet = new ItemQuestionSet(rtItem, qSet, itemQuestionSet.Order);
                iQSet.TransactionLevel = itemQuestionSet.TransactionLevel;
                iQSet.QuantityLevel = itemQuestionSet.QuantityLevel;
                rtItem.QuestionSets.Add(iQSet);
            }
        }

        private static void CopyEditors(Item sourceItem, Item rtItem, IRepository repository, string currentUserName)
        {
            var user = repository.OfType<User>().Queryable.Where(a => a.LoginID == currentUserName).Single();
            var owner = new Editor(rtItem, user);
            owner.Owner = true;
            rtItem.AddEditor(owner);

            foreach (var editor in sourceItem.Editors.Where(a => a.User.LoginID != currentUserName))
            {
                var edtr = new Editor(rtItem, editor.User);
                edtr.Owner = false;
                rtItem.AddEditor(edtr);
            }

            
        }

        private static void CopyCoupons(Item sourceItem, Item rtItem)
        {
            foreach (var coupon in sourceItem.Coupons.Where(a => a.IsActive))
            {
                if (coupon.Expiration == null) //We can't guess this, and it can't be edited. So Skip these if not null
                {
                    var coup = new Coupon(CouponGenerator.GenerateCouponCode(), rtItem, coupon.UserId);
                    coup.MaxQuantity = coupon.MaxQuantity;
                    coup.MaxUsage = coupon.MaxUsage;
                    coup.DiscountAmount = coupon.DiscountAmount;
                    coup.Email = coupon.Email;

                    rtItem.AddCoupon(coup);
                }
            }
        }

        private static void CopyExtendedPropertyAnswers(Item sourceItem, Item rtItem)
        {
            foreach (var extendedPropertyAnswer in sourceItem.ExtendedPropertyAnswers)
            {
                var epa = new ExtendedPropertyAnswer(extendedPropertyAnswer.Answer, rtItem,
                                                     extendedPropertyAnswer.ExtendedProperty);
                rtItem.AddExtendedPropertyAnswer(epa);
            }
        }

        private static void CopyTags(Item sourceItem, Item rtItem)
        {
            foreach (var tag in sourceItem.Tags)
            {
                rtItem.AddTag(tag);

            }
        }

        private static void CopyMapPins(Item sourceItem, Item rtItem)
        {
            foreach (var mapPin in sourceItem.MapPins)
            {
                var rtMapPin = new MapPin(rtItem, mapPin.IsPrimary, mapPin.Latitude, mapPin.Longitude);
                rtMapPin.Title = mapPin.Title;
                rtMapPin.Description = mapPin.Description;
                rtItem.AddMapPin(rtMapPin);
            }
        }

        private static void CopyItemFields(Item sourceItem, Item rtItem)
        {
            rtItem.Summary = sourceItem.Summary;
            rtItem.Description = sourceItem.Description;
            rtItem.CostPerItem = sourceItem.CostPerItem;
            rtItem.QuantityName = sourceItem.QuantityName;
            if (sourceItem.Expiration != null && (DateTime)sourceItem.Expiration > DateTime.Now)
            {
                rtItem.Expiration = sourceItem.Expiration;
            }
            else
            {
                rtItem.Expiration = DateTime.Now.Date.AddMonths(1);
            }
            rtItem.Image = sourceItem.Image;
            rtItem.Link = sourceItem.Link;

            rtItem.DonationLinkInformation = sourceItem.DonationLinkInformation;
            rtItem.DonationLinkLegend = sourceItem.DonationLinkLegend;
            rtItem.DonationLinkLink = sourceItem.DonationLinkLink;
            rtItem.DonationLinkText = sourceItem.DonationLinkText;

            rtItem.AllowCheckPayment = sourceItem.AllowCheckPayment;
            rtItem.AllowCreditPayment = sourceItem.AllowCreditPayment;
            //rtItem.HideDonation = sourceItem.HideDonation;
            rtItem.ItemType = sourceItem.ItemType;
            rtItem.Unit = sourceItem.Unit;
            rtItem.Private = sourceItem.Private;
            rtItem.RestrictedKey = sourceItem.RestrictedKey;
            //MapLink
            //LinkLink
            rtItem.CheckPaymentInstructions = sourceItem.CheckPaymentInstructions;
            rtItem.TouchnetFID = sourceItem.TouchnetFID;

        }
    }
}
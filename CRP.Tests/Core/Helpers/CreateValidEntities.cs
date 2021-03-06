﻿using System;
using System.Collections.Generic;
using CRP.Core.Domain;

namespace CRP.Tests.Core.Helpers
{
    public static class CreateValidEntities
    {
        #region CatbertClasses

        public static Unit Unit(int? counter)
        {
            var rtValue = new Unit();
            rtValue.FullName = "FullName" + counter.Extra();
            rtValue.ShortName = "ShortName";

            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid School</returns>
        public static School School(int? counter)
        {
            var rtValue = new School();
            rtValue.LongDescription = "LongDescription" + counter.Extra();
            rtValue.ShortDescription = "ShortDescription" + counter.Extra();

            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid User</returns>
        public static User User(int? counter)
        {
            var rtValue = new User();
            rtValue.FirstName = "FirstName" + counter.Extra();
            rtValue.Units = new List<Unit>();
            rtValue.ActiveUserId = counter;

            return rtValue;
        }

        #endregion CatbertClasses

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Check</returns>
        public static PaymentLog PaymentLog(int? counter)
        {
            var rtValue = new PaymentLog();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Transaction = new Transaction();
            rtValue.CheckNumber = 1; //Can be null when Credit is true
            rtValue.Check = true;
            rtValue.Accepted = true;
            rtValue.Credit = !rtValue.Check;
            rtValue.Amount = 1m;

            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Coupon</returns>
        public static Coupon Coupon(int? counter)
        {
            var rtValue = new Coupon();
            rtValue.Email = "email@test.edu" + counter.Extra();
            rtValue.UserId = "UserId" + counter.Extra();
            rtValue.Code = "1234567890";
            rtValue.DiscountAmount = 0.01m;
            rtValue.Item = new Item();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid DisplayProfile</returns>
        public static DisplayProfile DisplayProfile(int? counter)
        {
            var rtValue = new DisplayProfile();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Unit = new Unit();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Editor</returns>
        public static Editor Editor(int? counter)
        {
            var rtValue = new Editor();
            rtValue.Owner = false;
            rtValue.User = new User();
            rtValue.Item = new Item();
            
            return rtValue;
        }

        public static MapPin MapPin(int? counter)
        {
            var rtValue = new MapPin();
            rtValue.Item = new Item();
            rtValue.Title = "Title" + counter.Extra();
            rtValue.Description = "Description" + counter.Extra();
            rtValue.Latitude = "38.537052";
            rtValue.Longitude = "-121.749150";

            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid ExtendedProperty</returns>
        public static ExtendedProperty ExtendedProperty(int? counter)
        {
            var rtValue = new ExtendedProperty();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.ItemType = new ItemType();
            rtValue.QuestionType = new QuestionType();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid ExtendedPropertyAnswer</returns>
        public static ExtendedPropertyAnswer ExtendedPropertyAnswer(int? counter)
        {
            var rtValue = new ExtendedPropertyAnswer();
            rtValue.Answer = "Answer" + counter.Extra();
            rtValue.Item = new Item();
            rtValue.ExtendedProperty = new ExtendedProperty();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Item</returns>
        public static Item Item(int? counter)
        {
            var rtValue = new Item();
            rtValue.TouchnetFID = "001";
            rtValue.Name = "Name" + counter.Extra();
            rtValue.ItemType = new ItemType();
            rtValue.Unit = new Unit();
            rtValue.Private = false;
            rtValue.Available = true;
            rtValue.CostPerItem = 20.00m;
            rtValue.Summary = "Summary" + counter.Extra();
            rtValue.CheckPaymentInstructions =
                "<h1>Thank you for your purchase!</h1>  <h2>Please mail your payment to:</h2>  <address>John Zoidberg<br />150 Mrak Hall<br />One Shields Ave.<br />Davis, CA 94534 <br /></address>";
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid ItemQuestionSet</returns>
        public static ItemQuestionSet ItemQuestionSet(int? counter)
        {
            var rtValue = new ItemQuestionSet();
            rtValue.Item = new Item();
            rtValue.QuestionSet = new QuestionSet();
            rtValue.QuantityLevel = true;
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid ItemType</returns>
        public static ItemType ItemType(int? counter)
        {
            var rtValue = new ItemType();
            rtValue.Name = "Name" + counter.Extra();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid QuantityAnswer</returns>
        public static QuantityAnswer QuantityAnswer(int? counter)
        {
            var rtValue = new QuantityAnswer();
            rtValue.Answer = "Answer" + counter.Extra();
            rtValue.Transaction = new Transaction();
            rtValue.QuestionSet = new QuestionSet();
            rtValue.Question = new Question();
            rtValue.QuantityId = Guid.NewGuid();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Question</returns>
        public static Question Question(int? counter)
        {
            var rtValue = new Question();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.QuestionSet = new QuestionSet();
            rtValue.QuestionType = new QuestionType();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid QuestionOption</returns>
        public static QuestionOption QuestionOption(int? counter)
        {
            var rtValue = new QuestionOption();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Question = new Question();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid QuestionSet</returns>
        public static QuestionSet QuestionSet(int? counter)
        {
            var rtValue = new QuestionSet();
            rtValue.Name = "Name" + counter.Extra();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid QuestionType</returns>
        public static QuestionType QuestionType(int? counter)
        {
            var rtValue = new QuestionType();
            rtValue.Name = "Name" + counter.Extra();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Tag</returns>
        public static Tag Tag(int? counter)
        {
            var rtValue = new Tag();
            rtValue.Name = "Name" + counter.Extra();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Transaction</returns>
        public static Transaction Transaction(int? counter)
        {
            var rtValue = new Transaction();
            rtValue.Item = new Item();
            rtValue.Check = true;
            rtValue.Credit = false;
            if(counter != null)
            {
                var locCount = (int)counter;
                rtValue.TransactionNumber = string.Format("{0}-{1}", rtValue.TransactionDate.Year, locCount.ToString("000000"));
            }
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid TransactionAnswer</returns>
        public static TransactionAnswer TransactionAnswer(int? counter)
        {
            var rtValue = new TransactionAnswer();
            rtValue.Answer = "Answer" + counter.Extra();
            rtValue.Transaction = new Transaction();
            rtValue.QuestionSet = new QuestionSet();
            rtValue.Question = new Question();
            
            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Template</returns>
        public static Template Template(int? counter)
        {
            var rtValue = new Template();
            rtValue.Text = "Text" + counter.Extra();
            rtValue.Item = new Item();

            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests.
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>Valid ItemTypeQuestionSet</returns>
        public static ItemTypeQuestionSet ItemTypeQuestionSet(int? counter)
        {
            var rtValue = new ItemTypeQuestionSet();
            rtValue.ItemType = new ItemType();
            rtValue.QuestionSet = new QuestionSet();

            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests.
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>Valid ItemReport</returns>
        public static ItemReport ItemReport(int? counter)
        {
            var rtValue = new ItemReport();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Item = new Item();
            rtValue.User = new User();

            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests.
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>Valid ItemReportColumn</returns>
        public static ItemReportColumn ItemReportColumn(int? counter)
        {
            var rtValue = new ItemReportColumn();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Format = "Format" + counter.Extra();
            rtValue.ItemReport = new ItemReport();
            rtValue.Transaction = true;

            return rtValue;
        }

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Check</returns>
        public static Validator Validator(int? counter)
        {
            var rtValue = new Validator();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Class = "Class" + counter.Extra();

            return rtValue;
        }
        public static HelpTopic HelpTopic(int? counter)
        {
            var rtValue = new HelpTopic();
            rtValue.Question = "Question" + counter.Extra();
            rtValue.Answer = "Answer" + counter.Extra();
            rtValue.AvailableToPublic = true;
            return rtValue;
        }
        /// <summary>
        /// Create a valid entry for tests.
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>Valid ItemReportColumn</returns>
        public static OpenIdUser OpenIdUser(int? counter)
        {
            var rtValue = new OpenIdUser();
            rtValue.FirstName = "FirstName" + counter.Extra();
            rtValue.LastName = "LastName" + counter.Extra();
            rtValue.UserId = "UserId" + counter.Extra();

            return rtValue;
        }

        #region Helper Extension

        private static string Extra(this int? counter)
        {
            var extraString = "";
            if (counter != null)
            {
                extraString = counter.ToString();
            }
            return extraString;
        }

        #endregion Helper Extension

        public static TouchnetFID TouchnetFID(int counter)
        {
            var rtValue = new TouchnetFID("001", "Test");
            rtValue.FID = counter.ToString("00#");
            rtValue.Description = "Description" + counter;

            return rtValue;
        }
    }
}

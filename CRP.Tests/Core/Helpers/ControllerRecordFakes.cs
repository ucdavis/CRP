using System;
using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Testing;

namespace CRP.Tests.Core.Helpers
{
    /// <summary>
    /// Static methods to fake collections of records for controller tests.
    /// </summary>
    public static class ControllerRecordFakes
    {
        /// <summary>
        /// Fakes the QuestionSets.
        /// Author: Sylvestre, Jason
        /// Create: 2010/04/01
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="count">The number of QuestionSets to add.</param>
        public static void FakeQuestionSets(List<QuestionSet> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.QuestionSet(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the users.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeUsers(List<User> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.User(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the question types.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeQuestionTypes(List<QuestionType> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.QuestionType(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        public static void FakeQuestionTypes(List<QuestionType> entity)
        { 
            /*
                 IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Text Box' )
                 begin
	                 INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	                 VALUES ('Text Box', 0, 1)
                 end
                 
                IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Text Area' )
                begin
	                 INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	                VALUES ('Text Area', 0, 0)
                end
                 
                IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Boolean' )
                begin
	                INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	                VALUES ('Boolean', 0, 0)
                end

                IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Radio Buttons' )
                begin
	                INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	                VALUES ('Radio Buttons', 1, 0)
                end
                 
                IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Checkbox List' )
                begin
	                INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	                VALUES ('Checkbox List', 1, 0)
                end
                 
                IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Drop Down' )
                begin
	                INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	                VALUES ('Drop Down', 1, 0)
                end

                IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Date' )
                begin
	                INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	                VALUES ('Date', 0, 1)
                end
             */
            if(entity.Count != 0)
            {
                throw new ApplicationException("Didn't set up test correctly");
            }
            for (int i = 0; i < 7; i++)
            {
                entity.Add(CreateValidEntities.QuestionType(i + 1));
                entity[i].SetIdTo(i + 1 );
            }
            entity[0].Name = "Text Box";
            entity[0].HasOptions = false;
            entity[0].ExtendedProperty = true;

            entity[1].Name = "Text Area";
            entity[1].HasOptions = false;
            entity[1].ExtendedProperty = false;

            entity[2].Name = "Boolean";
            entity[2].HasOptions = false;
            entity[2].ExtendedProperty = false;

            entity[3].Name = "Radio Buttons";
            entity[3].HasOptions = true;
            entity[3].ExtendedProperty = false;

            entity[4].Name = "Checkbox List";
            entity[4].HasOptions = true;
            entity[4].ExtendedProperty = false;

            entity[5].Name = "Drop Down";
            entity[5].HasOptions = true;
            entity[5].ExtendedProperty = false;

            entity[6].Name = "Date";
            entity[6].HasOptions = false;
            entity[6].ExtendedProperty = true;
        }

        /// <summary>
        /// Fakes the validators.
        /// With Real lookup data as of 2010/04/23
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeValidators(List<Validator> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Validator(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        public static void FakeValidators(List<Validator> entity)
        {
            /*
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Required', 'required', '^.+$', '{0} is a required field.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Email', 'email', '^((([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$', '{0} is not a valid email.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Url', 'url', '^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|\/|\?)*)?$', '{0} is not a valid url.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Date', 'date', '^\d{4}[\/-]\d{1,2}[\/-]\d{1,2}$', '{0} is not a valid date.')
            ^(0[1-9]|1[012])[-\/](0[1-9]|[12][0-9]|3[01])[-\/](19|20)\d\d$
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Phone Number', 'phoneUS', '^\(?[\d]{3}\)?[\s-]?[\d]{3}[\s-]?[\d]{4}$', '{0} is not a valid phone number.')
GO
             */
            if (entity.Count != 0)
            {
                throw new ApplicationException("Didn't set up test correctly");
            }
            for (int i = 0; i < 5; i++)
            {
                entity.Add(CreateValidEntities.Validator(i + 1));
                entity[i].SetIdTo(i + 1);
            }

            entity[0].Name = "Required";
            entity[0].Class = "required";
            entity[0].RegEx = "^.+$";
            entity[0].ErrorMessage = "{0} is a required field";

            entity[1].Name = "Email";
            entity[1].Class = "email";
            entity[1].RegEx = @"^((([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
            entity[1].ErrorMessage = "{0} is not a valid email.";

            entity[2].Name = "Url";
            entity[2].Class = "url";
            entity[2].RegEx = @"^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|\/|\?)*)?$";
            entity[2].ErrorMessage = "{0} is not a valid url.";

            entity[3].Name = "Date";
            entity[3].Class = "date";
            //entity[3].RegEx = @"^\d{4}[\/-]\d{1,2}[\/-]\d{1,2}$";
            entity[3].RegEx = @"^(0[1-9]|1[012])[-\/](0[1-9]|[12][0-9]|3[01])[-\/](19|20)\d\d$";
            entity[3].ErrorMessage = "{0} is not a valid date.";

            entity[4].Name = "Phone Number";
            entity[4].Class = "phoneUS";
            entity[4].RegEx = @"^\(?[\d]{3}\)?[\s-]?[\d]{3}[\s-]?[\d]{4}$";
            entity[4].ErrorMessage = "{0} is not a valid phone number.";
        }

        /// <summary>
        /// Fakes the units.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeUnits(List<Unit> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Unit(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the items.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeItems(List<Item> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Item(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the item question sets.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeItemQuestionSets(List<ItemQuestionSet> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.ItemQuestionSet(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the editors.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeEditors(List<Editor> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Editor(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the schools.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeSchools(List<School> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.School(i + 1 + offSet));
                entity[i + offSet].SetIdTo((i + 1 + offSet).ToString());
            }
        }

        /// <summary>
        /// Fakes the questions.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeQuestions(List<Question> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Question(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the item types.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeItemTypes(List<ItemType> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.ItemType(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the transaction answers.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeTransactionAnswers(List<TransactionAnswer> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.TransactionAnswer(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the item reports.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeItemReports(List<ItemReport> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.ItemReport(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the transactions.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeTransactions(List<Transaction> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Transaction(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the tags.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeTags(List<Tag> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Tag(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the templates.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeTemplates(List<Template> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Template(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the display profile.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeDisplayProfile(List<DisplayProfile> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.DisplayProfile(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the open id users.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeOpenIdUsers(List<OpenIdUser> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.OpenIdUser(i + 1 + offSet));
                entity[i + offSet].SetIdTo((i + 1 + offSet).ToString());
            }
        }

        /// <summary>
        /// Fakes the coupons.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakeCoupons(List<Coupon> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.Coupon(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

        /// <summary>
        /// Fakes the payment logs.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public static void FakePaymentLogs(List<PaymentLog> entity, int count)
        {
            var offSet = entity.Count;
            for (int i = 0; i < count; i++)
            {
                entity.Add(CreateValidEntities.PaymentLog(i + 1 + offSet));
                entity[i + offSet].SetIdTo(i + 1 + offSet);
            }
        }

    }
}

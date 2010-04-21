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
            //TODO: Other Values

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
            //TODO: Other Values

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
            //TODO: Other Values

            return rtValue;
        }

        #endregion CatbertClasses

        /// <summary>
        /// Create a valid entry for tests. 
        /// Repository tests may need to modify this data to supply real linked data.
        /// </summary>
        /// <returns>Valid Check</returns>
        public static Check Check(int? counter)
        {
            var rtValue = new Check();
            rtValue.Payee = "Payee" + counter.Extra();
            //TODO: Populate values
            
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
            rtValue.Code = "Code";
            rtValue.Item = new Item();
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            rtValue.Name = "Name" + counter.Extra();
            rtValue.ItemType = new ItemType();
            rtValue.Unit = new Unit();
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
            //TODO: Populate values
            
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
    }
}

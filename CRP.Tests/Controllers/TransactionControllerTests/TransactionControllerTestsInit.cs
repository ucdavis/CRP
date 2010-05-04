using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CRP.Controllers;
using CRP.Controllers.Helpers;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using CRP.Core.Resources;
using CRP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace CRP.Tests.Controllers.TransactionControllerTests
{
    [TestClass]
    public partial class TransactionControllerTests : ControllerTestBase<TransactionController>
    {
        protected Type ControllerClass = typeof(TransactionController);
        protected IRepositoryWithTypedId<OpenIdUser, string> OpenIdUserRepository { get; set; }
        public INotificationProvider NotificationProvider { get; set; }
        protected List<Transaction> Transactions { get; set; }
        protected IRepository<Transaction> TransactionRepository { get; set; }
        protected List<Item> Items { get; set; }
        protected IRepository<Item> ItemRepository { get; set; }
        protected List<Unit> Units { get; set; }
        protected IRepository<Unit> UnitRepository { get; set; }
        protected List<DisplayProfile> DisplayProfiles { get; set; }
        protected IRepository<DisplayProfile> DisplayProfileRepository { get; set; }
        protected List<OpenIdUser> OpenIdUsers { get; set; }
        protected List<QuestionSet> QuestionSets { get; set; }
        protected IRepository<QuestionSet> QuestionSetRepository { get; set; }
        protected List<Question> Questions { get; set; }
        protected IRepository<Question> QuestionRepository { get; set; }
        protected List<Coupon> Coupons { get; set; }
        protected IRepository<Coupon> CouponRepository { get; set; }
        protected QuestionAnswerParameter[] TransactionAnswerParameters { get; set; }
        protected QuestionAnswerParameter[] QuantityAnswerParameters { get; set; }
        protected List<QuestionType> QuestionTypes { get; set; }
        protected List<Validator> Validators { get; set; }


        #region Init
        public TransactionControllerTests()
        {
            Transactions = new List<Transaction>();
            TransactionRepository = FakeRepository<Transaction>();
            Controller.Repository.Expect(a => a.OfType<Transaction>()).Return(TransactionRepository).Repeat.Any();

            Items = new List<Item>();
            ItemRepository = FakeRepository<Item>();
            Controller.Repository.Expect(a => a.OfType<Item>()).Return(ItemRepository).Repeat.Any();

            Units = new List<Unit>();
            UnitRepository = FakeRepository<Unit>();
            Controller.Repository.Expect(a => a.OfType<Unit>()).Return(UnitRepository).Repeat.Any();

            DisplayProfiles = new List<DisplayProfile>();
            DisplayProfileRepository = FakeRepository<DisplayProfile>();
            Controller.Repository.Expect(a => a.OfType<DisplayProfile>()).Return(DisplayProfileRepository).Repeat.Any();

            QuestionSets = new List<QuestionSet>();
            QuestionSetRepository = FakeRepository<QuestionSet>();
            Controller.Repository.Expect(a => a.OfType<QuestionSet>()).Return(QuestionSetRepository).Repeat.Any();

            Questions = new List<Question>();
            QuestionRepository = FakeRepository<Question>();
            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();

            Coupons = new List<Coupon>();
            CouponRepository = FakeRepository<Coupon>();
            Controller.Repository.Expect(a => a.OfType<Coupon>()).Return(CouponRepository).Repeat.Any();

            OpenIdUsers = new List<OpenIdUser>();
            //OpenIdUserRepository = FakeRepository<OpenIdUser>();
            //Controller.Repository.Expect(a => a.OfType<OpenIdUser>()).Return(OpenIdUserRepository).Repeat.Any();

            TransactionAnswerParameters = new QuestionAnswerParameter[1];
            QuestionTypes = new List<QuestionType>();
            Validators = new List<Validator>();
        }
        /// <summary>
        /// Registers the routes.
        /// </summary>
        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            OpenIdUserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<OpenIdUser, string>>();
            NotificationProvider = MockRepository.GenerateStub<INotificationProvider>();
            Controller = new TestControllerBuilder().CreateController<TransactionController>(OpenIdUserRepository, NotificationProvider);
        }

        #endregion Init

        #region Helper Methods

        /// <summary>
        /// Setups the data for tests.
        /// </summary>
        private void SetupDataForTests()
        {
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { RoleNames.Admin });
            ControllerRecordFakes.FakeItems(Items, 3);
            ControllerRecordFakes.FakeUnits(Units, 1);
            ControllerRecordFakes.FakeDisplayProfile(DisplayProfiles, 3);
            ControllerRecordFakes.FakeCoupons(Coupons, 3);

            Coupons[1].Item = Items[1];
            Coupons[1].Code = "COUPON";
            Coupons[1].IsActive = true;
            Coupons[1].DiscountAmount = 5.01m;
            Coupons[1].Unlimited = true;
            Coupons[1].Used = true; //And used
            Coupons[1].MaxQuantity = 2;
            Coupons[1].Email = string.Empty;

            Items[1].Unit = Units[0];
            DisplayProfiles[1].Unit = Units[0];

            ItemRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Any();
            ItemRepository.Expect(a => a.GetNullableByID(2)).Return(Items[1]).Repeat.Any();
            DisplayProfileRepository.Expect(a => a.Queryable).Return(DisplayProfiles.AsQueryable()).Repeat.Any();
            CouponRepository.Expect(a => a.Queryable).Return(Coupons.AsQueryable()).Repeat.Any();
        }

        /// <summary>
        /// Setups the data for populate item transaction answer.
        /// Needs Items populated
        /// </summary>
        private void SetupDataForPopulateItemTransactionAnswer()
        {
            ControllerRecordFakes.FakeOpenIdUsers(OpenIdUsers, 3);
            ControllerRecordFakes.FakeQuestionSets(QuestionSets, 3);
            ControllerRecordFakes.FakeQuestions(Questions, 10);
            LoadContactInfoQuestions();
            QuestionSets[0].Name = StaticValues.QuestionSet_ContactInformation;
            QuestionSets[0].SystemReusable = true;
            for (int i = 0; i < 9; i++)
            {
                QuestionSets[0].AddQuestion(Questions[i]);
            }
            Items[1].AddTransactionQuestionSet(QuestionSets[0]);

            OpenIdUsers[1].SetIdTo("UserName");
            OpenIdUsers[1].Address2 = "Address2";
            OpenIdUsers[1].City = "City";
            OpenIdUsers[1].Email = "Email@maily.org";
            OpenIdUsers[1].FirstName = "FirstName";
            OpenIdUsers[1].LastName = "LastName";
            OpenIdUsers[1].PhoneNumber = "555 555 5555";
            OpenIdUsers[1].State = "CA";
            OpenIdUsers[1].StreetAddress = "Address1";
            OpenIdUsers[1].Zip = "95616";

            OpenIdUserRepository.Expect(a => a.GetNullableByID("UserName")).Return(OpenIdUsers[1]).Repeat.Any();
            QuestionRepository.Expect(a => a.Queryable).Return(Questions.AsQueryable()).Repeat.Any();
        }

        private DateTime SetupDataForCheckoutTests()
        {
            var fakeDate = new DateTime(2010, 02, 14);
            SystemTime.Now = () => fakeDate;
            SetupDataForTests();
            SetupDataForPopulateItemTransactionAnswer();
            SetupDataForTransactionAnswerParameters();

            ControllerRecordFakes.FakeTransactions(Transactions, 5);
            foreach (var transaction in Transactions)
            {
                transaction.Item = Items[1];
                transaction.Quantity = 2;
                Items[1].Transactions.Add(transaction);
            }
            Items[1].Quantity = 20;
            Items[1].Available = true;
            Items[1].Expiration = fakeDate.AddDays(5);

            ControllerRecordFakes.FakeValidators(Validators);

            return fakeDate;
        }

        /// <summary>
        /// Setups the data for transaction answer parameters.
        /// </summary>
        private void SetupDataForTransactionAnswerParameters()
        {
            TransactionAnswerParameters[0] = new QuestionAnswerParameter();
            TransactionAnswerParameters[0].Answer = "bob@test.com";
            TransactionAnswerParameters[0].QuestionId = Questions[8].Id;
        }

        /// <summary>
        /// Setup data for all contact information transaction answer parameters.
        /// </summary>
        private void SetupDataForAllContactInformationTransactionAnswerParameters()
        {
            var myDict = new Dictionary<int, string>();
            myDict.Add(0, "FirstName");
            myDict.Add(1, "LastName");
            myDict.Add(2, "Address1");
            myDict.Add(3, "Address2");
            myDict.Add(4, "City");
            myDict.Add(5, "CA");
            myDict.Add(6, "95616");
            myDict.Add(7, "555 555 5555");
            myDict.Add(8, "bob@test.com");

            TransactionAnswerParameters = new QuestionAnswerParameter[9];
            for (int i = 0; i < 9; i++)
            {
                TransactionAnswerParameters[i] = new QuestionAnswerParameter();
                TransactionAnswerParameters[i].QuestionId = Questions[i].Id;
                TransactionAnswerParameters[i].Answer = myDict[i];
            }
        }

        private void SetupDataForQuantityQuestionsAnswerParameters()
        {
            var myDict = new Dictionary<int, string>();
            myDict.Add(0, "FirstName");
            myDict.Add(1, "LastName");
            myDict.Add(2, "Fish");
            QuantityAnswerParameters = new QuestionAnswerParameter[9];
            ControllerRecordFakes.FakeQuestions(Questions, 2);
            const int questionOffset = 9;
            Assert.AreEqual(12, Questions.Count, "Question setup needs to be fixed.");
            Questions[9].Name = "What is your First name?";
            Questions[9].QuestionSet = QuestionSets[2];
            Questions[10].Name = "What is your Last name?";
            Questions[10].QuestionSet = QuestionSets[2];
            Questions[11].Name = "What is your Favorite Food?";
            Questions[11].QuestionSet = QuestionSets[2];
            Items[1].AddQuantityQuestionSet(QuestionSets[2]);
            QuestionSets[2].AddQuestion(Questions[9]);
            QuestionSets[2].AddQuestion(Questions[10]);
            QuestionSets[2].AddQuestion(Questions[11]);
            var counter = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    QuantityAnswerParameters[counter] = new QuestionAnswerParameter();
                    QuantityAnswerParameters[counter].QuestionId = Questions[questionOffset + j].Id;
                    QuantityAnswerParameters[counter].Answer = myDict[j] + j;
                    QuantityAnswerParameters[counter].QuantityIndex = i;
                    QuantityAnswerParameters[counter].QuestionSetId = Questions[questionOffset + j].QuestionSet.Id;
                    counter++;
                }
            }

        }

        private void LoadContactInfoQuestions()
        {
            Questions[0].Name = StaticValues.Question_FirstName;
            Questions[1].Name = StaticValues.Question_LastName;
            Questions[2].Name = StaticValues.Question_StreetAddress;
            Questions[3].Name = StaticValues.Question_AddressLine2;
            Questions[4].Name = StaticValues.Question_City;
            Questions[5].Name = StaticValues.Question_State;
            Questions[6].Name = StaticValues.Question_Zip;
            Questions[7].Name = StaticValues.Question_PhoneNumber;
            Questions[8].Name = StaticValues.Question_Email;
        }

        #region mocks
        /// <summary>
        /// Mock the Identity. Used for getting the current user name
        /// </summary>
        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "UserName";
                }
            }
        }


        /// <summary>
        /// Mock the Principal. Used for getting the current user name
        /// </summary>
        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;
            public bool RoleReturnValue { get; set; }
            public string[] UserRoles { get; set; }

            public MockPrincipal(string[] userRoles)
            {
                UserRoles = userRoles;
            }

            public IIdentity Identity
            {
                get { return _identity ?? (_identity = new MockIdentity()); }
            }

            public bool IsInRole(string role)
            {
                if (UserRoles.Contains(role))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Mock the HTTPContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;
            private readonly int _count;
            public string[] UserRoles { get; set; }
            public MockHttpContext(int count, string[] userRoles)
            {
                _count = count;
                UserRoles = userRoles;
            }

            public override IPrincipal User
            {
                get { return _user ?? (_user = new MockPrincipal(UserRoles)); }
                set
                {
                    _user = value;
                }
            }

            public override HttpRequestBase Request
            {
                get
                {
                    return new MockHttpRequest(_count);
                }
            }
        }

        public class MockHttpRequest : HttpRequestBase
        {
            MockHttpFileCollectionBase Mocked { get; set; }

            public MockHttpRequest(int count)
            {
                Mocked = new MockHttpFileCollectionBase(count);
            }
            public override HttpFileCollectionBase Files
            {
                get
                {
                    return Mocked;
                }
            }
            //This will get past the code, but not allow an openId to be assigned to the transaction.
            public override HttpCookieCollection Cookies
            {
                get
                {
                    try
                    {
                        return new HttpCookieCollection();
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
        }


        public class MockHttpFileCollectionBase : HttpFileCollectionBase
        {
            public int Counter { get; set; }

            public MockHttpFileCollectionBase(int count)
            {
                Counter = count;
                for (int i = 0; i < count; i++)
                {
                    BaseAdd("Test" + (i + 1), new byte[] { 4, 5, 6, 7, 8 });
                }

            }

            public override int Count
            {
                get
                {
                    return Counter;
                }
            }
            public override HttpPostedFileBase Get(string name)
            {
                return new MockHttpPostedFileBase();
            }
            public override HttpPostedFileBase this[string name]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
            public override HttpPostedFileBase this[int index]
            {
                get
                {
                    return new MockHttpPostedFileBase();
                }
            }
        }

        public class MockHttpPostedFileBase : HttpPostedFileBase
        {
            public override int ContentLength
            {
                get
                {
                    return 5;
                }
            }
            public override string FileName
            {
                get
                {
                    return "Mocked File Name";
                }
            }
            public override Stream InputStream
            {
                get
                {
                    var memStream = new MemoryStream(new byte[] { 4, 5, 6, 7, 8 });
                    return memStream;
                }
            }
        }

        #endregion

        #endregion Helper Methods
    }
}

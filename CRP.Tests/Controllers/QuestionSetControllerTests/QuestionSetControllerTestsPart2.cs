using CRP.Controllers;
using CRP.Core.Domain;
using CRP.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace CRP.Tests.Controllers.QuestionSetControllerTests
{
    public partial class QuestionSetControllerTests
    {
        #region Route Tests

        /// <summary>
        /// Tests the index mapping.
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/QuestionSet/Index".ShouldMapTo<QuestionSetController>(a => a.Index());
        }

        /// <summary>
        /// Tests the list mapping.
        /// </summary>
        [TestMethod]
        public void TestListMapping()
        {
            "~/QuestionSet/List".ShouldMapTo<QuestionSetController>(a => a.List());
        }

        /// <summary>
        /// Tests the details mapping.
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/QuestionSet/Details/5".ShouldMapTo<QuestionSetController>(a => a.Details(5));
        }

        /// <summary>
        /// Tests the edit mapping.
        /// </summary>
        [TestMethod]
        public void TestEditMapping()
        {
            "~/QuestionSet/Edit/5".ShouldMapTo<QuestionSetController>(a => a.Edit(5));
        }

        /// <summary>
        /// Tests the edit with parameters mapping.
        /// </summary>
        [TestMethod]
        public void TestEditWithParametersMapping()
        {
            "~/QuestionSet/Edit/5".ShouldMapTo<QuestionSetController>(a => a.Edit(5, new QuestionSet()), true);
        }

        /// <summary>
        /// Tests the create with parameters mapping 1.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping1()
        {
            "~/QuestionSet/Create".ShouldMapTo<QuestionSetController>(a => a.Create(null, null, null, null), true);
        }

        /// <summary>
        /// Tests the create with parameters mapping 2.
        /// </summary>
        [TestMethod]
        public void TestCreateWithParametersMapping2()
        {
            "~/QuestionSet/Create".ShouldMapTo<QuestionSetController>(a => a.Create(null, null, new QuestionSet(), string.Empty, true, false), true);
        }

        /// <summary>
        /// Tests the link to item type with parameters mapping 1.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeWithParametersMapping1()
        {
            "~/QuestionSet/LinkToItemType/5".ShouldMapTo<QuestionSetController>(a => a.LinkToItemType(5, true, false), true);
        }

        /// <summary>
        /// Tests the link to item type with parameters mapping 2.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemTypeWithParametersMapping2()
        {
            "~/QuestionSet/LinkToItemType/5".ShouldMapTo<QuestionSetController>(a => a.LinkToItemType(5, 1, true, false), true);
        }

        /// <summary>
        /// Tests the link to item with parameters mapping 1.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemWithParametersMapping1()
        {
            "~/QuestionSet/LinkToItem/5".ShouldMapTo<QuestionSetController>(a => a.LinkToItem(5, true, false), true);
        }

        /// <summary>
        /// Tests the link to item with parameters mapping 2.
        /// </summary>
        [TestMethod]
        public void TestLinkToItemWithParametersMapping2()
        {
            "~/QuestionSet/LinkToItem/5".ShouldMapTo<QuestionSetController>(a => a.LinkToItem(5, 1, true, false), true);
        }

        /// <summary>
        /// Tests the unlink from item mapping.
        /// </summary>
        [TestMethod]
        public void TestUnlinkFromItemMapping()
        {
            "~/QuestionSet/UnlinkFromItem/5".ShouldMapTo<QuestionSetController>(a => a.UnlinkFromItem(5));
        }
        #endregion Route Tests
    }
}

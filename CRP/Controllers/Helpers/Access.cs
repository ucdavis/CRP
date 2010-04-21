using System;
using System.Linq;
using System.Security.Principal;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Check=UCDArch.Core.Utils.Check;

namespace CRP.Controllers.Helpers
{
    public class Access
    {
        /// <summary>
        /// Determines access to a question set
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="currentUser"></param>
        /// <param name="questionSet"></param>
        /// <returns></returns>
        public static bool HasQuestionSetAccess(IRepository repository, IPrincipal currentUser, QuestionSet questionSet)
        {
            var user = repository.OfType<User>().Queryable.Where(a => a.LoginID == currentUser.Identity.Name).FirstOrDefault();

            Check.Require(repository != null, "Repository is required.");
            Check.Require(user != null, "User is required.");

            if (questionSet.SystemReusable)
            {
                // system reusable only admins can modify these
                if (currentUser.IsInRole(RoleNames.Admin))
                {
                    return true;
                }
            }
            else if (questionSet.CollegeReusable)
            {
                // college reusable only school admin in the college can modify these
                var schools = user.Units.Select(a => a.School).ToList();

                if (currentUser.IsInRole(RoleNames.SchoolAdmin) 
                        && schools.Contains(questionSet.School))
                {
                    return true;
                }
            }
            else if (questionSet.UserReusable)
            {
                // user reusable matching user has access
                if (user == questionSet.User)
                {
                    return true;
                }
            }
            else
            {
                // not reusable access should be based on either item type or item access
                // if it's associated with an item type, then you can edit the question set with the item type
                if (questionSet.ItemTypes.Count > 0)
                {
                    if (currentUser.IsInRole(RoleNames.Admin))
                    {
                        return true;   
                    }
                }
                // if there is no item type, if someone has access to an item they have access to edit this question sets
                else
                {
                    var item = questionSet.Items.FirstOrDefault();

                    // the item cannot be null, if we are here.  The question set was created specifically for the item
                    Check.Require(item != null);

                    return HasItemAccess(repository, currentUser, item.Item);
                }
            }

            return false;
        }

        public static bool HasItemAccess(IRepository repository, IPrincipal currentUser, Item item)
        {
            //TODO: edit access to the item

            return true;
        }
    }
}

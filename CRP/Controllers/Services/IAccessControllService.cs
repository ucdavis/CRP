using System.Security.Principal;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;

namespace CRP.Controllers.Services
{
    public interface IAccessControllService
    {
        bool HasItemAccess(IPrincipal currentUser, Item item);
    }

    public class AccessControllService : IAccessControllService
    {
        public bool HasItemAccess(IPrincipal currentUser, Item item)
        {
            return Access.HasItemAccess(currentUser, item);
        }
    }
}

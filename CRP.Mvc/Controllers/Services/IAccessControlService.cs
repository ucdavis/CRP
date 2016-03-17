using System.Security.Principal;
using CRP.Controllers.Helpers;
using CRP.Core.Domain;

namespace CRP.Controllers.Services
{
    public interface IAccessControlService
    {
        bool HasItemAccess(IPrincipal currentUser, Item item);
    }

    public class AccessControlService : IAccessControlService
    {
        public bool HasItemAccess(IPrincipal currentUser, Item item)
        {
            return Access.HasItemAccess(currentUser, item);
        }
    }
}

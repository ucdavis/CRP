using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;

namespace CRP.Services.Wcf
{
    public class ServiceAuthorizationValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (!RepositoryFactory.ApplicationKeyRepository.Queryable.Where(a => a.Application == userName && a.Key == password).Any())
            {
                throw new SecurityTokenException(string.Format("Password token does not grant access to {0}", userName));
            }
        }
    }
}
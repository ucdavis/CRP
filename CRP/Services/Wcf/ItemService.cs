using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRP.Services.Wcf
{
    public class ItemService : IItemService
    {
        public string CreateCoupon(string email, DateTime expiration, int maxQuantity, int maxUsage)
        {
            throw new NotImplementedException();
        }

        public bool CancelCoupon(string couponCode)
        {
            throw new NotImplementedException();
        }

        public ServiceRegistration GetRegistration(string registrationId)
        {
            throw new NotImplementedException();
        }

        public ServiceRegistration[] GetRegistrations(int itemId)
        {
            throw new NotImplementedException();
        }
    }
}
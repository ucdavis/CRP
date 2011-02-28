using System;
using System.ServiceModel;

namespace CRP.Services.Wcf
{
    [ServiceContract]
    public interface IItemService
    {
        [OperationContract]
        string CreateCoupon(string email, DateTime expiration, int maxQuantity, int maxUsage);

        [OperationContract]
        bool CancelCoupon(string couponCode);

        [OperationContract]
        ServiceRegistration GetRegistration(string registrationId);

        [OperationContract]
        ServiceRegistration[] GetRegistrations(int itemId);
    }

    public class ServiceRegistration
    {
    }

    public class ServiceQuestions
    {
    }
}

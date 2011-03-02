using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CRP.Services.Wcf
{
    [ServiceContract]
    public interface IItemService
    {
        [OperationContract]
        string CreateCoupon(int itemId, string email, bool unlimited, DateTime? expiration, decimal discountAmount, int? maxUsage, int? maxQuantity);

        [OperationContract]
        bool CancelCoupon(string couponCode);

        [OperationContract]
        ServiceTransaction GetRegistration(string registrationId);

        [OperationContract]
        ServiceTransaction GetRegistration(int transactionId);

        [OperationContract]
        ServiceTransaction[] GetRegistrations(int itemId);
    }

    public class ServiceTransaction
    {
        // extract the fields that we can
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime DateRegistered { get; set; }
        public bool Paid { get; set; }

        public IEnumerable<ServiceQuestion> ServiceQuestions { get; set; }
    }

    public class ServiceQuestion
    {
        public ServiceQuestion(string question, string answer, Guid? quantityIndex = null)
        {
            Question = question;
            Answer = answer;
            QuantityIndex = quantityIndex;
        }

        public string Question { get; set; }
        public string Answer { get; set; }
        public Guid? QuantityIndex { get; set; }
    }
}

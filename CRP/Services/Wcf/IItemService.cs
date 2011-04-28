﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using CRP.Core.Domain;

namespace CRP.Services.Wcf
{
    [ServiceContract]
    public interface IItemService
    {
        [OperationContract]
        string CreateCoupon(int itemId, string email, DateTime? expiration, decimal discountAmount, int? maxUsage, int? maxQuantity, CouponTypes couponTypes);

        [OperationContract]
        bool CancelCoupon(int itemId, string couponCode);

        [OperationContract]
        ServiceTransaction GetRegistrationByReference(int itemId, string referenceId);

        [OperationContract]
        ServiceTransaction[] GetRegistrations(int itemId);
    }

    [DataContract]
    public class ServiceTransaction
    {
        [DataMember]
        public string ReferenceId { get; set; }

        // extract the fields that we can
        [DataMember]
        public string TransactionNumber { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public DateTime DateRegistered { get; set; }
        [DataMember]
        public bool Paid { get; set; }
        [DataMember]
        public ICollection<ServiceQuestion> ServiceQuestions { get; set; }
    }

    [DataContract]
    public class ServiceQuestion
    {
        public ServiceQuestion(string question, string answer, Guid? quantityIndex = null)
        {
            Question = question;
            Answer = answer;
            QuantityIndex = quantityIndex;
        }

        [DataMember]
        public string Question { get; set; }
        [DataMember]
        public string Answer { get; set; }
        [DataMember]
        public Guid? QuantityIndex { get; set; }
    }

    [DataContract]
    public enum CouponTypes
    {   [EnumMember]
        Unlimited, 
        [EnumMember]
        LimitedUsage, 
        [EnumMember]
        SingleUsage
    };
}

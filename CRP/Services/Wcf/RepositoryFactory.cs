using CRP.Core.Domain;
using Microsoft.Practices.ServiceLocation;
using UCDArch.Core.PersistanceSupport;

namespace CRP.Services.Wcf
{
    public class RepositoryFactory
    {
        // Private constructor prevents instantiation from other classes
        private RepositoryFactory() { }

        private static class CouponSingletonHolder
        {
            public static readonly IRepository<Coupon> Instance = ServiceLocator.Current.GetInstance<IRepository<Coupon>>();
        }

        private static class ItemSingletonHolder
        {
            public static readonly IRepository<Item> Instance = ServiceLocator.Current.GetInstance<IRepository<Item>>();
        }

        private static class TransactionAnswerSingletonHolder
        {
            public static readonly IRepository<TransactionAnswer> Instance = ServiceLocator.Current.GetInstance<IRepository<TransactionAnswer>>();
        }

        private static class TransactionSingletonHolder
        {
            public static readonly IRepository<Transaction> Instance = ServiceLocator.Current.GetInstance<IRepository<Transaction>>();
        }

        private static class ApplicationKeySingletonHolder
        {
            public static readonly IRepository<ApplicationKey> Instance = ServiceLocator.Current.GetInstance<IRepository<ApplicationKey>>();
        }

        public static IRepository<Coupon> CouponRepository
        {
            get { return CouponSingletonHolder.Instance; }
        }

        public static IRepository<Item> ItemRepository
        {
            get { return ItemSingletonHolder.Instance; }
        }

        public static IRepository<TransactionAnswer> TransactionAnswerRepository
        {
            get { return TransactionAnswerSingletonHolder.Instance; }
        }

        public static IRepository<Transaction> TransactionRepository
        {
            get { return TransactionSingletonHolder.Instance; }
        }

        public static IRepository<ApplicationKey> ApplicationKeyRepository
        {
            get { return ApplicationKeySingletonHolder.Instance; }
        }
    }
}
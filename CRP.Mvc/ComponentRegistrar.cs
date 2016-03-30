using Castle.Windsor;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Castle.MicroKernel.Registration;
using CRP.Controllers.Services;
using CRP.Core.Abstractions;
using CRP.Models;
using CRP.Services;

namespace CRP.Mvc
{
    internal static class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);

            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            container.Register(Component.For<IDbContext>().ImplementedBy<DbContext>().Named("dbContext"));

            container.Register(Component.For<ISearchTermProvider>().ImplementedBy<SearchTermProvider>().Named("searchProvider"));
            container.Register(Component.For<INotificationProvider>().ImplementedBy<NotificationProvider>().Named("NotificationProvider"));
            container.Register(Component.For<IChartProvider>().ImplementedBy<ChartProvider>().Named("ChartProvider"));
            container.Register(Component.For<IAccessControlService>().ImplementedBy<AccessControlService>().Named("AccessControlService"));
            container.Register(Component.For<ICouponService>().ImplementedBy<CouponService>().Named("CouponService"));
            container.Register(Component.For<ICopyItemService>().ImplementedBy<CopyItemService>().Named("CopyItemService"));
        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.Register(Component.For(typeof(IRepositoryWithTypedId<,>)).ImplementedBy(typeof(RepositoryWithTypedId<,>)).Named("repositoryWithTypedId"));
            container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).Named("repositoryType"));
            container.Register(Component.For<IRepository>().ImplementedBy<Repository>().Named("repository"));
        }
    }
}
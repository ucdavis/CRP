using System.Collections.Generic;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using NHibernate;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using System.Linq;

namespace CRP.Models
{
    public class SearchTermProvider : ISearchTermProvider
    {
        private readonly IRepository<Item> _itemRepository;

        public SearchTermProvider(IRepository<Item> itemRepository )
        {
            _itemRepository = itemRepository;
        }

        public IEnumerable<Item> GetByTerm(string term)
        {
            if(string.IsNullOrEmpty(term) || term.Trim() == string.Empty)
            {
                return _itemRepository.GetAll().Where(a => a.Private == false && a.Available);
            }
            IQuery query = NHibernateSessionManager.Instance.GetSession().GetNamedQuery("ItemsSearch");
            query.SetString("query", term);

            return query.List<Item>();
        }
    }
}
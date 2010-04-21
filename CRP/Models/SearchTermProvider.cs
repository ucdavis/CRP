using System.Collections.Generic;
using CRP.Core.Abstractions;
using CRP.Core.Domain;
using NHibernate;
using UCDArch.Data.NHibernate;

namespace CRP.Models
{
    public class SearchTermProvider : ISearchTermProvider
    {
        public IEnumerable<Item> GetByTerm(string term)
        {
            IQuery query = NHibernateSessionManager.Instance.GetSession().GetNamedQuery("ItemsSearch");
            query.SetString("query", term);

            return query.List<Item>();
        }
    }
}
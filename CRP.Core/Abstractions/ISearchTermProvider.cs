using System;
using System.Collections.Generic;
using CRP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace CRP.Core.Abstractions
{
    public interface ISearchTermProvider
    {
        IEnumerable<Item> GetByTerm(string term);
    }

    public class DevSearchTermProvider : ISearchTermProvider
    {
        private readonly IRepository<Item> _repository;

        public DevSearchTermProvider(IRepository<Item> repository)
        {
            _repository = repository;
        }

        public IEnumerable<Item> GetByTerm(string term)
        {
            return _repository.GetAll();
        }
    }
}
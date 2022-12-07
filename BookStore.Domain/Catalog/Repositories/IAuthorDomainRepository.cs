namespace BookStore.Domain.Catalog.Repositories;

using Common;
using Models.Authors;
using System.Threading;
using System.Threading.Tasks;

public interface IAuthorDomainRepository : IDomainRepository<Author> {
    Task<Author?> Find(int id, CancellationToken cancellationToken = default);

    Task<Author?> Find(string name, CancellationToken cancellationToken = default);

    Task<bool> Delete(int id, CancellationToken cancellationToken = default);
}
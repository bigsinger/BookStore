﻿namespace BookStore.Infrastructure.Sales.Repositories;

using Application.Sales.Customers;
using Application.Sales.Customers.Queries.Common;
using Application.Sales.Customers.Queries.Details;
using AutoMapper;
using Common.Events;
using Common.Repositories;
using Domain.Common;
using Domain.Sales.Exceptions;
using Domain.Sales.Models.Customers;
using Domain.Sales.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

internal class CustomerRepository : DataRepository<ISalesDbContext, Customer>,
    ICustomerDomainRepository,
    ICustomerQueryRepository {
    public CustomerRepository(
        ISalesDbContext db,
        IMapper mapper,
        IEventDispatcher eventDispatcher)
        : base(db, mapper, eventDispatcher) {
    }

    public async Task<Customer> FindByUser(
        string userId,
        CancellationToken cancellationToken = default)
        => await this.Find(
            userId,
            customer => customer,
            cancellationToken);

    public async Task<int> GetCustomerId(
        string userId,
        CancellationToken cancellationToken = default)
        => await this.Find(
            userId,
            customer => customer.Id,
            cancellationToken);

    public async Task<CustomerDetailsResponseModel?> Details(
        int id,
        CancellationToken cancellationToken = default)
        => await this.Mapper
            .ProjectTo<CustomerDetailsResponseModel>(this
                .AllAsNoTracking()
                .Where(c => c.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<int> Total(
        Specification<Customer> specification,
        CancellationToken cancellationToken = default)
        => await this
            .GetCustomersQuery(specification)
            .CountAsync(cancellationToken);

    public async Task<IEnumerable<CustomerResponseModel>> GetCustomersListing(
        Specification<Customer> specification,
        int skip = 0,
        int take = int.MaxValue,
        CancellationToken cancellationToken = default)
        => await this.Mapper
            .ProjectTo<CustomerResponseModel>(this
                .GetCustomersQuery(specification)
                .OrderBy(c => c.Id)
                .Skip(skip)
                .Take(take))
            .ToListAsync(cancellationToken);

    private async Task<T> Find<T>(
        string userId,
        Expression<Func<Customer, T>> selector,
        CancellationToken cancellationToken = default) {
        var customer = await this
            .All()
            .Where(u => u.UserId == userId)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);

        if (EqualityComparer<T>.Default.Equals(customer, default)) {
            throw new InvalidCustomerException("This user is not a customer.");
        }

        return customer!;
    }

    private IQueryable<Customer> GetCustomersQuery(
        Specification<Customer> specification)
        => this
            .AllAsNoTracking()
            .Where(specification);
}
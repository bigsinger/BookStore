﻿namespace BookStore.Infrastructure;

using Application;
using Application.Common.Contracts;
using Catalog;
using Common.Events;
using Common.Extensions;
using Common.Persistence;
using Domain;
using Domain.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sales;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

public class InfrastructureConfigurationSpecs {
    [Fact]
    public void AddRepositoriesShouldRegisterRepositories() {
        var serviceCollection = new ServiceCollection()
            .AddDbContext<BookStoreDbContext>(options => options
                .UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddScoped<ICatalogDbContext>(provider => provider
                .GetService<BookStoreDbContext>()!)
            .AddScoped<ISalesDbContext>(provider => provider
                .GetService<BookStoreDbContext>()!);

        var assembly = Assembly.GetExecutingAssembly();

        var services = serviceCollection
            .AddRepositories()
            .AddAutoMapper(assembly)
            .AddTransient<IEventDispatcher, EventDispatcher>()
            .BuildServiceProvider();

        AssertRepositoriesAreNotNull(
            services,
            typeof(DomainConfiguration),
            typeof(IDomainRepository<>));

        AssertRepositoriesAreNotNull(
            services,
            typeof(ApplicationConfiguration),
            typeof(IQueryRepository<>));
    }

    private static void AssertRepositoriesAreNotNull(
        IServiceProvider services,
        Type configurationType,
        Type repositoryType)
        => configurationType
            .Assembly
            .GetTypes()
            .Where(t => t
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == repositoryType))
            .ForEach(repository => services
                .GetService(repository)
                .Should()
                .NotBeNull());
}
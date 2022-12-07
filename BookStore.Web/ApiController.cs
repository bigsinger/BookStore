﻿namespace BookStore.Web;

using Application.Common.Models;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public abstract class ApiController : ControllerBase {
    protected const string Id = "{id}";
    protected const string PathSeparator = "/";

    private IMediator? mediator;

    protected IMediator Mediator
        => this.mediator ??= this.HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected Task<ActionResult<TResult>> Send<TResult>(IRequest<TResult> request)
        => this.Mediator.Send(request).ToActionResult();

    protected Task<ActionResult<TResult>> Send<TResult>(IRequest<Result<TResult>> request)
        => this.Mediator.Send(request).ToActionResult();

    protected Task<ActionResult> Send(IRequest<Result> request)
        => this.Mediator.Send(request).ToActionResult();
}
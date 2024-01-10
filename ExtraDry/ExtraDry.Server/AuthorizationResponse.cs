﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;

namespace ExtraDry.Server;

public class AuthorizationResponse {

    private readonly RequestDelegate next;
    private readonly ExtraDryOptions options;

    public AuthorizationResponse(RequestDelegate next, IOptions<ExtraDryOptions> options)
    {
        this.next = next;
        this.options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        switch(context.Response.StatusCode) {
            case (int)HttpStatusCode.Forbidden:
                ProblemDetailsResponse.RewriteResponse(context, HttpStatusCode.Forbidden, options.ForbiddenTitle, options.ForbiddenMessage);
                break;
            case (int)HttpStatusCode.Unauthorized:
                ProblemDetailsResponse.RewriteResponse(context, HttpStatusCode.Unauthorized, options.UnauthorizedTitle, options.UnauthorizedMessage);
                break;
        }
    }
}

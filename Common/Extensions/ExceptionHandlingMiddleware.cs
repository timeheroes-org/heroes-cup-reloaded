﻿using HeroesCup.Web.Common.Middlewares.Exceptions;

namespace HeroesCup.Web.Common.Extensions;

public static class ExceptionHandlingMiddleware
{
    public static IApplicationBuilder UseUnhandledExceptionLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LogUnhandledExceptionMiddleware>();
    }
}
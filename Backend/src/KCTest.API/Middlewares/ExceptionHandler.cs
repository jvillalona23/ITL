﻿using KCTest.Domain.Common;
using KCTest.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KCTest.API.Middlewares
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)GetStatusCode(exception);

            var errorMessage = exception.InnerException?.Message ?? exception.Message;

            await context.Response.WriteAsync(errorMessage);
        }

        public HttpStatusCode GetStatusCode(Exception exception)
        {
            var internalException = exception as BaseException;
            if (internalException == null)
            {
                return HttpStatusCode.InternalServerError;
            }
            return internalException.StatusCode;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebFlsQuiz.Interfaces;

namespace WebFlsQuiz.Models
{
    public static class OperationResult
    {
        public class SuccessResult : IOperationResult
        { }
        public class SuccessResult<T> : SuccessResult, IOperationResult<T>
        {
            public T Value { get; private set; }
            private SuccessResult() { }
            public SuccessResult(T value) { Value = value; }
        }
        public class UserErrorResult : IOperationResult
        {
            public string ErrorMessage { get; private set; }
            protected UserErrorResult() { }
            public UserErrorResult(string message) { ErrorMessage = message; }
        }
        public class UserErrorResult<T> : UserErrorResult, IOperationResult<T>
        {
            public UserErrorResult(string message) : base(message) { }
        }
        public class FailureResult : IOperationResult
        {
            public Exception Exception { get; private set; }
            protected FailureResult() { }
            public FailureResult(Exception ex) { Exception = ex; }
        }
        public class FailureResult<T> : FailureResult, IOperationResult<T>
        {
            public FailureResult(Exception ex) : base(ex) { }
        }
        public static IOperationResult<T> Success<T>(T value)
        {
            return new SuccessResult<T>(value);
        }
        public static IOperationResult Success()
        {
            return new SuccessResult();
        }
        public static IOperationResult<T> UserError<T>(string message)
        {
            return new UserErrorResult<T>(message);
        }
        public static IOperationResult UserError(string message)
        {
            return new UserErrorResult(message);
        }
        public static IOperationResult<T> Failure<T>(Exception ex)
        {
            return new FailureResult<T>(ex);
        }
        public static IOperationResult Failure(Exception ex)
        {
            return new FailureResult(ex);
        }
        public static IOperationResult<T> Try<T>(Func<IOperationResult<T>> f)
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                return OperationResult.Failure<T>(e);
            }
        }
        public static IOperationResult Try(Func<IOperationResult> f)
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                return OperationResult.Failure(e);
            }
        }
        public static IOperationResult<T> ToResult<T>(this T value)
        {
            return OperationResult.Success(value);
        }
        public static IOperationResult<T> WithLogging<T>(this IOperationResult<T> source, ILogger logger)
        {
            switch (source)
            {
                case OperationResult.FailureResult<T> failure:
                    logger.LogCritical(failure.Exception.Message);
                    return source;
                default: return source;
            }
        }
        public static IOperationResult WithLogging(this IOperationResult source, ILogger logger)
        {
            switch (source)
            {
                case OperationResult.FailureResult failure:
                    logger.LogCritical(failure.Exception.Message);
                    return source;
                default: return source;
            }
        }
        public static IActionResult ToApiResult<T>(this IOperationResult<T> source)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult<T> userError: return new BadRequestObjectResult(userError.ErrorMessage);
                case OperationResult.FailureResult<T> failure: return new StatusCodeResult(500);
                case OperationResult.SuccessResult<T> success: return new OkObjectResult(success.Value);
                default: return new StatusCodeResult(500);
            }
        }
        public static IActionResult ToApiResult(this IOperationResult source)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult userError: return new BadRequestObjectResult(userError.ErrorMessage);
                case OperationResult.FailureResult failure: return new StatusCodeResult(500);
                case OperationResult.SuccessResult success: return new OkResult();
                default: return new StatusCodeResult(500);
            }
        }
        public static IActionResult ToViewResult<T>(this IOperationResult<T> source, Controller controller, string viewName)
        {
            switch (source)
            {
                case OperationResult.SuccessResult<T> success:
                    controller.ViewData.Model = success.Value;
                    return new ViewResult()
                    {
                        ViewName = viewName,
                        ViewData = controller.ViewData,
                        TempData = controller.TempData
                    };
                default:
                    return new ViewResult()
                    {
                        ViewName = viewName,
                        ViewData = controller.ViewData,
                        TempData = controller.TempData
                    };
            }
        }
        public static IOperationResult<T2> Bind<T1, T2>(this IOperationResult<T1> source, Func<T1, IOperationResult<T2>> f)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult<T1> userError: return OperationResult.UserError<T2>(userError.ErrorMessage);
                case OperationResult.FailureResult<T1> failure: return OperationResult.Failure<T2>(failure.Exception);
                case OperationResult.SuccessResult<T1> success: return f(success.Value);
                default: return OperationResult.Failure<T2>(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static IOperationResult Bind<T>(this IOperationResult<T> source, Func<T, IOperationResult> f)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult<T> userError: return OperationResult.UserError(userError.ErrorMessage);
                case OperationResult.FailureResult<T> failure: return OperationResult.Failure(failure.Exception);
                case OperationResult.SuccessResult<T> success: return f(success.Value);
                default: return OperationResult.Failure(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static IOperationResult<T> Bind<T>(this IOperationResult source, Func<IOperationResult<T>> f)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult userError: return OperationResult.UserError<T>(userError.ErrorMessage);
                case OperationResult.FailureResult failure: return OperationResult.Failure<T>(failure.Exception);
                case OperationResult.SuccessResult success: return f();
                default: return OperationResult.Failure<T>(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static async Task<IOperationResult<T2>> Bind<T1, T2>(this Task<IOperationResult<T1>> source, Func<T1, IOperationResult<T2>> f)
        {
            switch (await source)
            {
                case OperationResult.UserErrorResult<T1> userError: return OperationResult.UserError<T2>(userError.ErrorMessage);
                case OperationResult.FailureResult<T1> failure: return OperationResult.Failure<T2>(failure.Exception);
                case OperationResult.SuccessResult<T1> success: return f(success.Value);
                default: return OperationResult.Failure<T2>(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static async Task<IOperationResult<T2>> Bind<T1, T2>(this Task<IOperationResult<T1>> source, Func<T1, Task<IOperationResult<T2>>> f)
        {
            switch (await source)
            {
                case OperationResult.UserErrorResult<T1> userError: return OperationResult.UserError<T2>(userError.ErrorMessage);
                case OperationResult.FailureResult<T1> failure: return OperationResult.Failure<T2>(failure.Exception);
                case OperationResult.SuccessResult<T1> success: return await f(success.Value);
                default: return OperationResult.Failure<T2>(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static async Task<IOperationResult<T2>> BindAsync<T1, T2>(this IOperationResult<T1> source, Func<T1, Task<IOperationResult<T2>>> f)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult<T1> userError: return OperationResult.UserError<T2>(userError.ErrorMessage);
                case OperationResult.FailureResult<T1> failure: return OperationResult.Failure<T2>(failure.Exception);
                case OperationResult.SuccessResult<T1> success: return await f(success.Value);
                default: return OperationResult.Failure<T2>(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static async Task<IOperationResult<TResult>> Merge<T1, T2, TResult>(this IOperationResult<T1> source, Func<Task<IOperationResult<T2>>> getAddition, Func<T1, T2, IOperationResult<TResult>> f)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult<T1> userError: return OperationResult.UserError<TResult>(userError.ErrorMessage);
                case OperationResult.FailureResult<T1> failure: return OperationResult.Failure<TResult>(failure.Exception);
                case OperationResult.SuccessResult<T1> successSource:
                    switch (await getAddition())
                    {
                        case OperationResult.UserErrorResult<T2> userError: return OperationResult.UserError<TResult>(userError.ErrorMessage);
                        case OperationResult.FailureResult<T2> failure: return OperationResult.Failure<TResult>(failure.Exception);
                        case OperationResult.SuccessResult<T2> successAddition: return f(successSource.Value, successAddition.Value);
                        default: return OperationResult.Failure<TResult>(new ArgumentException("Unsupported operation result type."));
                    }
                default: return OperationResult.Failure<TResult>(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static IOperationResult<TResult> Merge<T1, T2, TResult>(this IOperationResult<T1> source, Func<IOperationResult<T2>> getAddition, Func<T1, T2, IOperationResult<TResult>> f)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult<T1> userError: return OperationResult.UserError<TResult>(userError.ErrorMessage);
                case OperationResult.FailureResult<T1> failure: return OperationResult.Failure<TResult>(failure.Exception);
                case OperationResult.SuccessResult<T1> successSource:
                    switch (getAddition())
                    {
                        case OperationResult.UserErrorResult<T2> userError: return OperationResult.UserError<TResult>(userError.ErrorMessage);
                        case OperationResult.FailureResult<T2> failure: return OperationResult.Failure<TResult>(failure.Exception);
                        case OperationResult.SuccessResult<T2> successAddition: return f(successSource.Value, successAddition.Value);
                        default: return OperationResult.Failure<TResult>(new ArgumentException("Unsupported operation result type."));
                    }
                default: return OperationResult.Failure<TResult>(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static IOperationResult Merge(this IOperationResult source, Func<IOperationResult> getAddition)
        {
            switch (source)
            {
                case OperationResult.UserErrorResult userError: return userError;
                case OperationResult.FailureResult failure: return failure;
                case OperationResult.SuccessResult successSource:
                    switch (getAddition())
                    {
                        case OperationResult.UserErrorResult userError: return userError;
                        case OperationResult.FailureResult failure: return failure;
                        case OperationResult.SuccessResult successAddition: return successAddition;
                        default: return OperationResult.Failure(new ArgumentException("Unsupported operation result type."));
                    }
                default: return OperationResult.Failure(new ArgumentException("Unsupported operation result type."));
            }
        }
        public static IOperationResult<T[]> All<T>(IEnumerable<Func<IOperationResult<T>>> source)
        {
            var result = OperationResult.Success(new List<T>());
            foreach (var func in source)
            {
                result = result.Merge(func, (a, n) =>
                {
                    a.Add(n);
                    return OperationResult.Success(a);
                });
            }
            return result.Bind(x => OperationResult.Success(x.ToArray()));
        }
        public static IOperationResult All(IEnumerable<Func<IOperationResult>> source)
        {
            var result = OperationResult.Success();
            foreach (var func in source)
            {
                result = result.Merge(func);
            }
            return result;
        }
    }
}
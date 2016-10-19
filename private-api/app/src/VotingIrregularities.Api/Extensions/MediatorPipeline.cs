using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Serilog.Context;

namespace VotingIrregularities.Api.Extensions
{
    public class MediatorPipeline<TRequest, TResponse>
      : IRequestHandler<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IMessageAuthorizer _authorizer;
        private readonly IMessageCache _cache;
        private readonly IEnumerable<IMessageValidator<TRequest>> _validators;

        public MediatorPipeline(
                    IRequestHandler<TRequest, TResponse> inner,
                    IEnumerable<IMessageValidator<TRequest>> validator,
                    IMessageAuthorizer authorizer,
                    IMessageCache cache)
        {
            _inner = inner;
            _validators = validator;
            _authorizer = authorizer;
            _cache = cache;
        }

        public TResponse Handle(TRequest message)
        {
            using (LogContext.PushProperty("MediatRRequestType", typeof(TRequest).FullName))
            using (Metrics.Time("MediatRRequest"))
            {
                var failures = _validators
                .SelectMany(v => v.Validate(message))
                .Select(a => a.ErrorMessage)
                .Where(f => f != null)
                .ToList();

                if (failures.Any())
                    throw new ValidationException(string.Join(", ", failures));

                return _inner.Handle(message);
            }
        }
    }

    public interface IMessageValidator<in T>
    {
        IQueryable<ValidationFailure> Validate(T message);
    }

    public interface IMessageAuthorizer
    {
        void Evaluate<TRequest>(TRequest request) where TRequest : class;
    }
    

    public interface IMessageCache
    {
        void Evaluate<TRequest, TResponse>(TRequest request, Func<TResponse> response) where TRequest : class;
    }
}

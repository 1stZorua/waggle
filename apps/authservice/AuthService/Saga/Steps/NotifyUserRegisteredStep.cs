using AutoMapper;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Logging;
using Waggle.AuthService.Saga.Context;
using Waggle.Common.Constants;
using Waggle.Common.Messaging;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.Contracts.Auth.Events;

namespace Waggle.AuthService.Saga.Steps
{
    public class NotifyUserRegisteredStep : ISagaStep<RegistrationSagaContext, RegisterResponseDto>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;
        private readonly ILogger<NotifyUserRegisteredStep> _logger;

        public NotifyUserRegisteredStep(
            IEventPublisher eventPublisher,
            IMapper mapper,
            ILogger<NotifyUserRegisteredStep> logger)
        {
            _eventPublisher = eventPublisher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<RegisterResponseDto>> ExecuteAsync(RegistrationSagaContext context)
        {
            try
            {
                var completedEvent = _mapper.Map<RegistrationCompletedEvent>(context);
                await _eventPublisher.PublishAsync(completedEvent);

                var response = _mapper.Map<RegisterResponseDto>(context);
                return Result<RegisterResponseDto>.Ok(response);
            } 
            catch (Exception ex)
            {
                _logger.LogRegistrationCompletedEventPublishFailed(ex, context.Username, context.Id);
                return Result<RegisterResponseDto>.Fail(AuthErrors.User.ProfileInitFailed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<RegisterResponseDto>> CompensateAsync(RegistrationSagaContext context)
        {
            await Task.Yield();
            return Result<RegisterResponseDto>.Fail(AuthErrors.User.ProfileInitFailed, ErrorCodes.ServiceFailed);
        }
    }
}

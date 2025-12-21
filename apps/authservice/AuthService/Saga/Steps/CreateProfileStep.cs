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
    public class CreateProfileStep : ISagaStep<RegistrationSagaContext, RegisterResponseDto>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProfileStep> _logger;

        public CreateProfileStep(
            IEventPublisher eventPublisher,
            IMapper mapper,
            ILogger<CreateProfileStep> logger)
        {
            _eventPublisher = eventPublisher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<RegisterResponseDto>> ExecuteAsync(RegistrationSagaContext context)
        {
            try
            {
                var registeredEvent = _mapper.Map<RegisteredEvent>(context);
                await _eventPublisher.PublishAsync(registeredEvent);

                var response = _mapper.Map<RegisterResponseDto>(context);
                return Result<RegisterResponseDto>.Ok(response);
            } 
            catch (Exception ex)
            {
                _logger.LogRegisteredEventPublishFailed(ex, context.Username, context.Id);
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

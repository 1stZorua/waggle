using AutoMapper;
using MassTransit;
using UserService.Dtos;
using UserService.Services;
using Waggle.Contracts.Auth.Events;

namespace UserService.Consumers
{
    public class RegisteredEventConsumer : IConsumer<RegisteredEvent>
    {
        private readonly ILogger<RegisteredEventConsumer> _logger;
        private readonly IUserService _userService;
        private IMapper _mapper;

        public RegisteredEventConsumer(ILogger<RegisteredEventConsumer> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<RegisteredEvent> context)
        {
            var registeredEvent = context.Message;

            _logger.LogInformation(
                "Received RegisteredEvent: UserId = {UserId}, Email = {Email}",
                registeredEvent.UserId,
                registeredEvent.Email
            );

            var user = _mapper.Map<UserCreateDto>(registeredEvent);
            await _userService.CreateUserFromEventAsync(user);
        }
    }
}

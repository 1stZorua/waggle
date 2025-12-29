using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.PostService.Constants;
using Waggle.PostService.Data;
using Waggle.PostService.Logging;
using Waggle.PostService.Saga.Context;
using Waggle.PostService.Services;

namespace Waggle.PostService.Saga.Steps
{
    public class DeletePostStep : ISagaStep<DeletionSagaContext>
    {
        private readonly IPostRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<DeletePostStep> _logger;

        public DeletePostStep(IPostRepository repo, IMapper mapper, ILogger<DeletePostStep> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> ExecuteAsync(DeletionSagaContext context)
        {
            var currentUser = context.CurrentUser;

            if (!currentUser.TryEnsure(out var failedResult))
                return failedResult;

            try
            {
                var post = await _repo.GetPostByIdAsync(context.Id);
                if (post == null)
                {
                    _logger.LogPostDeleteNotFound(context.Id);
                    return Result.Fail(PostErrors.Post.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(post.UserId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), context.Id);
                    return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                await _repo.DeletePostAsync(post);

                _logger.LogPostDeleted(context.Id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogPostDeletionFailed(ex, context.Id);
                return Result.Fail(PostErrors.Post.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogPostDeletionFailed(ex, context.Id);
                return Result.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> CompensateAsync(DeletionSagaContext context)
        {
            await Task.Yield();
            return Result.Ok();
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.CommentService.Constants;
using Waggle.CommentService.Data;
using Waggle.CommentService.Logging;
using Waggle.CommentService.Saga.Context;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;

namespace Waggle.CommentService.Saga.Steps
{
    public class DeleteCommentStep : ISagaStep<DeletionSagaContext>
    {
        private readonly ICommentRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteCommentStep> _logger;

        public DeleteCommentStep(ICommentRepository repo, IMapper mapper, ILogger<DeleteCommentStep> logger)
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
                var comment = await _repo.GetCommentByIdAsync(context.Id);
                if (comment == null)
                {
                    _logger.LogCommentDeleteNotFound(context.Id);
                    return Result.Fail(CommentErrors.Comment.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(comment.UserId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), context.Id);
                    return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                var allReplyIds = await _repo.GetAllReplyIdsRecursivelyAsync(context.Id);

                var allCommentIds = new List<Guid> { context.Id };
                allCommentIds.AddRange(allReplyIds);

                await _repo.DeleteCommentsByIdsAsync(allCommentIds);

                _logger.LogCommentDeleted(context.Id);

                context.DeletedCommentIds = allCommentIds;

                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCommentDeletionFailed(ex, context.Id);
                return Result.Fail(CommentErrors.Comment.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogCommentDeletionFailed(ex, context.Id);
                return Result.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> CompensateAsync(DeletionSagaContext context)
        {
            await Task.Yield();
            return Result.Ok();
        }
    }
}
import * as commentApi from './generated/index';

export const CommentClient = {
  getAll: commentApi.getApiComment,
  getById: commentApi.getApiCommentId,
  getAllByUserId: commentApi.getApiCommentUsersUserId,
  getAllByPostId: commentApi.getApiCommentPostsPostId,
  getReplies: commentApi.getApiCommentCommentIdReplies,
  getPostCount: commentApi.getApiCommentPostsPostIdCount,
  getReplyCount: commentApi.getApiCommentCommentIdRepliesCount,
  create: commentApi.postApiComment,
  update: commentApi.putApiCommentId,
  delete: commentApi.deleteApiCommentId
};

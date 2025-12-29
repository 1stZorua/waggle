import * as likeApi from './generated/index';

export const LikeClient = {
  getAll: likeApi.getApiLike,
  getById: likeApi.getApiLikeId,
  getAllByUserId: likeApi.getApiLikeUsersUserId,
  getAllByTargetId: likeApi.getApiLikeTargetTargetId,
  getTargetCount: likeApi.getApiLikeTargetTargetIdCount,
  hasLiked: likeApi.getApiLikeCheckUserIdTargetId,
  create: likeApi.postApiLike,
  delete: likeApi.deleteApiLikeId
};

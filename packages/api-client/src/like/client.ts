import * as likeApi from './generated/index';

export const LikeClient = {
  getAll: likeApi.getApiLike,
  getById: likeApi.getApiLikeId,
  getAllByUserId: likeApi.getApiLikeUsersUserId,
  getAllByTargetId: likeApi.getApiLikeTargetTargetId,
  hasLiked: likeApi.getApiLikeCheckUserIdTargetId,
  create: likeApi.postApiLike,
  delete: likeApi.deleteApiLikeId
};

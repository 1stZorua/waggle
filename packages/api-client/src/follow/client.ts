import * as followApi from './generated/index';

export const FollowClient = {
  getAll: followApi.getApiFollow,
  getById: followApi.getApiFollowId,
  getFollowing: followApi.getApiFollowFollowingUserId,
  getFollowers: followApi.getApiFollowFollowersUserId,
  getFollowingCount: followApi.getApiFollowFollowingUserIdCount,
  getFollowersCount: followApi.getApiFollowFollowersUserIdCount,
  isFollowing: followApi.getApiFollowCheckFollowerIdFollowingId,
  create: followApi.postApiFollow,
  delete: followApi.deleteApiFollowId
};

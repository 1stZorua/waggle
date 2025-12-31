import * as favoriteApi from './generated/index';

export const FavoriteClient = {
  getAll: favoriteApi.getApiFavorite,
  getById: favoriteApi.getApiFavoriteId,
  getAllByUserId: favoriteApi.getApiFavoriteUserUserId,
  getAllByTargetId: favoriteApi.getApiFavoriteTargetTargetId,
  hasFavorited: favoriteApi.getApiFavoriteCheckUserIdTargetId,
  create: favoriteApi.postApiFavorite,
  delete: favoriteApi.deleteApiFavoriteId
};

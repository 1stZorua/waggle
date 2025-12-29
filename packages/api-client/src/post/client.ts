import * as postApi from './generated/index';

export const PostClient = {
  getAll: postApi.getApiPost,
  getById: postApi.getApiPostId,
  getAllByUserId: postApi.getApiPostUsersUserIdPosts,
  create: postApi.postApiPost,
  delete: postApi.deleteApiPostId
};

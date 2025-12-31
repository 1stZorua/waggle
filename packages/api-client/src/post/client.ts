import * as postApi from './generated/index';

export const PostClient = {
  getAll: postApi.getApiPost,
  getById: postApi.getApiPostId,
  getAllByUserId: postApi.getApiPostUsersUserIdPosts,
  create: postApi.postApiPost,
  update: postApi.putApiPostId,
  delete: postApi.deleteApiPostId
};

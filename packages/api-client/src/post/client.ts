import * as postApi from './generated/index';

export const PostClient = {
  getAll: postApi.getApiPost,
  getById: postApi.getApiPostId,
  create: postApi.postApiPost,
  delete: postApi.deleteApiPostId
};

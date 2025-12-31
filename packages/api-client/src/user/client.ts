import * as userApi from './generated/index';

export const UserClient = {
  getAll: userApi.getApiUsers,
  getById: userApi.getApiUsersId,
  getByIds: userApi.postApiUsersBatch,
  create: userApi.postApiUsers,
  update: userApi.putApiUsersId,
  delete: userApi.deleteApiUsersId
};

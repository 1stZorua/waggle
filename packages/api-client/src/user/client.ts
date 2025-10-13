import * as userApi from './generated/index';

export const UserClient = {
  getUsers: userApi.getApiUsers,
  createUser: userApi.postApiUsers,
  getUserById: userApi.getApiUsersId
};

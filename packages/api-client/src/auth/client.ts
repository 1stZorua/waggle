import * as authApi from './generated/index';

export const AuthClient = {
  login: authApi.postApiAuthLogin,
  refresh: authApi.postApiAuthRefresh,
  logout: authApi.postApiAuthLogout,
  validate: authApi.getApiAuthValidate
};

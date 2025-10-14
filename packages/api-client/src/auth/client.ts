import * as authApi from './generated/index';

export const AuthClient = {
  login: authApi.postApiAuthLogin,
  register: authApi.postApiAuthRegister,
  refresh: authApi.postApiAuthRefresh,
  logout: authApi.postApiAuthLogout,
  validate: authApi.getApiAuthValidate
};

import * as mediaApi from './generated/index';

export const MediaClient = {
  getAll: mediaApi.getApiMedia,
  getById: mediaApi.getApiMediaId,
  getUrl: mediaApi.getApiMediaIdUrl,
  create: mediaApi.postApiMedia,
  delete: mediaApi.deleteApiMediaId
};

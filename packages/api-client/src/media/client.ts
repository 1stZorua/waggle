import * as mediaApi from './generated/index';

export const MediaClient = {
  getAll: mediaApi.getApiMedia,
  getById: mediaApi.getApiMediaId,
  getBatch: mediaApi.postApiMediaBatch,
  getUrl: mediaApi.getApiMediaIdUrl,
  getUrls: mediaApi.postApiMediaBatchUrls,
  create: mediaApi.postApiMedia,
  createBatch: mediaApi.postApiMediaBatchUpload,
  delete: mediaApi.deleteApiMediaId
};

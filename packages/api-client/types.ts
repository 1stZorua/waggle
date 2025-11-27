export interface ApiErrorResponse {
  status: 'fail' | 'error';
  message: string;
  code?: string;
  data?: unknown;
}

export interface ApiError extends Error {
  status: number;
  message: string;
  body?: ApiErrorResponse;
}

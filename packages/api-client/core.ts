import type { ApiError, ApiErrorResponse } from './types';

const getBaseUrl = (url: string): string | undefined => {
  if (url.startsWith('http')) return undefined;

  const service = url.match(/^\/api\/(\w+)\/?/)?.[1]?.toUpperCase();
  if (!service) return undefined;

  const envVar =
    typeof process !== 'undefined' && process.env
      ? process.env[`${service}_SERVICE_BASE_URL`]
      : undefined;

  if (!envVar) {
    throw new Error(`Base URL for ${service} service is not configured`);
  }

  return envVar;
};

export const isApiError = (err: unknown): err is ApiError =>
  typeof err === 'object' && err !== null && 'status' in err && 'message' in err;

const convertToFormData = (data: Record<string, any>): FormData => {
  const formData = new FormData();

  for (const [key, value] of Object.entries(data)) {
    if (value === undefined || value === null) continue;

    if (Array.isArray(value)) {
      value.forEach((item) => formData.append(key, item));
    } else {
      formData.append(key, value);
    }
  }

  return formData;
};

export const apiFetch = async <T>(url: string, options: RequestInit = {}): Promise<T> => {
  const baseUrl = getBaseUrl(url);
  const fullUrl = baseUrl ? `${baseUrl.replace(/\/$/, '')}${url}` : url;

  const headers: Record<string, string> = {
    ...((options.headers as Record<string, string>) || {})
  };

  const shouldConvertToFormData = headers['X-Convert-To-FormData'] === 'true';
  delete headers['X-Convert-To-FormData'];

  let body = options.body;

  if (shouldConvertToFormData && body && typeof body === 'object' && !(body instanceof FormData)) {
    body = convertToFormData(body as Record<string, any>);
  }

  const isJsonBody =
    body &&
    typeof body === 'object' &&
    !(body instanceof FormData) &&
    !(body instanceof Blob) &&
    !(body instanceof ArrayBuffer) &&
    !(body instanceof URLSearchParams) &&
    typeof body !== 'string';

  if (isJsonBody && !headers['Content-Type']) {
    headers['Content-Type'] = 'application/json';
  }

  const finalBody = isJsonBody ? JSON.stringify(body) : body;

  const res = await fetch(fullUrl, {
    ...options,
    headers,
    body: finalBody
  });

  const text = await res.text().catch(() => '');
  const responseBody = text ? JSON.parse(text) : {};

  if (!res.ok) {
    throw {
      status: res.status,
      message: res.statusText || 'Request failed',
      body: responseBody as ApiErrorResponse
    } as ApiError;
  }

  return { data: responseBody, status: res.status, headers: res.headers } as unknown as T;
};

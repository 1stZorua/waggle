import type { ApiError, ApiErrorResponse } from './types';

const getBaseUrl = (url: string): string | undefined => {
  if (url.startsWith('http')) return undefined;

  const service = url.match(/^\/api\/(\w+)\//)?.[1]?.toUpperCase();
  if (!service) return undefined;

  const envVar = process.env[`${service}_SERVICE_BASE_URL`];
  if (!envVar) throw new Error(`Environment variable ${service}_SERVICE_BASE_URL is not set`);

  return envVar;
};

export const isApiError = (err: unknown): err is ApiError => {
  return typeof err === 'object' && err !== null && 'status' in err && 'message' in err;
};

export const apiFetch = async <T>(url: string, options: RequestInit = {}): Promise<T> => {
  const baseUrl = getBaseUrl(url);
  const fullUrl = baseUrl ? `${baseUrl.replace(/\/$/, '')}${url}` : url;

  const res = await fetch(fullUrl, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers || {})
    }
  });

  const text = await res.text().catch(() => '');
  const body = text ? JSON.parse(text) : {};

  if (!res.ok) {
    throw {
      status: res.status,
      message: res.statusText || 'Request failed',
      body: body as ApiErrorResponse
    } as ApiError;
  }

  return {
    data: body,
    status: res.status,
    headers: res.headers
  } as unknown as T;
};

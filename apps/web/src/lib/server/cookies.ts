import { COOKIE_OPTIONS, MAX_AGE } from '$lib/config';
import type { Cookies } from '@sveltejs/kit';

export const setAuthCookies = (
	cookies: Cookies,
	accessToken: string,
	refreshToken: string,
	expiresIn: number
) => {
	const expiresAt = Date.now() + expiresIn * 1000;

	cookies.set('access_token', accessToken, { ...COOKIE_OPTIONS, maxAge: expiresIn });
	cookies.set('refresh_token', refreshToken, { ...COOKIE_OPTIONS, maxAge: MAX_AGE });
	cookies.set('expires_at', expiresAt.toString(), { ...COOKIE_OPTIONS, maxAge: MAX_AGE });
};

export const deleteAuthCookies = (cookies: Cookies) => {
	cookies.delete('access_token', COOKIE_OPTIONS);
	cookies.delete('refresh_token', COOKIE_OPTIONS);
	cookies.delete('expires_at', COOKIE_OPTIONS);
};

export const getAuthCookies = (cookies: Cookies) => {
	const accessToken = cookies.get('access_token');
	const refreshToken = cookies.get('refresh_token');
	const expiresAtStr = cookies.get('expires_at');

	if (!accessToken || !refreshToken || !expiresAtStr) return null;

	const expiresAt = parseInt(expiresAtStr, 10);
	if (isNaN(expiresAt)) return null;

	return { accessToken, refreshToken, expiresAt };
};

import { COOKIE_OPTIONS, MAX_AGE } from '$lib/config';
import type { Cookies } from '@sveltejs/kit';

export const setAuthCookies = (
	cookies: Cookies,
	accessToken: string,
	refreshToken: string,
	expiresIn: number
) => {
	cookies.set('access_token', accessToken, { ...COOKIE_OPTIONS, maxAge: expiresIn });
	cookies.set('refresh_token', refreshToken, { ...COOKIE_OPTIONS, maxAge: MAX_AGE });
};

export const deleteAuthCookies = (cookies: Cookies) => {
	cookies.delete('access_token', COOKIE_OPTIONS);
	cookies.delete('refresh_token', COOKIE_OPTIONS);
};

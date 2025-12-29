import { deleteAuthCookies, getAuthCookies, setAuthCookies } from '$lib/server';
import { AuthClient } from '@waggle/api-client/auth';
import type { Cookies } from '@sveltejs/kit';
import type { User } from '$lib/types/types';

export function authHeaders(accessToken: string) {
	return { Authorization: `Bearer ${accessToken}` };
}

export async function refreshTokenClient(cookies: Cookies, refreshToken?: string) {
	if (!refreshToken) return null;

	try {
		const { data } = await AuthClient.refresh({ refreshToken }).then((res) => res.data);
		if (!data?.accessToken || !data?.refreshToken) {
			deleteAuthCookies(cookies);
			return null;
		}

		setAuthCookies(cookies, data.accessToken, data.refreshToken, data.expiresIn);
		return getAuthCookies(cookies);
	} catch (err) {
		console.error('Token refresh failed:', err);
		deleteAuthCookies(cookies);
		return null;
	}
}

export async function validateUser(accessToken: string): Promise<User | null> {
	try {
		const { data } = await AuthClient.validate({
			headers: authHeaders(accessToken)
		}).then((res) => res.data);

		if (!data?.sub || !data.preferred_username || !data.email || !data.name) return null;

		return {
			id: data.sub,
			username: data.preferred_username,
			email: data.email,
			name: data.name,
			roles: data.realm_access?.roles ?? []
		};
	} catch (err) {
		console.error('Token validation failed:', err);
		return null;
	}
}

export function shouldRefresh(auth: ReturnType<typeof getAuthCookies>) {
	if (!auth?.refreshToken) return false;
	if (!auth.accessToken) return true;

	const BUFFER_TIME_MS = 60_000;
	return Date.now() + BUFFER_TIME_MS >= auth.expiresAt;
}

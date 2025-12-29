import type { Handle } from '@sveltejs/kit';
import { getAuthCookies } from '$lib/server';
import { refreshTokenClient, validateUser, shouldRefresh } from '$lib/server/auth';

export const handle: Handle = async ({ event, resolve }) => {
	let auth = getAuthCookies(event.cookies);

	if (!auth?.accessToken && !auth?.refreshToken) return resolve(event);

	if (shouldRefresh(auth)) {
		const refreshed = await refreshTokenClient(event.cookies, auth.refreshToken);
		if (!refreshed) return resolve(event);
		auth = refreshed;
	}

	event.locals.auth = auth;

	const user = await validateUser(auth.accessToken);
	if (!user) return resolve(event);

	event.locals.user = user;

	return resolve(event);
};

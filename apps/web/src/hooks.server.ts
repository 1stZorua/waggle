import { getAuthCookies, setAuthCookies } from '$lib/server';
import type { Cookies, Handle } from '@sveltejs/kit';
import { AuthClient } from '@waggle/api-client/auth';

export const handle: Handle = async ({ event, resolve }) => {
	let auth = getAuthCookies(event.cookies);

	if (!auth?.accessToken && auth?.refreshToken)
		auth = await refreshToken(event.cookies, auth.refreshToken);

	if (auth) {
		event.locals.auth = auth;
		console.log('Auth cookies:', auth);
		const user = await validateUser(auth.accessToken);
		console.log('Validated user:', user);
		if (user) event.locals.user = user;
	}

	return await resolve(event);
};

async function refreshToken(cookies: Cookies, refreshToken: string) {
	try {
		const { data } = await AuthClient.refresh({ refreshToken }).then((res) => res.data);

		if (!data?.accessToken || !data?.refreshToken) return null;

		setAuthCookies(cookies, data.accessToken, data.refreshToken, data.expiresIn);
		return getAuthCookies(cookies);
	} catch (err) {
		console.error(err);
		console.log(err);
		return null;
	}
}

export async function validateUser(accessToken: string) {
	try {
		const { data, status, code, message } = await AuthClient.validate({
			headers: { Authorization: `Bearer ${accessToken}` }
		}).then((res) => res.data);

		console.log(status);
		console.log(code);
		console.log(message);
		console.log(data);

		if (
			!data?.sub ||
			!data.preferred_username ||
			!data.email ||
			!data.name ||
			!data.realm_access?.roles
		)
			return null;

		return {
			id: data.sub,
			username: data.preferred_username,
			email: data.email,
			name: data.name,
			roles: data.realm_access.roles
		};
	} catch (err) {
		console.error(err);
		console.log(err);
		return null;
	}
}

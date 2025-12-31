import { json } from '@sveltejs/kit';
import { getAuthCookies } from '$lib/server';
import { refreshTokenClient } from '$lib/server';
import { redirect } from 'sveltekit-flash-message/server';

export const POST = async ({ cookies }) => {
	const auth = getAuthCookies(cookies);

	if (!auth?.refreshToken) {
		return json({ success: false }, { status: 401 });
	}

	const refreshed = await refreshTokenClient(cookies, auth.refreshToken);
	if (!refreshed) return json({ success: false }, { status: 401 });

	return json({ success: true });
};

export const GET = () => {
	throw redirect(302, '/');
};

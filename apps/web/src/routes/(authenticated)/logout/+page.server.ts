import { authHeaders, deleteAuthCookies } from '$lib/server';
import { AuthClient } from '@waggle/api-client/auth';
import type { Actions, PageServerLoad } from './$types';
import { redirect } from 'sveltekit-flash-message/server';
import { handleAction } from '$lib/server';

export const load: PageServerLoad = async () => {
	throw redirect(302, '/');
};

export const actions: Actions = {
	default: async ({ cookies, locals }) => {
		await handleAction(async () => {
			await AuthClient.logout(
				{ refreshToken: locals.auth.refreshToken },
				{ headers: authHeaders(locals.auth.accessToken) }
			);
		}, cookies);

		deleteAuthCookies(cookies);
		throw redirect('/login', { type: 'success', message: 'Successfully logged out' }, cookies);
	}
};

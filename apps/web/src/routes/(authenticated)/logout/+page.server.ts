import { deleteAuthCookies } from '$lib/server';
import { AuthClient } from '@waggle/api-client/auth';
import type { Actions, PageServerLoad } from './$types';
import { redirect } from 'sveltekit-flash-message/server';

export const load: PageServerLoad = async () => {
	throw redirect(302, '/');
};

export const actions: Actions = {
	default: async ({ cookies, locals }) => {
		try {
			await AuthClient.logout(
				{
					refreshToken: locals.auth.refreshToken
				},
				{ headers: { Authorization: `Bearer ${locals.auth.accessToken}` } }
			);
		} catch (err) {
			console.error(err);
		}

		deleteAuthCookies(cookies);
		throw redirect('/login', { type: 'success', message: 'Successfully logged out' }, cookies);
	}
};

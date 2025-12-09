import { setAuthCookies } from '$lib/server';
import { json } from '@sveltejs/kit';
import { isApiError } from '@waggle/api-client';
import { AuthClient } from '@waggle/api-client/auth';

export async function GET({ cookies }) {
	const refreshToken = cookies.get('refresh_token');

	if (!refreshToken)
		return json({ ok: false, status: 'Unauthorized', message: 'No refresh token provided' });

	try {
		const { data, status, message } = await AuthClient.refresh({ refreshToken }).then(
			(res) => res.data
		);

		if (!data?.accessToken || !data?.refreshToken) return json({ ok: false, status, message });

		setAuthCookies(cookies, data.accessToken, data.refreshToken, data.expiresIn);
		return json({ status, message });
	} catch (err) {
		let message = 'Login service is temporarily unavailable';
		let status = 'Unauthorized';
		if (isApiError(err)) {
			message = err.body?.message ?? message;
			status = err.body?.status ?? status;
		}

		console.error(err);
		return json({ ok: false, status, message });
	}
}

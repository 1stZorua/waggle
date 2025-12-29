import { fail } from '@sveltejs/kit';
import { setFlash } from 'sveltekit-flash-message/server';
import { isApiError } from '@waggle/api-client';
import type { Cookies } from '@sveltejs/kit';

function handleError(err: unknown, cookies: Cookies) {
	let message = 'Service is temporarily unavailable';
	if (isApiError(err)) message = err.body?.message ?? message;

	console.error(err);
	setFlash({ type: 'error', message }, cookies);
	return message;
}

export async function handleFormAction<T>(
	form: { valid: boolean; data: T },
	handler: (data: T) => Promise<void>,
	cookies: Cookies
) {
	if (!form.valid) return fail(400, { form });

	try {
		await handler(form.data);
		return null;
	} catch (err) {
		const message = handleError(err, cookies);
		return fail(400, { form, message });
	}
}

export async function handleAction(handler: () => Promise<void>, cookies: Cookies) {
	try {
		await handler();
		return null;
	} catch (err) {
		const message = handleError(err, cookies);
		return fail(500, { message });
	}
}

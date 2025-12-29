import type { RequestHandler } from './$types';
import { like, unlike } from '$lib/server/likes';
import { json } from '@sveltejs/kit';

export const POST: RequestHandler = async ({ locals, request }) => {
	const { targetId, targetType } = await request.json();
	const result = await like(locals.auth.accessToken, targetId, targetType);
	return json(result);
};

export const DELETE: RequestHandler = async ({ locals, request }) => {
	const { id } = await request.json();
	const result = await unlike(locals.auth.accessToken, id);
	return json(result);
};

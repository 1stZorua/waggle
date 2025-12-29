import type { RequestHandler } from './$types';
import { json } from '@sveltejs/kit';
import { favorite, unfavorite } from '$lib/server';

export const POST: RequestHandler = async ({ locals, request }) => {
	const { targetId, targetType } = await request.json();
	const result = await favorite(locals.auth.accessToken, targetId, targetType);
	return json(result);
};

export const DELETE: RequestHandler = async ({ locals, request }) => {
	const { id } = await request.json();
	const result = await unfavorite(locals.auth.accessToken, id);
	return json(result);
};

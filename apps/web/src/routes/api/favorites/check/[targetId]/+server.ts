import type { RequestHandler } from './$types';
import { hasFavorited } from '$lib/server';
import { json } from '@sveltejs/kit';

export const GET: RequestHandler = async ({ params, locals }) => {
	const result = await hasFavorited(locals.auth.accessToken, locals.user.id, params.targetId).then(
		(l) => l.data
	);
	return json(result);
};

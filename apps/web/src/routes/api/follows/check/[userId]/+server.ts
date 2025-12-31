import type { RequestHandler } from './$types';
import { isFollowing } from '$lib/server';
import { json } from '@sveltejs/kit';

export const GET: RequestHandler = async ({ params, locals }) => {
	const result = await isFollowing(locals.auth.accessToken, locals.user.id, params.userId);
	return json(result);
};

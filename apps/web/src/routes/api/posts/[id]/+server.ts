import { getPostById } from '$lib/server';
import type { RequestHandler } from './$types';

export const GET: RequestHandler = async ({ locals, params }) => {
	const { id } = params;

	const result = await getPostById(locals.auth.accessToken, id);

	return new Response(JSON.stringify(result), {
		headers: { 'Content-Type': 'application/json' }
	});
};

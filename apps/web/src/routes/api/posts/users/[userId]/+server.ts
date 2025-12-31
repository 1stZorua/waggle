import { getPostsByUserId } from '$lib/server';
import type { RequestHandler } from './$types';

export const GET: RequestHandler = async ({ locals, url, params }) => {
	const { userId } = params;
	const cursor = url.searchParams.get('cursor') || undefined;

	const result = await getPostsByUserId(locals.auth.accessToken, userId, cursor);

	return new Response(JSON.stringify(result), {
		headers: { 'Content-Type': 'application/json' }
	});
};

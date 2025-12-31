import { getCommentsByPostId } from '$lib/server';
import type { RequestHandler } from './$types';

export const GET: RequestHandler = async ({ locals, url, params }) => {
	const { postId } = params;
	const cursor = url.searchParams.get('cursor') || undefined;

	const result = await getCommentsByPostId(locals.auth.accessToken, postId, cursor);

	return new Response(JSON.stringify(result), {
		headers: { 'Content-Type': 'application/json' }
	});
};

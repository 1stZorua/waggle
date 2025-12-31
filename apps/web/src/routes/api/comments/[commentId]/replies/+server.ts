import { getRepliesByCommentId } from '$lib/server';
import type { RequestHandler } from './$types';

export const GET: RequestHandler = async ({ locals, url, params }) => {
	const { commentId } = params;
	const cursor = url.searchParams.get('cursor') || undefined;

	const result = await getRepliesByCommentId(locals.auth.accessToken, commentId, cursor);

	return new Response(JSON.stringify(result), {
		headers: { 'Content-Type': 'application/json' }
	});
};

import type { RequestHandler } from './$types';
import { getPostsWithUsers } from '$lib/server';
import { redirect } from 'sveltekit-flash-message/server';

export const POST: RequestHandler = async ({ locals, request }) => {
	const { cursor } = await request.json();

	const result = await getPostsWithUsers(locals.auth.accessToken, cursor);

	return new Response(JSON.stringify(result), {
		headers: { 'Content-Type': 'application/json' }
	});
};

export const GET = () => {
	throw redirect(302, '/');
};

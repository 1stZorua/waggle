import type { RequestHandler } from './$types';
import { json } from '@sveltejs/kit';
import { follow, unfollow } from '$lib/server';
import { redirect } from 'sveltekit-flash-message/server';

export const POST: RequestHandler = async ({ locals, request }) => {
	const { followingId } = await request.json();
	const result = await follow(locals.auth.accessToken, followingId);
	return json(result);
};

export const DELETE: RequestHandler = async ({ locals, request }) => {
	const { id } = await request.json();
	const result = await unfollow(locals.auth.accessToken, id);
	return json(result);
};

export const GET = () => {
	throw redirect(302, '/');
};

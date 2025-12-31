import { createCommentSchema, updateCommentSchema } from '$lib/schemas';
import { type Actions } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import type { PageServerLoad } from './$types';
import { redirect, setFlash } from 'sveltekit-flash-message/server';
import { CommentClient } from '@waggle/api-client/comment';
import { authHeaders, handleFormAction } from '$lib/server';
import { deleteCommentSchema } from '$lib/schemas/deleteComment';

export const load: PageServerLoad = async () => {
	throw redirect(302, '/');
};

export const actions: Actions = {
	create: async ({ request, locals, cookies }) => {
		const form = await superValidate(request, zod4(createCommentSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				await CommentClient.create(
					{
						content: data.content,
						postId: data.postId,
						parentId: data.parentId
					},
					{ headers: authHeaders(locals.auth.accessToken) }
				);
			},
			cookies
		);

		if (error) return error;

		const message = form.data.parentId
			? 'Successfully created reply'
			: 'Successfully created comment';
		setFlash({ type: 'success', message }, cookies);

		return { form };
	},

	delete: async ({ request, locals, cookies }) => {
		const form = await superValidate(request, zod4(deleteCommentSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				await CommentClient.delete(data.id, {
					headers: authHeaders(locals.auth.accessToken)
				});
			},
			cookies
		);

		if (error) return error;
		setFlash({ type: 'success', message: 'Successfully deleted comment' }, cookies);

		return { form };
	},

	update: async ({ request, locals, cookies }) => {
		const form = await superValidate(request, zod4(updateCommentSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				await CommentClient.update(
					data.id,
					{
						postId: data.postId,
						content: data.content
					},
					{
						headers: authHeaders(locals.auth.accessToken)
					}
				);
			},
			cookies
		);

		if (error) return error;
		setFlash({ type: 'success', message: 'Successfully updated comment' }, cookies);

		return { form };
	}
};

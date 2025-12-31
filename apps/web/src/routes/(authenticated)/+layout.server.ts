import { redirect } from '@sveltejs/kit';
import type { LayoutServerLoad } from './$types';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import {
	createCommentSchema,
	createPostSchema,
	updateCommentSchema,
	deleteCommentSchema,
	deleteUserSchema,
	updateUserSchema
} from '$lib/schemas';
import { updatePostSchema } from '$lib/schemas/updatePost';
import { deletePostSchema } from '$lib/schemas/deletePost';
import { getUserById } from '$lib/server';

export const load: LayoutServerLoad = async ({ locals }) => {
	if (!locals.auth) throw redirect(302, '/login');

	const createPostForm = await superValidate(zod4(createPostSchema));
	const updatePostForm = await superValidate(zod4(updatePostSchema));
	const deletePostForm = await superValidate(zod4(deletePostSchema));

	const createCommentForm = await superValidate(zod4(createCommentSchema));
	const updateCommentForm = await superValidate(zod4(updateCommentSchema));
	const deleteCommentForm = await superValidate(zod4(deleteCommentSchema));

	const deleteUserForm = await superValidate(zod4(deleteUserSchema));
	const updateUserForm = await superValidate(zod4(updateUserSchema));

	return {
		user: locals.user,
		profile: await getUserById(locals.auth.accessToken, locals.user.id),
		createPostForm: createPostForm,
		updatePostForm: updatePostForm,
		deletePostForm: deletePostForm,
		createCommentForm: createCommentForm,
		updateCommentForm: updateCommentForm,
		deleteCommentForm: deleteCommentForm,
		deleteUserForm: deleteUserForm,
		updateUserForm: updateUserForm
	};
};

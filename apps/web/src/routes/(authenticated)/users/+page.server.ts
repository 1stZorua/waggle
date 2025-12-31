import { type Actions } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import type { PageServerLoad } from './$types';
import { redirect } from 'sveltekit-flash-message/server';
import { authHeaders, deleteAuthCookies, handleFormAction } from '$lib/server';
import { deleteUserSchema, updateUserSchema } from '$lib/schemas';
import { AuthClient } from '@waggle/api-client/auth';
import { MediaClient } from '@waggle/api-client/media';
import { UserClient } from '@waggle/api-client/user';
import { setFlash } from 'sveltekit-flash-message/server';

export const load: PageServerLoad = async () => {
	throw redirect(302, '/');
};

export const actions: Actions = {
	update: async ({ request, locals, cookies }) => {
		const form = await superValidate(request, zod4(updateUserSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				const { data: mediaData } = await MediaClient.create(
					{ BucketName: 'users', Prefix: 'avatar', File: data.avatar },
					{
						headers: {
							...authHeaders(locals.auth.accessToken),
							'X-Convert-To-FormData': 'true'
						}
					}
				).then((res) => res.data);

				const avatarId = mediaData?.id;

				if (!avatarId) {
					throw new Error('Failed to upload avatar');
				}

				await UserClient.update(
					data.id,
					{
						avatarId
					},
					{
						headers: authHeaders(locals.auth.accessToken)
					}
				);
			},
			cookies
		);

		if (error) return error;
		setFlash({ type: 'success', message: 'Successfully updated avatar' }, cookies);
	},

	delete: async ({ request, locals, cookies }) => {
		const form = await superValidate(request, zod4(deleteUserSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				await AuthClient.delete(
					{ id: data.id },
					{
						headers: authHeaders(locals.auth.accessToken)
					}
				);
			},
			cookies
		);

		if (error) return error;

		deleteAuthCookies(cookies);
		throw redirect('/login', { type: 'success', message: 'Successfully deleted account' }, cookies);
	}
};

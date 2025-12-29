import { createPostSchema } from '$lib/schemas';
import { type Actions } from '@sveltejs/kit';
import { superValidate } from 'sveltekit-superforms/server';
import { zod4 } from 'sveltekit-superforms/adapters';
import type { PageServerLoad } from './$types';
import { MediaClient } from '@waggle/api-client/media';
import { redirect } from 'sveltekit-flash-message/server';
import { PostClient } from '@waggle/api-client/post';
import { authHeaders, handleFormAction } from '$lib/server';

export const load: PageServerLoad = async () => {
	throw redirect(302, '/');
};

export const actions: Actions = {
	create: async ({ request, locals, cookies }) => {
		const form = await superValidate(request, zod4(createPostSchema));

		const error = await handleFormAction(
			form,
			async (data) => {
				const { data: mediaData } = await MediaClient.createBatch(
					{ BucketName: 'posts', Files: data.images },
					{
						headers: {
							...authHeaders(locals.auth.accessToken),
							'X-Convert-To-FormData': 'true'
						}
					}
				).then((res) => res.data);

				const fileIds =
					mediaData?.map((m) => m.id).filter((id): id is string => id !== undefined) ?? [];
				const [thumbnailId, ...mediaIds] = fileIds;

				await PostClient.create(
					{
						caption: data.caption,
						thumbnailId,
						mediaIds
					},
					{ headers: authHeaders(locals.auth.accessToken) }
				);
			},
			cookies
		);

		if (error) return error;

		throw redirect('/', { type: 'success', message: 'Successfully created post' }, cookies);
	}
};

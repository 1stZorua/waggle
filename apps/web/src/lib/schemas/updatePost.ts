import { MAX_CAPTION_LENGTH } from '$lib/utils';
import { z } from 'zod';

export const updatePostSchema = z.object({
	id: z.uuid('Invalid ID'),
	caption: z.string().min(1, 'Caption is required').max(MAX_CAPTION_LENGTH, 'Caption is too long')
});

export type UpdatePostSchema = typeof updatePostSchema;

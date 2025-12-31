import { MAX_COMMENT_LENGTH } from '$lib/utils';
import { z } from 'zod';

export const updateCommentSchema = z.object({
	id: z.uuid('Invalid ID'),
	postId: z.uuid('Invalid post ID'),
	content: z.string().min(1, 'Comment is required').max(MAX_COMMENT_LENGTH, 'Comment is too long')
});

export type UpdateCommentSchema = typeof updateCommentSchema;

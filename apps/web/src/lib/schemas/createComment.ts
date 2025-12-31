import { z } from 'zod';
import { MAX_COMMENT_LENGTH } from '$lib/utils/constants';

export const createCommentSchema = z.object({
	postId: z.uuid('Invalid post ID'),
	content: z.string().min(1, 'Comment is required').max(MAX_COMMENT_LENGTH, 'Comment is too long'),
	parentId: z.uuid('Invalid comment ID').nullable()
});

export type CreateCommentSchema = z.infer<typeof createCommentSchema>;

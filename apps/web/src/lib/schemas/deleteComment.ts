import { z } from 'zod';

export const deleteCommentSchema = z.object({
	id: z.uuid('Invalid ID')
});

export type DeleteCommentSchema = typeof deleteCommentSchema;

import { z } from 'zod';

export const deletePostSchema = z.object({
	id: z.uuid('Invalid ID')
});

export type DeletePostSchema = typeof deletePostSchema;

import { z } from 'zod';

export const updateUserSchema = z.object({
	id: z.uuid('Invalid ID'),
	avatar: z.instanceof(File, { message: 'Avatar is required' })
});

export type UpdateUserSchema = z.infer<typeof updateUserSchema>;

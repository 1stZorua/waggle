import { z } from 'zod';

export const deleteUserSchema = z.object({
	id: z.uuid('The provided ID is not valid')
});

export type DeleteUserSchema = typeof deleteUserSchema;

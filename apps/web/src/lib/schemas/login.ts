import { z } from 'zod';

export const loginSchema = z.object({
	identifier: z
		.string()
		.min(1, 'Enter your username or email')
		.refine(
			(val) => /^[a-zA-Z0-9]{3,25}$/.test(val) || /\S+@\S+\.\S+/.test(val),
			'Enter a valid username or email address'
		),
	password: z.string().min(1, 'Enter your password')
});

export type LoginSchema = typeof loginSchema;

import { z } from 'zod';

export const registerSchema = z
	.object({
		username: z
			.string()
			.min(3, 'Username must be at least 3 characters long')
			.max(20, 'Username cannot exceed 20 characters')
			.regex(/^[a-zA-Z0-9]+$/, 'Username can only contain letters and numbers'),
		email: z.email().min(1, 'Email is required'),
		firstName: z
			.string()
			.min(1, 'First name is required')
			.regex(/^[A-Za-z]+$/, 'First name can only contain letters'),
		lastName: z
			.string()
			.min(1, 'Last name is required')
			.regex(/^[A-Za-z]+$/, 'Last name can only contain letters'),
		password: z
			.string()
			.min(4, 'Password must be at least 4 characters long')
			.max(128, 'Password cannot exceed 128 characters'),
		confirmPassword: z.string().min(1, 'Confirm password is required')
	})
	.refine((data) => data.password === data.confirmPassword, {
		message: 'Passwords do not match',
		path: ['confirmPassword']
	});

export type RegisterSchema = typeof registerSchema;

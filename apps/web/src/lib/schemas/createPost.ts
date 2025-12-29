import { z } from 'zod';
import { MAX_CAPTION, MAX_IMAGES } from '$lib/utils/constants';

export const createPostSchema = z.object({
	images: z
		.array(z.instanceof(File))
		.min(1, 'Upload at least one image')
		.max(MAX_IMAGES, `Maximum of ${MAX_IMAGES} files allowed`),
	caption: z.string().min(1, 'Caption is required').max(MAX_CAPTION, 'Caption too long')
});

// export const createPostSchema = z.object({
// 	images: z
// 		.array(z.instanceof(File))
// 		.min(1, 'Upload at least one image')
// 		.max(MAX_IMAGES, `Maximum of ${MAX_IMAGES} files allowed`),
// 	mainPet: z.string().min(1, 'Main pet is required'),
// 	additionalPets: z.array(z.string()).optional(),
// 	caption: z.string().max(200, 'Caption too long'),
// 	tags: z.array(z.string()).optional()
// });

export type CreatePostSchema = z.infer<typeof createPostSchema>;

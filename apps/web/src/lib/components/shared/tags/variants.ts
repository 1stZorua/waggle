import { tv, type VariantProps } from 'tailwind-variants';

const tagVariants = tv({
	base: 'px-4 py-1 text-btn-primary-fg rounded-full border-3 flex items-center',
	variants: {
		variant: {
			error: 'bg-pink border-pink-light',
			info: 'bg-blue border-blue-light',
			success: 'bg-green border-green-light',
			warning: 'bg-yellow border-yellow-light'
		}
	},
	defaultVariants: {
		variant: 'info'
	}
});

type TagProps = VariantProps<typeof tagVariants>;

export { tagVariants, type TagProps };

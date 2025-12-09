import { tv, type VariantProps } from 'tailwind-variants';

const cardVariants = tv({
	base: 'flex gap-sm w-full h-max rounded-lg outline-none transition-colors duration-500 focus-visible:ring-2 focus-visible:ring-accent',
	variants: {
		variant: {
			primary: 'shadow-ui',
			secondary: ''
		},
		size: {
			none: 'p-0',
			sm: 'p-2',
			md: 'p-4',
			lg: 'p-6',
			xl: 'p-8'
		}
	},
	defaultVariants: {
		variant: 'primary',
		size: 'md'
	}
});

type CardProps = VariantProps<typeof cardVariants>;

export { cardVariants, type CardProps };

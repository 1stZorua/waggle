import { tv, type VariantProps } from 'tailwind-variants';

const buttonVariants = tv({
	base: 'flex items-center cursor-pointer w-full h-full outline-none transition-colors duration-500 focus-visible:ring-2 focus-visible:ring-accent disabled:cursor-default disabled:bg-btn-disabled-bg disabled:text-btn-disabled-fg',
	variants: {
		variant: {
			action: 'shadow-ui rounded-full h-max w-max',
			primary:
				'bg-btn-primary-bg text-btn-primary-fg rounded-full justify-center border-8 border-pink-light hover:shadow-xs hover:bg-btn-primary-hover-bg hover:border-btn-primary-hover-border',
			secondary:
				'bg-btn-action-bg text-btn-action-fg rounded-full shadow-ui justify-center hover:bg-btn-action-hover-bg',
			text: 'bg-btn-text-bg text-btn-text-fg w-max h-max focus-visible:ring-0'
		},
		size: {
			none: 'p-0',
			sm: 'px-4 py-2',
			md: 'px-6 py-3',
			lg: 'px-12 py-4',
			xl: 'px-12 py-5'
		}
	},
	defaultVariants: {
		variant: 'primary',
		size: 'md'
	}
});

type ButtonProps = VariantProps<typeof buttonVariants>;

export { buttonVariants, type ButtonProps };

import { tv, type VariantProps } from 'tailwind-variants';

const buttonVariants = tv({
	base: 'flex cursor-pointer w-full outline-none focus:ring-2 focus:ring-accent disabled:cursor-default disabled:bg-light-btn-disabled-bg',
	variants: {
		variant: {
			action: 'shadow-ui rounded-full h-max w-max',
			primary: 'bg-btn-primary-bg text-btn-primary-fg rounded-full justify-center',
			secondary: 'bg-btn-action-bg text-btn-action-fg rounded-full shadow-ui justify-center',
			text: 'bg-light-btn-text-bg text-light-btn-text-fg w-max focus:ring-0'
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

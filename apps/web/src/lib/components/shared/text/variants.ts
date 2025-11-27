import { tv, type VariantProps } from 'tailwind-variants';

const textVariants = tv({
	base: 'leading-tight',
	variants: {
		weight: {
			light: 'font-light',
			normal: 'font-normal',
			medium: 'font-medium',
			semibold: 'font-semibold',
			bold: 'font-bold',
			extrabold: 'font-extrabold'
		},
		size: {
			xs: 'text-xs',
			sm: 'text-sm',
			base: 'text-base',
			md: 'text-md',
			lg: 'text-lg',
			xl: 'text-xl'
		},
		family: {
			poppins: 'font-poppins'
		}
	},
	defaultVariants: {
		weight: 'medium',
		size: 'base',
		family: 'poppins'
	}
});

type TextProps = VariantProps<typeof textVariants>;

export { textVariants, type TextProps };

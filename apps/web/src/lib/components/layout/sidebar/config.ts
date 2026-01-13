import type { TagProps } from '$components/shared/tags/variants';

export interface SidebarItem {
	title: string;
	icon: string;
	href?: string;
	onClick?: () => void;
	tag?: {
		label: string;
		variant: TagProps['variant'];
	};
}

export const sidebarConfig: SidebarItem[] = [
	{
		title: 'Timeline',
		href: '/',
		icon: 'material-symbols:home-outline-rounded'
	},
	{
		title: 'Explore',
		href: '#explore',
		icon: 'mingcute:grid-line'
	},
	{
		title: 'Search',
		href: '#search',
		icon: 'uil:search'
	},
	{
		title: 'Collections',
		href: '#collections',
		icon: 'material-symbols:bookmark-outline-rounded',
		tag: {
			label: 'New',
			variant: 'info'
		}
	},
	{
		title: 'Create',
		icon: 'uil:camera'
	},
	{
		title: 'Profile',
		href: '/profile',
		icon: 'bx:user'
	}
];

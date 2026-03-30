# Waggle

Waggle is a social platform for pet owners to share content about their pets and connect with others.

## Figma Design

The design of Waggle prioritizes a clean, intuitive interface for sharing and exploring pet-related content. For detailed design elements and prototypes, please refer to the [Figma design](https://www.figma.com/design/oOiIaAz9eU59ygzbiIfJSV/Waggle?node-id=0-1&t=OzkLrQz79xTLNbGu-1).

## Architecture

Waggle is built as a Turborepo monorepo, enabling multiple applications and shared packages to coexist in a single, organized codebase.

## Tech Stack

- **Svelte** – Frontend framework  
- **Turborepo** – Monorepo architecture  
- **C#** – Backend services  
- **Docker** – Containerization  
- **Kubernetes** – Container orchestration  
- **Helmfile** – Kubernetes deployment management  

## Folder Structure

```bash
.
├── .github/workflows/   # CI/CD pipelines
├── apps/                # Applications (frontend, backend)
├── infra/               # Infrastructure (e.g., authentication setup)
├── packages/            # Shared libraries and components
├── tests/
│   └── performance/     # Performance and load testing
└── turbo.json           # Turborepo configuration
```

## Features

- **Pet-Centered Platform:** Share posts, images, and updates about pets  

- **Community Interaction:** Engage with other users through likes and comments  

- **Content Discovery:** Explore posts from other pet owners and discover new content  

- **User Profiles:** Create and manage a personal profile for you and your pets

- **User-Friendly Interface:** Clean and intuitive design for an enjoyable experience  

## Getting Started

```bash
npm install
npm run dev
```

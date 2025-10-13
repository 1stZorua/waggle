FROM node:22-alpine AS builder
WORKDIR /app

# Copy everything and build
COPY . .
RUN npm install && \
    npm install -g turbo@latest && \
    npx turbo run build --filter=web...

# Production dependencies stage
FROM node:22-alpine AS prod-deps
WORKDIR /app

# Copy package files (avoiding the packages/* pattern that caused issues)
COPY package*.json ./
COPY apps/web/package*.json ./apps/web/
COPY packages/ ./packages/

# Install only production dependencies
RUN npm install --production --ignore-scripts

FROM node:22-alpine AS runner
WORKDIR /app

# Create non-root user
RUN addgroup -g 1001 -S nodejs && \
    adduser -S sveltekit -u 1001 -G nodejs

# Copy built app
COPY --from=builder --chown=sveltekit:nodejs /app/apps/web/build ./build
COPY --from=builder --chown=sveltekit:nodejs /app/apps/web/package.json ./package.json

# Copy production node_modules (includes built internal packages)
COPY --from=prod-deps --chown=sveltekit:nodejs /app/node_modules ./node_modules

USER sveltekit

EXPOSE 3000
ENV NODE_ENV=production
ENV HOST=0.0.0.0
ENV PORT=3000

CMD ["node", "build"]
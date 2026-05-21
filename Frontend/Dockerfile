FROM node:22-alpine AS base
WORKDIR /app

FROM base AS deps
RUN corepack enable
COPY package.json ./
COPY pnpm-workspace.yaml ./
RUN pnpm config set minimum-release-age 0
RUN pnpm install

FROM base AS builder
RUN corepack enable
ENV CI=true
ENV PNPM_PUBLISH_TIME_CHECK_ENABLED=false
ENV NUXT_PUBLIC_API_BASE=http://localhost:5000
RUN pnpm config set minimum-release-age 0
COPY --from=deps /app/node_modules ./node_modules
COPY . .
RUN pnpm build

FROM base AS runner
ENV NODE_ENV=production
COPY --from=builder /app/.output ./.output
EXPOSE 3000
CMD ["node", ".output/server/index.mjs"]

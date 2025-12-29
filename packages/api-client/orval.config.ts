import { defineConfig } from 'orval';

const makeService = (name: string) =>
  ({
    input: {
      target: `./openapi/${name}.json`,
      override: {
        transformer: './scripts/remove-dto-transformer.js'
      }
    },
    output: {
      mode: 'split',
      target: `src/${name}/generated/index.ts`,
      schemas: `src/${name}/generated/model`,
      client: 'fetch',
      prettier: true,
      clean: true,
      override: {
        mutator: {
          path: './core.ts',
          name: 'apiFetch'
        }
      }
    }
  }) as const;

export default defineConfig({
  authservice: makeService('auth'),
  userservice: makeService('user'),
  postservice: makeService('post'),
  mediaservice: makeService('media'),
  likeservice: makeService('like'),
  followservice: makeService('follow'),
  favoriteservice: makeService('favorite'),
  commentService: makeService('comment')
});

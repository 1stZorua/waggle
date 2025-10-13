import { defineConfig } from 'orval';
import { execSync } from 'child_process';

type RouteInfo = {
  path: string;
  verb: string;
  operationId?: string;
  tags?: string[];
};

const makeService = (name: string, port: number) =>
  ({
    input: {
      target:
        process.env[`${name.toUpperCase()}_SERVICE_SPEC`] ||
        `http://localhost:${port}/swagger/v1/swagger.json`,
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
  authservice: makeService('auth', 8081),
  userservice: makeService('user', 8080)
});

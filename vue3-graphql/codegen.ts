
import type { CodegenConfig } from '@graphql-codegen/cli';

const config: CodegenConfig = {
  overwrite: true,
  schema: "http://.../graphql",
  documents: ["src/**/*.vue", "src/**/*.graphql"],
  generates: {
    "src/gql/codegen/": {
      preset: "client",
      config: {
        useTypeImports: true
      }
    }
  }
};

export default config;

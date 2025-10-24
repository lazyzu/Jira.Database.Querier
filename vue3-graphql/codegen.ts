
import type { CodegenConfig } from '@graphql-codegen/cli';

const config: CodegenConfig = {
  overwrite: true,
  schema: "http://pc-tu_tu.phison.com:8989/graphql",
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

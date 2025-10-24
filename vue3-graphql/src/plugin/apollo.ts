import { DefaultApolloClient } from "@vue/apollo-composable"
import type { App } from "vue";
import {
    ApolloClient,
    createHttpLink,
    InMemoryCache
} from "@apollo/client/core"

const httpLink = createHttpLink({
    uri: "http://pc-tu_tu.phison.com:8989/graphql"
});

const cache = new InMemoryCache();

const apolloClient = new ApolloClient({
    link: httpLink,
    cache
});

export const apolloPlugin = {
  install(app: App) {
    app.provide(DefaultApolloClient, apolloClient);
    console.log('Appolo Installed')
  }  
};
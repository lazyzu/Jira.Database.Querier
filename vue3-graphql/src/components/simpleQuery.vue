<template>
    <input v-model="issueKey" placeholder="issue key"></input> 
    <button v-on:click="query">Query</button>
    <div v-if="loading !== undefined">
        <p v-if="loading">Loading from Simple Query...</p>
        <div v-else>
            <div v-if="error">
                <p> Query is failed </p>
                <p> {{ error.name }}</p>
                <p> {{ error.message }}</p>
            </div>
            <div v-else>
                <div v-if="issueInfo">
                    <p>Id: {{ issueInfo.id }}</p>
                    <p>Summary: {{ issueInfo.summary }} </p>
                    <p>Priority: {{ issueInfo.priority?.name }}</p>
                    <p>Status: {{ issueInfo.issueStatus?.name }}</p>
                    <p>Assignee: {{ issueInfo.assignee?.username }}</p>
                    <p>Reporter: {{ issueInfo.reporter?.username }}</p>
                </div>
                <p v-else>Issue is not found</p>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">

import { ref } from "vue"
import gql from "graphql-tag"
import { type ApolloError } from "@apollo/client"
import { useApolloClient } from "@vue/apollo-composable"
import { type SimpleQuery_IssueQueryQuery as IssueQueryResponseType } from "../gql/codegen/graphql"

const { resolveClient } = useApolloClient()
const appoloClient = resolveClient();

const issueKey = ref('');

const loading = ref<boolean>();
type issueInfoType = IssueQueryResponseType['issue'];
const issueInfo = ref<issueInfoType>();
const error = ref<Error>();

const issueQuery = gql`query SimpleQuery_IssueQuery($key: String)
{
    issue(key: $key)
    {
        id,
        summary,
        priority
        {
            name
        },
        issueStatus
        {
            name
        },
        assignee
        {
            username,
        },
        reporter
        {
            username,
        }
    }
}`

const query = async () => {
    if(Boolean(issueKey.value) == false) return;

    try {
        loading.value = true;

        const response = await appoloClient.query({
            query: issueQuery,
            variables: {
                key: issueKey.value
            },
            fetchPolicy: 'network-only'
        });

        error.value = response.error;
        issueInfo.value = response.data.issue;
    }
    catch(ex) {
        if (ex instanceof Error) {
            error.value = ex;
            issueInfo.value = undefined;
        }
    }
    finally {
        loading.value = false;
    }
}

</script>
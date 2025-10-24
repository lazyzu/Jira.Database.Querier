<template>
    <input v-model="issueKey" placeholder="issue key"></input> 
    <button v-on:click="query">Query</button>
    <div v-if="loading !== undefined">
        <p v-if="loading">Loading from Fragment Query...</p>
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
                    <div v-if="issueInfo.assignee">
                        <p>Assignee: {{ issueInfo.assignee.username }}</p>
                        <userChildFragment :user="issueInfo.assignee"></userChildFragment>
                    </div>
                    <p v-else>Assignee: Unknown</p>
                    <div v-if="issueInfo.reporter">
                        <p>Reporter: {{ issueInfo.reporter.username }}</p>
                        <userChildFragment :user="issueInfo.reporter"></userChildFragment>
                    </div>
                    <p v-else>Reporter: Unknown</p>
                </div>
                <p v-else>Issue is not found</p>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">

import { ref } from "vue"
import { useApolloClient } from "@vue/apollo-composable"
import { graphql } from "../../gql/codegen"
import { type FragmentParentQuery_IssueQueryQuery as IssueQueryResponseType } from "../../gql/codegen/graphql"
import userChildFragment from "./fragmentChild.vue"

const { resolveClient } = useApolloClient();
const apolloClient = resolveClient();

const issueKey = ref('');

const loading = ref<boolean>();
type IssueInfoType = IssueQueryResponseType['issue'];
const issueInfo = ref<IssueInfoType>();
const error = ref<Error>();

const query = async () => {
    if(Boolean(issueKey.value) == false) return;

    try {
        loading.value = true;

        const response = await apolloClient.query({
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
        if(ex instanceof Error) {
            error.value = ex;
            issueInfo.value = undefined;
        }
    }
    finally {
        loading.value = false;
    }
}

const issueQuery = graphql(`query FragmentParentQuery_IssueQuery($key: String)
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
            ...FragmentChild_UserInfoFragment
        },
        reporter
        {
            username,
            ...FragmentChild_UserInfoFragment
        }
    }
}`)

</script>
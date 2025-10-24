<template>
    <div>
        <p> {{ userInfo.displayName }}</p>
        <p>UserName: {{ userInfo.username }}</p>
        <p>Email: {{ userInfo.email }} </p>
        <p>Key: {{ userInfo.key }}</p>
        <p>IsActive: {{ userInfo.isActive }}</p>
        <div>
            <p>Groups:</p>
            <li v-for="group in userInfo.groups"> {{ group?.name }}</li>
        </div>
    </div>
    
</template>

<script setup lang="ts">

import { computed } from "vue";
import { graphql, useFragment, type FragmentType } from "../../gql/codegen"


const userInfoFragment = graphql(`fragment FragmentChild_UserInfoFragment on User {
    key,
    username,
    displayName
    email,
    isActive,
    groups {
        name
    }
}`)

interface Props {
    user: FragmentType<typeof userInfoFragment>  // | null | undefined
}

const props = defineProps<Props>()

const userInfo = computed(() => useFragment(userInfoFragment, props.user))

</script>
/* eslint-disable */
import * as types from './graphql';
import type { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';

/**
 * Map of all GraphQL operations in the project.
 *
 * This map has several performance disadvantages:
 * 1. It is not tree-shakeable, so it will include all operations in the project.
 * 2. It is not minifiable, so the string of a GraphQL query will be multiple times inside the bundle.
 * 3. It does not support dead code elimination, so it will add unused operations.
 *
 * Therefore it is highly recommended to use the babel or swc plugin for production.
 * Learn more about it here: https://the-guild.dev/graphql/codegen/plugins/presets/preset-client#reducing-bundle-size
 */
type Documents = {
    "query SimpleQuery_IssueQuery($key: String)\n        {\n            issue(key: $key)\n            {\n                id,\n                summary,\n                priority\n                {\n                    name\n                },\n                issueStatus\n                {\n                    name\n                },\n                assignee\n                {\n                    username,\n                },\n                reporter\n                {\n                    username,\n                }\n            }\n        }": typeof types.SimpleQuery_IssueQueryDocument,
    "query FileDefineQuery_IssueQuery($key: String) {\n  issue(key: $key) {\n    id\n    summary\n    priority {\n      name\n    }\n    issueStatus {\n      name\n    }\n    assignee {\n      username\n    }\n    reporter {\n      username\n    }\n  }\n}": typeof types.FileDefineQuery_IssueQueryDocument,
    "fragment FragmentChild_UserInfoFragment on User {\n    key,\n    username,\n    displayName\n    email,\n    isActive,\n    groups {\n        name\n    }\n}": typeof types.FragmentChild_UserInfoFragmentFragmentDoc,
    "query FragmentParentQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n            ...FragmentChild_UserInfoFragment\n        },\n        reporter\n        {\n            username,\n            ...FragmentChild_UserInfoFragment\n        }\n    }\n}": typeof types.FragmentParentQuery_IssueQueryDocument,
    "query InComponentQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n        },\n        reporter\n        {\n            username,\n        }\n    }\n}": typeof types.InComponentQuery_IssueQueryDocument,
    "query SimpleQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n        },\n        reporter\n        {\n            username,\n        }\n    }\n}": typeof types.SimpleQuery_IssueQueryDocument,
};
const documents: Documents = {
    "query SimpleQuery_IssueQuery($key: String)\n        {\n            issue(key: $key)\n            {\n                id,\n                summary,\n                priority\n                {\n                    name\n                },\n                issueStatus\n                {\n                    name\n                },\n                assignee\n                {\n                    username,\n                },\n                reporter\n                {\n                    username,\n                }\n            }\n        }": types.SimpleQuery_IssueQueryDocument,
    "query FileDefineQuery_IssueQuery($key: String) {\n  issue(key: $key) {\n    id\n    summary\n    priority {\n      name\n    }\n    issueStatus {\n      name\n    }\n    assignee {\n      username\n    }\n    reporter {\n      username\n    }\n  }\n}": types.FileDefineQuery_IssueQueryDocument,
    "fragment FragmentChild_UserInfoFragment on User {\n    key,\n    username,\n    displayName\n    email,\n    isActive,\n    groups {\n        name\n    }\n}": types.FragmentChild_UserInfoFragmentFragmentDoc,
    "query FragmentParentQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n            ...FragmentChild_UserInfoFragment\n        },\n        reporter\n        {\n            username,\n            ...FragmentChild_UserInfoFragment\n        }\n    }\n}": types.FragmentParentQuery_IssueQueryDocument,
    "query InComponentQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n        },\n        reporter\n        {\n            username,\n        }\n    }\n}": types.InComponentQuery_IssueQueryDocument,
    "query SimpleQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n        },\n        reporter\n        {\n            username,\n        }\n    }\n}": types.SimpleQuery_IssueQueryDocument,
};

/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 *
 *
 * @example
 * ```ts
 * const query = graphql(`query GetUser($id: ID!) { user(id: $id) { name } }`);
 * ```
 *
 * The query argument is unknown!
 * Please regenerate the types.
 */
export function graphql(source: string): unknown;

/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "query SimpleQuery_IssueQuery($key: String)\n        {\n            issue(key: $key)\n            {\n                id,\n                summary,\n                priority\n                {\n                    name\n                },\n                issueStatus\n                {\n                    name\n                },\n                assignee\n                {\n                    username,\n                },\n                reporter\n                {\n                    username,\n                }\n            }\n        }"): (typeof documents)["query SimpleQuery_IssueQuery($key: String)\n        {\n            issue(key: $key)\n            {\n                id,\n                summary,\n                priority\n                {\n                    name\n                },\n                issueStatus\n                {\n                    name\n                },\n                assignee\n                {\n                    username,\n                },\n                reporter\n                {\n                    username,\n                }\n            }\n        }"];
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "query FileDefineQuery_IssueQuery($key: String) {\n  issue(key: $key) {\n    id\n    summary\n    priority {\n      name\n    }\n    issueStatus {\n      name\n    }\n    assignee {\n      username\n    }\n    reporter {\n      username\n    }\n  }\n}"): (typeof documents)["query FileDefineQuery_IssueQuery($key: String) {\n  issue(key: $key) {\n    id\n    summary\n    priority {\n      name\n    }\n    issueStatus {\n      name\n    }\n    assignee {\n      username\n    }\n    reporter {\n      username\n    }\n  }\n}"];
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "fragment FragmentChild_UserInfoFragment on User {\n    key,\n    username,\n    displayName\n    email,\n    isActive,\n    groups {\n        name\n    }\n}"): (typeof documents)["fragment FragmentChild_UserInfoFragment on User {\n    key,\n    username,\n    displayName\n    email,\n    isActive,\n    groups {\n        name\n    }\n}"];
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "query FragmentParentQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n            ...FragmentChild_UserInfoFragment\n        },\n        reporter\n        {\n            username,\n            ...FragmentChild_UserInfoFragment\n        }\n    }\n}"): (typeof documents)["query FragmentParentQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n            ...FragmentChild_UserInfoFragment\n        },\n        reporter\n        {\n            username,\n            ...FragmentChild_UserInfoFragment\n        }\n    }\n}"];
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "query InComponentQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n        },\n        reporter\n        {\n            username,\n        }\n    }\n}"): (typeof documents)["query InComponentQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n        },\n        reporter\n        {\n            username,\n        }\n    }\n}"];
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "query SimpleQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n        },\n        reporter\n        {\n            username,\n        }\n    }\n}"): (typeof documents)["query SimpleQuery_IssueQuery($key: String)\n{\n    issue(key: $key)\n    {\n        id,\n        summary,\n        priority\n        {\n            name\n        },\n        issueStatus\n        {\n            name\n        },\n        assignee\n        {\n            username,\n        },\n        reporter\n        {\n            username,\n        }\n    }\n}"];

export function graphql(source: string) {
  return (documents as any)[source] ?? {};
}

export type DocumentType<TDocumentNode extends DocumentNode<any, any>> = TDocumentNode extends DocumentNode<  infer TType,  any>  ? TType  : never;
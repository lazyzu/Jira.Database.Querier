/* eslint-disable */
import type { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';
export type Maybe<T> = T | null;
export type InputMaybe<T> = T | null | undefined;
export type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
export type MakeOptional<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]: Maybe<T[SubKey]> };
export type MakeEmpty<T extends { [key: string]: unknown }, K extends keyof T> = { [_ in K]?: never };
export type Incremental<T> = T | { [P in keyof T]?: P extends ' $fragmentName' | '__typename' ? T[P] : never };
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: { input: string; output: string; }
  String: { input: string; output: string; }
  Boolean: { input: boolean; output: boolean; }
  Int: { input: number; output: number; }
  Float: { input: number; output: number; }
  /** The `DateTime` scalar type represents a date and time. `DateTime` expects timestamps to be formatted in accordance with the [ISO-8601](https://en.wikipedia.org/wiki/ISO_8601) standard. */
  DateTime: { input: any; output: any; }
  Decimal: { input: any; output: any; }
  /** The `Seconds` scalar type represents a period of time represented as the total number of seconds in range [-922337203685, 922337203685]. */
  Seconds: { input: any; output: any; }
  Uri: { input: any; output: any; }
};

export type AvatarUrl = {
  __typename?: 'AvatarUrl';
  large?: Maybe<Scalars['String']['output']>;
  medium?: Maybe<Scalars['String']['output']>;
  small?: Maybe<Scalars['String']['output']>;
  xSmall?: Maybe<Scalars['String']['output']>;
};

export type FullProjectComponent = {
  __typename?: 'FullProjectComponent';
  archived: Scalars['Boolean']['output'];
  deleted: Scalars['Boolean']['output'];
  description?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
};

export type FullProjectVersion = {
  __typename?: 'FullProjectVersion';
  archived: Scalars['Boolean']['output'];
  description?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
  releaseDate?: Maybe<Scalars['DateTime']['output']>;
  startDate?: Maybe<Scalars['DateTime']['output']>;
};

export type Issue = {
  __typename?: 'Issue';
  actualStartDateTime?: Maybe<Scalars['DateTime']['output']>;
  actualTestTime?: Maybe<Scalars['String']['output']>;
  affectsVersions?: Maybe<Array<Maybe<ProjectVersion>>>;
  assignee?: Maybe<User>;
  attachments?: Maybe<Array<Maybe<IssueAttachment>>>;
  burnInTimeSec?: Maybe<Scalars['Decimal']['output']>;
  comments?: Maybe<Array<Maybe<IssueComment>>>;
  components?: Maybe<Array<Maybe<ProjectComponent>>>;
  createDate?: Maybe<Scalars['DateTime']['output']>;
  description?: Maybe<Scalars['String']['output']>;
  dueDate?: Maybe<Scalars['DateTime']['output']>;
  environment?: Maybe<Scalars['String']['output']>;
  fixVersions?: Maybe<Array<Maybe<ProjectVersion>>>;
  id: Scalars['Decimal']['output'];
  issueLinks?: Maybe<Array<Maybe<IssueLink>>>;
  issueNum?: Maybe<Scalars['Decimal']['output']>;
  issueStatus?: Maybe<IssueStatus>;
  issueType?: Maybe<IssueType>;
  key?: Maybe<Scalars['String']['output']>;
  labels?: Maybe<Array<Maybe<Scalars['String']['output']>>>;
  parentIssueId?: Maybe<Scalars['Decimal']['output']>;
  priority?: Maybe<IssuePriority>;
  project?: Maybe<Project>;
  remoteLinks?: Maybe<Array<Maybe<IssueRemoteLink>>>;
  reporter?: Maybe<User>;
  resolution?: Maybe<IssueResolution>;
  resolutionDate?: Maybe<Scalars['DateTime']['output']>;
  securityLevel?: Maybe<IssueSecurityLevel>;
  subTaskIds?: Maybe<Array<Scalars['Decimal']['output']>>;
  summary?: Maybe<Scalars['String']['output']>;
  updateDate?: Maybe<Scalars['DateTime']['output']>;
  votes?: Maybe<Scalars['Decimal']['output']>;
  worklogs?: Maybe<Array<Maybe<IssueWorklog>>>;
};

export type IssueAttachment = {
  __typename?: 'IssueAttachment';
  author?: Maybe<Scalars['String']['output']>;
  content?: Maybe<Scalars['Uri']['output']>;
  created: Scalars['DateTime']['output'];
  fileName?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  mimeType?: Maybe<Scalars['String']['output']>;
  size: Scalars['Decimal']['output'];
};

export type IssueComment = {
  __typename?: 'IssueComment';
  author?: Maybe<Scalars['String']['output']>;
  body?: Maybe<Scalars['String']['output']>;
  created?: Maybe<Scalars['DateTime']['output']>;
  id: Scalars['Decimal']['output'];
  updateAuthor?: Maybe<Scalars['String']['output']>;
  updated?: Maybe<Scalars['DateTime']['output']>;
};

export type IssueLink = {
  __typename?: 'IssueLink';
  id: Scalars['Decimal']['output'];
  inwardIssueId?: Maybe<Scalars['Decimal']['output']>;
  linkType?: Maybe<IssueLinkType>;
  outwardIssueId?: Maybe<Scalars['Decimal']['output']>;
};

export type IssueLinkType = {
  __typename?: 'IssueLinkType';
  id: Scalars['Decimal']['output'];
  inward?: Maybe<Scalars['String']['output']>;
  name?: Maybe<Scalars['String']['output']>;
  outward?: Maybe<Scalars['String']['output']>;
};

export type IssuePriority = {
  __typename?: 'IssuePriority';
  description?: Maybe<Scalars['String']['output']>;
  id?: Maybe<Scalars['String']['output']>;
  name?: Maybe<Scalars['String']['output']>;
};

export type IssueRemoteLink = {
  __typename?: 'IssueRemoteLink';
  id: Scalars['Decimal']['output'];
  relationship?: Maybe<Scalars['String']['output']>;
  remoteUrl?: Maybe<Scalars['String']['output']>;
  summary?: Maybe<Scalars['String']['output']>;
  title?: Maybe<Scalars['String']['output']>;
};

export type IssueResolution = {
  __typename?: 'IssueResolution';
  description?: Maybe<Scalars['String']['output']>;
  id?: Maybe<Scalars['String']['output']>;
  name?: Maybe<Scalars['String']['output']>;
};

export type IssueSecurityLevel = {
  __typename?: 'IssueSecurityLevel';
  description?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
  scheme?: Maybe<IssueSecurityLevelScheme>;
};

export type IssueSecurityLevelScheme = {
  __typename?: 'IssueSecurityLevelScheme';
  description?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
};

export type IssueStatus = {
  __typename?: 'IssueStatus';
  category?: Maybe<IssueStatusCategory>;
  description?: Maybe<Scalars['String']['output']>;
  id?: Maybe<Scalars['String']['output']>;
  name?: Maybe<Scalars['String']['output']>;
};

export type IssueStatusCategory = {
  __typename?: 'IssueStatusCategory';
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
};

export type IssueType = {
  __typename?: 'IssueType';
  description?: Maybe<Scalars['String']['output']>;
  id?: Maybe<Scalars['String']['output']>;
  isSubTask: Scalars['Boolean']['output'];
  name?: Maybe<Scalars['String']['output']>;
};

export type IssueTypeScheme = {
  __typename?: 'IssueTypeScheme';
  id: Scalars['Decimal']['output'];
  issueTypes?: Maybe<Array<Maybe<IssueType>>>;
};

export type IssueWorklog = {
  __typename?: 'IssueWorklog';
  author?: Maybe<Scalars['String']['output']>;
  comment?: Maybe<Scalars['String']['output']>;
  created?: Maybe<Scalars['DateTime']['output']>;
  id: Scalars['Decimal']['output'];
  started?: Maybe<Scalars['DateTime']['output']>;
  timeSpent?: Maybe<Scalars['Seconds']['output']>;
  updateAuthor?: Maybe<Scalars['String']['output']>;
  updated?: Maybe<Scalars['DateTime']['output']>;
};

export type JiraDatabaseQuery = {
  __typename?: 'JiraDatabaseQuery';
  issue?: Maybe<Issue>;
  issues?: Maybe<Array<Maybe<Issue>>>;
  project?: Maybe<Project>;
  projects?: Maybe<Array<Maybe<Project>>>;
  user?: Maybe<User>;
  users?: Maybe<Array<Maybe<User>>>;
};


export type JiraDatabaseQueryIssueArgs = {
  id?: InputMaybe<Scalars['Decimal']['input']>;
  key?: InputMaybe<Scalars['String']['input']>;
};


export type JiraDatabaseQueryIssuesArgs = {
  ids?: InputMaybe<Scalars['Decimal']['input']>;
  keys?: InputMaybe<Scalars['String']['input']>;
};


export type JiraDatabaseQueryProjectArgs = {
  id?: InputMaybe<Scalars['Decimal']['input']>;
  key?: InputMaybe<Scalars['String']['input']>;
};


export type JiraDatabaseQueryProjectsArgs = {
  ids?: InputMaybe<Array<Scalars['Decimal']['input']>>;
  keys?: InputMaybe<Array<InputMaybe<Scalars['String']['input']>>>;
};


export type JiraDatabaseQueryUserArgs = {
  key?: InputMaybe<Scalars['String']['input']>;
  username?: InputMaybe<Scalars['String']['input']>;
};


export type JiraDatabaseQueryUsersArgs = {
  keys?: InputMaybe<Array<InputMaybe<Scalars['String']['input']>>>;
  usernames?: InputMaybe<Array<InputMaybe<Scalars['String']['input']>>>;
};

export type Project = {
  __typename?: 'Project';
  /** Project Avatar */
  avatar?: Maybe<ProjectAvatar>;
  /** Project Category */
  category?: Maybe<ProjectCategory>;
  /** Project Components */
  components?: Maybe<Array<Maybe<FullProjectComponent>>>;
  /** Project Description */
  description?: Maybe<Scalars['String']['output']>;
  /** Project Id */
  id: Scalars['Decimal']['output'];
  /** Project IssueTypeScheme */
  issueTypeScheme?: Maybe<IssueTypeScheme>;
  /** Project Key */
  key?: Maybe<Scalars['String']['output']>;
  /** Project Lead */
  lead?: Maybe<User>;
  /** Project Name */
  name?: Maybe<Scalars['String']['output']>;
  /** Project Roles */
  projectRoles?: Maybe<Array<Maybe<ProjectRoleActorMap>>>;
  /** Project SecurityLevels */
  securityLevels?: Maybe<Array<Maybe<IssueSecurityLevel>>>;
  /** Project Type */
  type?: Maybe<Scalars['String']['output']>;
  /** Project Url */
  url?: Maybe<Scalars['String']['output']>;
  /** Project Versions */
  versions?: Maybe<Array<Maybe<FullProjectVersion>>>;
};

export type ProjectAvatar = {
  __typename?: 'ProjectAvatar';
  id: Scalars['Decimal']['output'];
  urls?: Maybe<AvatarUrl>;
};

export type ProjectCategory = {
  __typename?: 'ProjectCategory';
  description?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
};

export type ProjectComponent = {
  __typename?: 'ProjectComponent';
  archived: Scalars['Boolean']['output'];
  deleted: Scalars['Boolean']['output'];
  description?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
};

export type ProjectRoleActor = {
  __typename?: 'ProjectRoleActor';
  type?: Maybe<Scalars['String']['output']>;
  value?: Maybe<Scalars['String']['output']>;
};

export type ProjectRoleActorMap = {
  __typename?: 'ProjectRoleActorMap';
  actors?: Maybe<Array<Maybe<ProjectRoleActor>>>;
  description?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
};

export type ProjectVersion = {
  __typename?: 'ProjectVersion';
  archived: Scalars['Boolean']['output'];
  description?: Maybe<Scalars['String']['output']>;
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
  releaseDate?: Maybe<Scalars['DateTime']['output']>;
  startDate?: Maybe<Scalars['DateTime']['output']>;
};

export type User = {
  __typename?: 'User';
  /** Id in AppUser Table */
  appId: Scalars['Decimal']['output'];
  /** Avatar of Jira User */
  avatar?: Maybe<UserAvatar>;
  /** Id in CwdUser Table */
  cwdId: Scalars['Decimal']['output'];
  /** Display name of Jira User */
  displayName?: Maybe<Scalars['String']['output']>;
  /** Email of Jira User */
  email?: Maybe<Scalars['String']['output']>;
  /** Groups of Jira User */
  groups?: Maybe<Array<Maybe<UserGroup>>>;
  /** Isactive of Jira User */
  isActive?: Maybe<Scalars['Boolean']['output']>;
  /** Key mapping between AppUser & CwdUser */
  key?: Maybe<Scalars['String']['output']>;
  /** User name of Jira User */
  username?: Maybe<Scalars['String']['output']>;
};

export type UserAvatar = {
  __typename?: 'UserAvatar';
  id: Scalars['Decimal']['output'];
  urls?: Maybe<AvatarUrl>;
};

export type UserGroup = {
  __typename?: 'UserGroup';
  id: Scalars['Decimal']['output'];
  name?: Maybe<Scalars['String']['output']>;
};

export type SimpleQuery_IssueQueryQueryVariables = Exact<{
  key?: InputMaybe<Scalars['String']['input']>;
}>;


export type SimpleQuery_IssueQueryQuery = { __typename?: 'JiraDatabaseQuery', issue?: { __typename?: 'Issue', id: any, summary?: string | null, priority?: { __typename?: 'IssuePriority', name?: string | null } | null, issueStatus?: { __typename?: 'IssueStatus', name?: string | null } | null, assignee?: { __typename?: 'User', username?: string | null } | null, reporter?: { __typename?: 'User', username?: string | null } | null } | null };

export type FileDefineQuery_IssueQueryQueryVariables = Exact<{
  key?: InputMaybe<Scalars['String']['input']>;
}>;


export type FileDefineQuery_IssueQueryQuery = { __typename?: 'JiraDatabaseQuery', issue?: { __typename?: 'Issue', id: any, summary?: string | null, priority?: { __typename?: 'IssuePriority', name?: string | null } | null, issueStatus?: { __typename?: 'IssueStatus', name?: string | null } | null, assignee?: { __typename?: 'User', username?: string | null } | null, reporter?: { __typename?: 'User', username?: string | null } | null } | null };

export type FragmentChild_UserInfoFragmentFragment = { __typename?: 'User', key?: string | null, username?: string | null, displayName?: string | null, email?: string | null, isActive?: boolean | null, groups?: Array<{ __typename?: 'UserGroup', name?: string | null } | null> | null } & { ' $fragmentName'?: 'FragmentChild_UserInfoFragmentFragment' };

export type FragmentParentQuery_IssueQueryQueryVariables = Exact<{
  key?: InputMaybe<Scalars['String']['input']>;
}>;


export type FragmentParentQuery_IssueQueryQuery = { __typename?: 'JiraDatabaseQuery', issue?: { __typename?: 'Issue', id: any, summary?: string | null, priority?: { __typename?: 'IssuePriority', name?: string | null } | null, issueStatus?: { __typename?: 'IssueStatus', name?: string | null } | null, assignee?: (
      { __typename?: 'User', username?: string | null }
      & { ' $fragmentRefs'?: { 'FragmentChild_UserInfoFragmentFragment': FragmentChild_UserInfoFragmentFragment } }
    ) | null, reporter?: (
      { __typename?: 'User', username?: string | null }
      & { ' $fragmentRefs'?: { 'FragmentChild_UserInfoFragmentFragment': FragmentChild_UserInfoFragmentFragment } }
    ) | null } | null };

export type InComponentQuery_IssueQueryQueryVariables = Exact<{
  key?: InputMaybe<Scalars['String']['input']>;
}>;


export type InComponentQuery_IssueQueryQuery = { __typename?: 'JiraDatabaseQuery', issue?: { __typename?: 'Issue', id: any, summary?: string | null, priority?: { __typename?: 'IssuePriority', name?: string | null } | null, issueStatus?: { __typename?: 'IssueStatus', name?: string | null } | null, assignee?: { __typename?: 'User', username?: string | null } | null, reporter?: { __typename?: 'User', username?: string | null } | null } | null };

export const FragmentChild_UserInfoFragmentFragmentDoc = {"kind":"Document","definitions":[{"kind":"FragmentDefinition","name":{"kind":"Name","value":"FragmentChild_UserInfoFragment"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"User"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"key"}},{"kind":"Field","name":{"kind":"Name","value":"username"}},{"kind":"Field","name":{"kind":"Name","value":"displayName"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"groups"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}}]} as unknown as DocumentNode<FragmentChild_UserInfoFragmentFragment, unknown>;
export const SimpleQuery_IssueQueryDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"SimpleQuery_IssueQuery"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"key"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"issue"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"key"},"value":{"kind":"Variable","name":{"kind":"Name","value":"key"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"summary"}},{"kind":"Field","name":{"kind":"Name","value":"priority"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}},{"kind":"Field","name":{"kind":"Name","value":"issueStatus"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}},{"kind":"Field","name":{"kind":"Name","value":"assignee"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"username"}}]}},{"kind":"Field","name":{"kind":"Name","value":"reporter"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"username"}}]}}]}}]}}]} as unknown as DocumentNode<SimpleQuery_IssueQueryQuery, SimpleQuery_IssueQueryQueryVariables>;
export const FileDefineQuery_IssueQueryDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"FileDefineQuery_IssueQuery"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"key"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"issue"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"key"},"value":{"kind":"Variable","name":{"kind":"Name","value":"key"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"summary"}},{"kind":"Field","name":{"kind":"Name","value":"priority"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}},{"kind":"Field","name":{"kind":"Name","value":"issueStatus"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}},{"kind":"Field","name":{"kind":"Name","value":"assignee"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"username"}}]}},{"kind":"Field","name":{"kind":"Name","value":"reporter"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"username"}}]}}]}}]}}]} as unknown as DocumentNode<FileDefineQuery_IssueQueryQuery, FileDefineQuery_IssueQueryQueryVariables>;
export const FragmentParentQuery_IssueQueryDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"FragmentParentQuery_IssueQuery"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"key"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"issue"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"key"},"value":{"kind":"Variable","name":{"kind":"Name","value":"key"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"summary"}},{"kind":"Field","name":{"kind":"Name","value":"priority"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}},{"kind":"Field","name":{"kind":"Name","value":"issueStatus"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}},{"kind":"Field","name":{"kind":"Name","value":"assignee"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"username"}},{"kind":"FragmentSpread","name":{"kind":"Name","value":"FragmentChild_UserInfoFragment"}}]}},{"kind":"Field","name":{"kind":"Name","value":"reporter"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"username"}},{"kind":"FragmentSpread","name":{"kind":"Name","value":"FragmentChild_UserInfoFragment"}}]}}]}}]}},{"kind":"FragmentDefinition","name":{"kind":"Name","value":"FragmentChild_UserInfoFragment"},"typeCondition":{"kind":"NamedType","name":{"kind":"Name","value":"User"}},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"key"}},{"kind":"Field","name":{"kind":"Name","value":"username"}},{"kind":"Field","name":{"kind":"Name","value":"displayName"}},{"kind":"Field","name":{"kind":"Name","value":"email"}},{"kind":"Field","name":{"kind":"Name","value":"isActive"}},{"kind":"Field","name":{"kind":"Name","value":"groups"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}}]}}]} as unknown as DocumentNode<FragmentParentQuery_IssueQueryQuery, FragmentParentQuery_IssueQueryQueryVariables>;
export const InComponentQuery_IssueQueryDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"InComponentQuery_IssueQuery"},"variableDefinitions":[{"kind":"VariableDefinition","variable":{"kind":"Variable","name":{"kind":"Name","value":"key"}},"type":{"kind":"NamedType","name":{"kind":"Name","value":"String"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"issue"},"arguments":[{"kind":"Argument","name":{"kind":"Name","value":"key"},"value":{"kind":"Variable","name":{"kind":"Name","value":"key"}}}],"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"summary"}},{"kind":"Field","name":{"kind":"Name","value":"priority"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}},{"kind":"Field","name":{"kind":"Name","value":"issueStatus"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"name"}}]}},{"kind":"Field","name":{"kind":"Name","value":"assignee"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"username"}}]}},{"kind":"Field","name":{"kind":"Name","value":"reporter"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"username"}}]}}]}}]}}]} as unknown as DocumentNode<InComponentQuery_IssueQueryQuery, InComponentQuery_IssueQueryQueryVariables>;
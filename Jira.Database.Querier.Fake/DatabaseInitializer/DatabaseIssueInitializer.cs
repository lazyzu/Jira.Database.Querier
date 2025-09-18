using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Fake.DatabaseInitializer
{
    public class DatabaseIssueInitializer
    {
        public readonly FieldSelectionHandler FieldSelection = new FieldSelectionHandler();
        protected readonly FieldHandler Field = new FieldHandler();

        public async Task AddJiraIssue(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
        {
            await jiraContext.jiraissue.AddAsync(new EntityFrameworkCore.Model.jiraissue
            {
                ID = issue.Id,
                pkey = null,
                issuenum = issue.IssueNum,
                PROJECT = issue.Project.Id,
                REPORTER = issue.Reporter.Key,
                ASSIGNEE = issue.Assignee.Key,
                CREATOR = null, //TODO: Not support yet
                issuetype = issue.IssueType.Id,
                SUMMARY = issue.Summary,
                DESCRIPTION = issue.Description,
                ENVIRONMENT = issue.Environment,
                PRIORITY = issue.Priority.Id,
                RESOLUTION = issue.Resolution?.Id,
                issuestatus = issue.IssueStatus.Id,
                CREATED = issue.CreateDate,
                UPDATED = issue.UpdateDate,
                DUEDATE = issue.DueDate,
                RESOLUTIONDATE = issue.ResolutionDate,
                VOTES = issue.Votes,
                WATCHES = null, //TODO: Not support yet
                TIMEORIGINALESTIMATE = null,//TODO: Not support yet
                TIMEESTIMATE = null,//TODO: Not support yet
                TIMESPENT = null,//TODO: Not support yet
                WORKFLOW_ID = null,//TODO: Not support yet
                SECURITY = issue.SecurityLevel.Id,
                FIXFOR = null,//TODO: Not support yet
                COMPONENT = null,//TODO: Not support yet
                ARCHIVEDBY = null,//TODO: Not support yet
                ARCHIVEDDATE = null,//TODO: Not support yet
                ARCHIVED = null //TODO: Not support yet
            });

            await Field.RenderIssueComponentsAssociationTable(issue, jiraContext, saveChange: false);
            await Field.RenderIssueAffectVersionsAssociationTable(issue, jiraContext, saveChange: false);
            await Field.RenderIssueFixVersionsAssociationTable(issue, jiraContext, saveChange: false);
            await Field.RenderIssueLabelsAssociationTable(issue, jiraContext, saveChange: false);
            await Field.RenderIssueCommentsAssociationTable(issue, jiraContext, saveChange: false);
            await Field.RenderIssueWorklogsAssociationTable(issue, jiraContext, saveChange: false);
            await Field.RenderIssueChangelogsAssociationTable(issue, jiraContext, saveChange: false);
            await Field.RenderIssueRemoteLinksAssociationTable(issue, jiraContext, saveChange: false);

            // Link is not implement when issue create

            await Field.RenderIssueAttachmentsAssociationTable(issue, jiraContext);

            foreach (var customFieldValuePair in issue.CustomFields.customFieldValueBag)
            {
                await Field.RenderCustomField(issue.Id, customFieldValuePair.Key, customFieldValuePair.Value, jiraContext, saveChange: false);
            }

            if (saveChange) await jiraContext.SaveChangesAsync();
        }

        public Task MoveIssue(IJiraIssue issue, string originalKey, JiraContext jiraContext, bool saveChange = true)
            => Field.RenderIssueMovedKey(issue, originalKey, jiraContext, saveChange);

        public Task Link(IJiraIssue outwardIssue, IJiraIssue inwardIssue, IIssueLinkType issueLinkType, JiraContext jiraContext, bool saveChange = true)
            => Field.RenderIssueLinksAssociationTable(outwardIssue, inwardIssue, issueLinkType, jiraContext, saveChange);

        public class FieldSelectionHandler
        {
            public Task AddIssueStatus(IEnumerable<IIssueStatus> issueStatuses, JiraContext jiraContext, bool saveChange = true)
                => Util.AddMultiple(issueStatuses, jiraContext, AddIssueStatus, saveChange);

            public async Task AddIssueStatus(IIssueStatus issueStatus, JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.issuestatus.AddAsync(new issuestatus
                {
                    ID = issueStatus.Id,
                    SEQUENCE = null,    // TODO: Not support yet
                    pname = issueStatus.Name,
                    DESCRIPTION = issueStatus.Description,
                    ICONURL = null, // TODO: Not support yet
                    STATUSCATEGORY = issueStatus.Category.Id
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public Task AddIssuePriority(IEnumerable<IIssuePriority> issuePriorities, JiraContext jiraContext, bool saveChange = true)
                => Util.AddMultiple(issuePriorities, jiraContext, AddIssuePriority, saveChange);

            public async Task AddIssuePriority(IIssuePriority issuePriority, JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.priority.AddAsync(new priority
                {
                    ID = issuePriority.Id,
                    SEQUENCE = null,    // TODO: Not support yet
                    pname = issuePriority.Name,
                    DESCRIPTION = issuePriority.Description,
                    ICONURL = null,     // TODO: Not support yet
                    STATUS_COLOR = null,// TODO: Not support yet
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public Task AddIssueResolution(IEnumerable<IIssueResolution> issueResolutions, JiraContext jiraContext, bool saveChange = true)
                => Util.AddMultiple(issueResolutions, jiraContext, AddIssueResolution, saveChange);

            public async Task AddIssueResolution(IIssueResolution issueResolution, JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.resolution.AddAsync(new resolution
                {
                    ID = issueResolution.Id,
                    SEQUENCE = null,    // TODO: Not support yet
                    pname = issueResolution.Name,
                    DESCRIPTION = issueResolution.Description,
                    ICONURL = null      // TODO: Not support yet
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public Task AddIssueLinkTypes(IEnumerable<IIssueLinkType> issueLinkTypes, JiraContext jiraContext, bool saveChange = true)
                => Util.AddMultiple(issueLinkTypes, jiraContext, AddIssueLinkTypes, saveChange);

            public async Task AddIssueLinkTypes(IIssueLinkType issueLinkType, JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.issuelinktype.AddAsync(new issuelinktype
                {
                    ID = issueLinkType.Id,
                    LINKNAME = issueLinkType.Name,
                    INWARD = issueLinkType.Inward,
                    OUTWARD = issueLinkType.Outward,
                    pstyle = null   // TODO: Not support yet
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }
        }

        public class FieldHandler
        {
            protected long movedIdIndex = 0;
            protected long labelIdIndex = 0;
            protected long changeitemIdIndex = 0;
            protected long issuelinkIdIndex = 0;

            protected long customfieldvalueIdIndex = 0;

            public virtual async Task RenderIssueMovedKey(IJiraIssue issue, string originalKey, JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.moved_issue_key.AddAsync(new moved_issue_key
                {
                    ID = movedIdIndex++,
                    ISSUE_ID = issue.Id,
                    OLD_ISSUE_KEY = originalKey,
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueComponentsAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.Components?.Any() ?? false)
                {
                    foreach (var component in issue.Components)
                    {
                        await jiraContext.nodeassociation.AddAsync(new nodeassociation
                        {
                            SOURCE_NODE_ID = issue.Id,
                            SOURCE_NODE_ENTITY = "Issue",
                            SINK_NODE_ID = component.Id,
                            SINK_NODE_ENTITY = "Component",
                            ASSOCIATION_TYPE = "IssueComponent",
                            SEQUENCE = null,
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueAffectVersionsAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.AffectsVersions?.Any() ?? false)
                {
                    foreach (var affectVersion in issue.AffectsVersions)
                    {
                        await jiraContext.nodeassociation.AddAsync(new nodeassociation
                        {
                            SOURCE_NODE_ID = issue.Id,
                            SOURCE_NODE_ENTITY = "Issue",
                            SINK_NODE_ID = affectVersion.Id,
                            SINK_NODE_ENTITY = "Version",
                            ASSOCIATION_TYPE = "IssueVersion",
                            SEQUENCE = null,
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueFixVersionsAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.FixVersions?.Any() ?? false)
                {
                    foreach (var fixVersion in issue.FixVersions)
                    {
                        await jiraContext.nodeassociation.AddAsync(new nodeassociation
                        {
                            SOURCE_NODE_ID = issue.Id,
                            SOURCE_NODE_ENTITY = "Issue",
                            SINK_NODE_ID = fixVersion.Id,
                            SINK_NODE_ENTITY = "Version",
                            ASSOCIATION_TYPE = "IssueFixVersion",
                            SEQUENCE = null,
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueLabelsAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.Labels?.Any() ?? false)
                {
                    foreach (var label in issue.Labels)
                    {
                        await jiraContext.label.AddAsync(new label
                        {
                            ID = labelIdIndex++,
                            FIELDID = null,
                            ISSUE = issue.Id,
                            LABEL1 = label
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueCommentsAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.Comments?.Any() ?? false)
                {
                    foreach (var comment in issue.Comments)
                    {
                        await jiraContext.jiraaction.AddAsync(new jiraaction
                        {
                            ID = comment.Id,
                            issueid = issue.Id,
                            AUTHOR = comment.Author,
                            actiontype = "comment",
                            actionlevel = null,
                            rolelevel = null,
                            actionbody = comment.Body,
                            CREATED = comment.Created,
                            UPDATEAUTHOR = comment.UpdateAuthor,
                            UPDATED = comment.Updated,
                            actionnum = null
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueWorklogsAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.Worklogs?.Any() ?? false)
                {
                    foreach (var worklog in issue.Worklogs)
                    {
                        await jiraContext.worklog.AddAsync(new worklog
                        {
                            ID = worklog.Id,
                            issueid = issue.Id,
                            AUTHOR = worklog.Author,
                            grouplevel = null,
                            rolelevel = null,
                            worklogbody = worklog.Comment,
                            CREATED = worklog.Created,
                            UPDATEAUTHOR = worklog.UpdateAuthor,
                            UPDATED = worklog.Updated,
                            STARTDATE = worklog.Started,
                            timeworked = (decimal?)worklog.TimeSpent?.TotalSeconds,
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueChangelogsAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.Changelogs?.Any() ?? false)
                {
                    foreach (var changelog in issue.Changelogs)
                    {
                        await jiraContext.changegroup.AddAsync(new changegroup
                        {
                            ID = changelog.Id,
                            issueid = issue.Id,
                            AUTHOR = changelog.Author,
                            CREATED = changelog.Created
                        });

                        foreach (var changelogItem in changelog.Items)
                        {
                            await jiraContext.changeitem.AddAsync(new changeitem
                            {
                                ID = changeitemIdIndex++,
                                groupid = changelog.Id,
                                FIELDTYPE = changelogItem.FieldType,
                                FIELD = changelogItem.Field,
                                OLDVALUE = changelogItem.OldValue,
                                OLDSTRING = changelogItem.OldString,
                                NEWVALUE = changelogItem.NewValue,
                                NEWSTRING = changelogItem.NewString
                            });
                        }
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueLinksAssociationTable(IJiraIssue outwardIssue, IJiraIssue inwardIssue, IIssueLinkType issueLinkType, JiraContext jiraContext, bool saveChange = true)
            {
                if (jiraContext.issuelinktype.Any(linkType => linkType.ID == issueLinkType.Id) == false)
                {
                    await jiraContext.issuelinktype.AddAsync(new issuelinktype
                    {
                        ID = issueLinkType.Id,
                        LINKNAME = issueLinkType.Name,
                        INWARD = issueLinkType.Inward,
                        OUTWARD = issueLinkType.Outward,
                        pstyle = null
                    });
                }

                await jiraContext.issuelink.AddAsync(new issuelink
                {
                    ID = issuelinkIdIndex++,
                    LINKTYPE = issueLinkType.Id,
                    SOURCE = outwardIssue.Id,
                    DESTINATION = inwardIssue.Id,
                    SEQUENCE = null,
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueRemoteLinksAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.RemoteLinks?.Any() ?? false)
                {
                    foreach (var remoteLink in issue.RemoteLinks)
                    {
                        if (remoteLink is IssueConfluenceLink confluenceLink) await jiraContext.remotelink.AddAsync(new remotelink
                        {
                            ID = remoteLink.Id,
                            ISSUEID = issue.Id,
                            GLOBALID = $"appId={confluenceLink.ConfluenceInstance.AppId}&pageId={confluenceLink.PageId}",
                            TITLE = remoteLink.Title,
                            SUMMARY = remoteLink.Summary,
                            URL = remoteLink.RemoteUrl,
                            ICONURL = null,
                            ICONTITLE = null,
                            RELATIONSHIP = remoteLink.Relationship,
                            RESOLVED = null,
                            STATUSNAME = null,
                            STATUSDESCRIPTION = null,
                            STATUSICONURL = null,
                            STATUSICONTITLE = null,
                            STATUSICONLINK = null,
                            STATUSCATEGORYKEY = null,
                            STATUSCATEGORYCOLORNAME = null,
                            APPLICATIONTYPE = "com.atlassian.confluence",
                            APPLICATIONNAME = confluenceLink.ConfluenceInstance.Name,
                        });
                        else await jiraContext.remotelink.AddAsync(new remotelink
                        {
                            ID = remoteLink.Id,
                            ISSUEID = issue.Id,
                            GLOBALID = null,
                            TITLE = remoteLink.Title,
                            SUMMARY = remoteLink.Summary,
                            URL = remoteLink.RemoteUrl,
                            ICONURL = null,
                            ICONTITLE = null,
                            RELATIONSHIP = remoteLink.Relationship,
                            RESOLVED = null,
                            STATUSNAME = null,
                            STATUSDESCRIPTION = null,
                            STATUSICONURL = null,
                            STATUSICONTITLE = null,
                            STATUSICONLINK = null,
                            STATUSCATEGORYKEY = null,
                            STATUSCATEGORYCOLORNAME = null,
                            APPLICATIONTYPE = null,
                            APPLICATIONNAME = null,
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderIssueAttachmentsAssociationTable(IJiraIssue issue, JiraContext jiraContext, bool saveChange = true)
            {
                if (issue.Attachments?.Any() ?? false)
                {
                    foreach (var attachment in issue.Attachments)
                    {
                        await jiraContext.fileattachment.AddAsync(new fileattachment
                        {
                            ID = attachment.Id,
                            issueid = issue.Id,
                            MIMETYPE = attachment.MimeType,
                            FILENAME = attachment.FileName,
                            CREATED = attachment.Created,
                            FILESIZE = attachment.Size,
                            AUTHOR = attachment.Author,
                            zip = null, //TODO: Not support yet
                            thumbnailable = null    //TODO: Not support yet
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public virtual async Task RenderCustomField(decimal issueId, ICustomFieldKey field, object schemaValue, JiraContext jiraContext, bool saveChange = true)
            {
                if (schemaValue is CascadingSelectCustomFieldSchema cascadingSelectSchema)
                {
                    var parentChildPairs = cascadingSelectSchema.Value.CascadingSelections.Aggregate(new List<(ISelectOption Parent, ISelectOption Child)>(), (result, next) =>
                    {
                        var parent = result.Any() ? result.Last().Child : null;
                        result.Add((Parent: parent, Child: next));
                        return result;
                    }, result => result);

                    foreach (var parentChildPair in parentChildPairs)
                    {
                        if (jiraContext.customfieldoption.Any(fieldOption => fieldOption.ID == parentChildPair.Child.Id) == false) await jiraContext.customfieldoption.AddAsync(new customfieldoption
                        {
                            ID = parentChildPair.Child.Id,
                            CUSTOMFIELD = field.Id,
                            CUSTOMFIELDCONFIG = null,   // TODO: NOT SUPPORT YET
                            PARENTOPTIONID = parentChildPair.Parent?.Id,
                            SEQUENCE = null,    // TODO: NOT SUPPORT YET
                            customvalue = parentChildPair.Child.Value,
                            optiontype = null,
                            disabled = parentChildPair.Child.Disabled ? "Y" : "N",
                        });
                    }

                    await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                    {
                        ID = customfieldvalueIdIndex++,
                        CUSTOMFIELD = field.Id,
                        ISSUE = issueId,
                        STRINGVALUE = cascadingSelectSchema.Value.CascadingSelections.Last().Id.ToString()
                    });
                }
                else if (schemaValue is DateTimeCustomFieldSchema dateTimeSchema)
                {
                    await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                    {
                        ID = customfieldvalueIdIndex++,
                        CUSTOMFIELD = field.Id,
                        ISSUE = issueId,
                        DATEVALUE = dateTimeSchema.Value
                    });
                }
                else if (schemaValue is LabelCustomFieldSchema labelSchema)
                {
                    foreach (var labelValue in labelSchema.Value)
                    {
                        await jiraContext.label.AddAsync(new label
                        {
                            ID = labelIdIndex++,
                            FIELDID = field.Id,
                            ISSUE = issueId,
                            LABEL1 = labelValue
                        });
                    }
                }
                else if (schemaValue is MultiSelectCustomFieldSchema multiSelectSchema)
                {
                    foreach (var fieldSelect in multiSelectSchema.Value.Selections)
                    {
                        if (jiraContext.customfieldoption.Any(fieldOption => fieldOption.ID == fieldSelect.Id) == false) await jiraContext.customfieldoption.AddAsync(new customfieldoption
                        {
                            ID = fieldSelect.Id,
                            CUSTOMFIELD = field.Id,
                            CUSTOMFIELDCONFIG = null,   // TODO: NOT SUPPORT YET
                            PARENTOPTIONID = null,
                            SEQUENCE = null,    // TODO: NOT SUPPORT YET
                            customvalue = fieldSelect.Value,
                            optiontype = null,
                            disabled = fieldSelect.Disabled ? "Y" : "N",
                        });

                        await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                        {
                            ID = customfieldvalueIdIndex++,
                            CUSTOMFIELD = field.Id,
                            ISSUE = issueId,
                            STRINGVALUE = fieldSelect.Id.ToString()
                        });
                    }
                }
                else if (schemaValue is MultiUserCustomFieldSchema multiUserSchema)
                {
                    foreach (var user in multiUserSchema.Value.Users)
                    {
                        await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                        {
                            ID = customfieldvalueIdIndex++,
                            CUSTOMFIELD = field.Id,
                            ISSUE = issueId,
                            STRINGVALUE = user.Key
                        });
                    }
                }
                else if (schemaValue is NumberCustomFieldSchema numberSchema)
                {
                    await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                    {
                        ID = customfieldvalueIdIndex++,
                        CUSTOMFIELD = field.Id,
                        ISSUE = issueId,
                        NUMBERVALUE = numberSchema.Value
                    });
                }
                else if (schemaValue is SelectCustomFieldSchema selectSchema)
                {
                    if (jiraContext.customfieldoption.Any(fieldOption => fieldOption.ID == selectSchema.Value.Id) == false) await jiraContext.customfieldoption.AddAsync(new customfieldoption
                    {
                        ID = selectSchema.Value.Id,
                        CUSTOMFIELD = field.Id,
                        CUSTOMFIELDCONFIG = null,   // TODO: NOT SUPPORT YET
                        PARENTOPTIONID = null,
                        SEQUENCE = null,    // TODO: NOT SUPPORT YET
                        customvalue = selectSchema.Value.Value,
                        optiontype = null,
                        disabled = selectSchema.Value.Disabled ? "Y" : "N",
                    });

                    await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                    {
                        ID = customfieldvalueIdIndex++,
                        CUSTOMFIELD = field.Id,
                        ISSUE = issueId,
                        STRINGVALUE = selectSchema.Value.Id.ToString()
                    });
                }
                else if (schemaValue is StringCustomFieldSchema stringSchema)
                {
                    await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                    {
                        ID = customfieldvalueIdIndex++,
                        CUSTOMFIELD = field.Id,
                        ISSUE = issueId,
                        STRINGVALUE = stringSchema.Value
                    });
                }
                else if (schemaValue is TextCustomFieldSchema textSchema)
                {
                    await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                    {
                        ID = customfieldvalueIdIndex++,
                        CUSTOMFIELD = field.Id,
                        ISSUE = issueId,
                        TEXTVALUE = textSchema.Value
                    });
                }
                else if (schemaValue is UserCustomFieldSchema userSchema)
                {
                    await jiraContext.customfieldvalue.AddAsync(new customfieldvalue
                    {
                        ID = customfieldvalueIdIndex++,
                        CUSTOMFIELD = field.Id,
                        ISSUE = issueId,
                        STRINGVALUE = userSchema.Value.Key
                    });
                }
                else throw new NotSupportedException();

                if (saveChange) await jiraContext.SaveChangesAsync();
            }
        }
    }
}

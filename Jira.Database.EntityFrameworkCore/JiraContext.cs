using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace lazyzu.Jira.Database.EntityFrameworkCore;

public partial class JiraContext : DbContext
{
    public JiraContext()
    {
    }

    public JiraContext(DbContextOptions<JiraContext> options)
        : base(options)
    {
    }

    protected JiraContext(DbContextOptions options)
        : base(options)
    {
    }

    public virtual DbSet<app_user> app_user { get; set; }

    public virtual DbSet<audit_changed_value> audit_changed_value { get; set; }

    public virtual DbSet<audit_item> audit_item { get; set; }

    public virtual DbSet<audit_log> audit_log { get; set; }

    public virtual DbSet<avatar> avatar { get; set; }

    public virtual DbSet<board> board { get; set; }

    public virtual DbSet<boardproject> boardproject { get; set; }

    public virtual DbSet<changegroup> changegroup { get; set; }

    public virtual DbSet<changeitem> changeitem { get; set; }

    public virtual DbSet<columnlayout> columnlayout { get; set; }

    public virtual DbSet<columnlayoutitem> columnlayoutitem { get; set; }

    public virtual DbSet<comment_reaction> comment_reaction { get; set; }

    public virtual DbSet<comment_version> comment_version { get; set; }

    public virtual DbSet<component> component { get; set; }

    public virtual DbSet<configurationcontext> configurationcontext { get; set; }

    public virtual DbSet<customfield> customfield { get; set; }

    public virtual DbSet<customfieldoption> customfieldoption { get; set; }

    public virtual DbSet<customfieldvalue> customfieldvalue { get; set; }

    public virtual DbSet<cwd_application> cwd_application { get; set; }

    public virtual DbSet<cwd_application_address> cwd_application_address { get; set; }

    public virtual DbSet<cwd_directory> cwd_directory { get; set; }

    public virtual DbSet<cwd_directory_attribute> cwd_directory_attribute { get; set; }

    public virtual DbSet<cwd_directory_operation> cwd_directory_operation { get; set; }

    public virtual DbSet<cwd_group> cwd_group { get; set; }

    public virtual DbSet<cwd_group_attributes> cwd_group_attributes { get; set; }

    public virtual DbSet<cwd_membership> cwd_membership { get; set; }

    public virtual DbSet<cwd_synchronisation_status> cwd_synchronisation_status { get; set; }

    public virtual DbSet<cwd_synchronisation_token> cwd_synchronisation_token { get; set; }

    public virtual DbSet<cwd_user> cwd_user { get; set; }

    public virtual DbSet<cwd_user_attributes> cwd_user_attributes { get; set; }

    public virtual DbSet<deadletter> deadletter { get; set; }

    public virtual DbSet<draftworkflowscheme> draftworkflowscheme { get; set; }

    public virtual DbSet<draftworkflowschemeentity> draftworkflowschemeentity { get; set; }

    public virtual DbSet<entity_property> entity_property { get; set; }

    public virtual DbSet<entity_property_index_document> entity_property_index_document { get; set; }

    public virtual DbSet<entity_translation> entity_translation { get; set; }

    public virtual DbSet<external_entities> external_entities { get; set; }

    public virtual DbSet<externalgadget> externalgadget { get; set; }

    public virtual DbSet<favouriteassociations> favouriteassociations { get; set; }

    public virtual DbSet<feature> feature { get; set; }

    public virtual DbSet<fieldconfigscheme> fieldconfigscheme { get; set; }

    public virtual DbSet<fieldconfigschemeissuetype> fieldconfigschemeissuetype { get; set; }

    public virtual DbSet<fieldconfiguration> fieldconfiguration { get; set; }

    public virtual DbSet<fieldlayout> fieldlayout { get; set; }

    public virtual DbSet<fieldlayoutitem> fieldlayoutitem { get; set; }

    public virtual DbSet<fieldlayoutscheme> fieldlayoutscheme { get; set; }

    public virtual DbSet<fieldlayoutschemeassociation> fieldlayoutschemeassociation { get; set; }

    public virtual DbSet<fieldlayoutschemeentity> fieldlayoutschemeentity { get; set; }

    public virtual DbSet<fieldscreen> fieldscreen { get; set; }

    public virtual DbSet<fieldscreenlayoutitem> fieldscreenlayoutitem { get; set; }

    public virtual DbSet<fieldscreenscheme> fieldscreenscheme { get; set; }

    public virtual DbSet<fieldscreenschemeitem> fieldscreenschemeitem { get; set; }

    public virtual DbSet<fieldscreentab> fieldscreentab { get; set; }

    public virtual DbSet<fileattachment> fileattachment { get; set; }

    public virtual DbSet<filtersubscription> filtersubscription { get; set; }

    public virtual DbSet<gadgetuserpreference> gadgetuserpreference { get; set; }

    public virtual DbSet<genericconfiguration> genericconfiguration { get; set; }

    public virtual DbSet<globalpermissionentry> globalpermissionentry { get; set; }

    public virtual DbSet<groupbase> groupbase { get; set; }

    public virtual DbSet<issue_field_option> issue_field_option { get; set; }

    public virtual DbSet<issue_field_option_scope> issue_field_option_scope { get; set; }

    public virtual DbSet<issue_version> issue_version { get; set; }

    public virtual DbSet<issuelink> issuelink { get; set; }

    public virtual DbSet<issuelinktype> issuelinktype { get; set; }

    public virtual DbSet<issuesecurityscheme> issuesecurityscheme { get; set; }

    public virtual DbSet<issuestatus> issuestatus { get; set; }

    public virtual DbSet<issuetype> issuetype { get; set; }

    public virtual DbSet<issuetypescreenscheme> issuetypescreenscheme { get; set; }

    public virtual DbSet<issuetypescreenschemeentity> issuetypescreenschemeentity { get; set; }

    public virtual DbSet<jiraaction> jiraaction { get; set; }

    public virtual DbSet<jiradraftworkflows> jiradraftworkflows { get; set; }

    public virtual DbSet<jiraeventtype> jiraeventtype { get; set; }

    public virtual DbSet<jiraissue> jiraissue { get; set; }

    public virtual DbSet<jiraperms> jiraperms { get; set; }

    public virtual DbSet<jiraworkflows> jiraworkflows { get; set; }

    public virtual DbSet<jiraworkflowstatuses> jiraworkflowstatuses { get; set; }

    public virtual DbSet<label> label { get; set; }

    public virtual DbSet<licenserolesdefault> licenserolesdefault { get; set; }

    public virtual DbSet<licenserolesgroup> licenserolesgroup { get; set; }

    public virtual DbSet<listenerconfig> listenerconfig { get; set; }

    public virtual DbSet<mailserver> mailserver { get; set; }

    public virtual DbSet<managedconfigurationitem> managedconfigurationitem { get; set; }

    public virtual DbSet<membershipbase> membershipbase { get; set; }

    public virtual DbSet<moved_issue_key> moved_issue_key { get; set; }

    public virtual DbSet<nodeassociation> nodeassociation { get; set; }

    public virtual DbSet<nodeindexcounter> nodeindexcounter { get; set; }

    public virtual DbSet<nomenclature_entries> nomenclature_entries { get; set; }

    public virtual DbSet<notification> notification { get; set; }

    public virtual DbSet<notificationinstance> notificationinstance { get; set; }

    public virtual DbSet<notificationscheme> notificationscheme { get; set; }

    public virtual DbSet<oauthconsumer> oauthconsumer { get; set; }

    public virtual DbSet<oauthconsumertoken> oauthconsumertoken { get; set; }

    public virtual DbSet<oauthspconsumer> oauthspconsumer { get; set; }

    public virtual DbSet<oauthsptoken> oauthsptoken { get; set; }

    public virtual DbSet<optionconfiguration> optionconfiguration { get; set; }

    public virtual DbSet<os_currentstep> os_currentstep { get; set; }

    public virtual DbSet<os_currentstep_prev> os_currentstep_prev { get; set; }

    public virtual DbSet<os_historystep> os_historystep { get; set; }

    public virtual DbSet<os_historystep_prev> os_historystep_prev { get; set; }

    public virtual DbSet<os_wfentry> os_wfentry { get; set; }

    public virtual DbSet<permissionscheme> permissionscheme { get; set; }

    public virtual DbSet<permissionschemeattribute> permissionschemeattribute { get; set; }

    public virtual DbSet<pluginstate> pluginstate { get; set; }

    public virtual DbSet<pluginversion> pluginversion { get; set; }

    public virtual DbSet<portalpage> portalpage { get; set; }

    public virtual DbSet<portletconfiguration> portletconfiguration { get; set; }

    public virtual DbSet<priority> priority { get; set; }

    public virtual DbSet<productlicense> productlicense { get; set; }

    public virtual DbSet<project> project { get; set; }

    public virtual DbSet<project_key> project_key { get; set; }

    public virtual DbSet<projectcategory> projectcategory { get; set; }

    public virtual DbSet<projectchangedtime> projectchangedtime { get; set; }

    public virtual DbSet<projectrole> projectrole { get; set; }

    public virtual DbSet<projectroleactor> projectroleactor { get; set; }

    public virtual DbSet<projectversion> projectversion { get; set; }

    public virtual DbSet<propertydata> propertydata { get; set; }

    public virtual DbSet<propertydate> propertydate { get; set; }

    public virtual DbSet<propertydecimal> propertydecimal { get; set; }

    public virtual DbSet<propertyentry> propertyentry { get; set; }

    public virtual DbSet<propertynumber> propertynumber { get; set; }

    public virtual DbSet<propertystring> propertystring { get; set; }

    public virtual DbSet<propertytext> propertytext { get; set; }

    public virtual DbSet<reindex_component> reindex_component { get; set; }

    public virtual DbSet<reindex_request> reindex_request { get; set; }

    public virtual DbSet<remembermetoken> remembermetoken { get; set; }

    public virtual DbSet<remotelink> remotelink { get; set; }

    public virtual DbSet<replicatedindexoperation> replicatedindexoperation { get; set; }

    public virtual DbSet<resolution> resolution { get; set; }

    public virtual DbSet<rundetails> rundetails { get; set; }

    public virtual DbSet<schemeissuesecurities> schemeissuesecurities { get; set; }

    public virtual DbSet<schemeissuesecuritylevels> schemeissuesecuritylevels { get; set; }

    public virtual DbSet<schemepermissions> schemepermissions { get; set; }

    public virtual DbSet<searchrequest> searchrequest { get; set; }

    public virtual DbSet<securityproperty> securityproperty { get; set; }

    public virtual DbSet<sequence_value_item> sequence_value_item { get; set; }

    public virtual DbSet<serviceconfig> serviceconfig { get; set; }

    public virtual DbSet<sharepermissions> sharepermissions { get; set; }

    public virtual DbSet<tempattachmentsmonitor> tempattachmentsmonitor { get; set; }

    public virtual DbSet<trackback_ping> trackback_ping { get; set; }

    public virtual DbSet<trustedapp> trustedapp { get; set; }

    public virtual DbSet<upgradehistory> upgradehistory { get; set; }

    public virtual DbSet<upgradetaskhistory> upgradetaskhistory { get; set; }

    public virtual DbSet<upgradetaskhistoryauditlog> upgradetaskhistoryauditlog { get; set; }

    public virtual DbSet<upgradeversionhistory> upgradeversionhistory { get; set; }

    public virtual DbSet<userassociation> userassociation { get; set; }

    public virtual DbSet<userbase> userbase { get; set; }

    public virtual DbSet<userhistoryitem> userhistoryitem { get; set; }

    public virtual DbSet<userpickerfilter> userpickerfilter { get; set; }

    public virtual DbSet<userpickerfiltergroup> userpickerfiltergroup { get; set; }

    public virtual DbSet<userpickerfilterrole> userpickerfilterrole { get; set; }

    public virtual DbSet<versioncontrol> versioncontrol { get; set; }

    public virtual DbSet<votehistory> votehistory { get; set; }

    public virtual DbSet<workflowscheme> workflowscheme { get; set; }

    public virtual DbSet<workflowschemeentity> workflowschemeentity { get; set; }

    public virtual DbSet<worklog> worklog { get; set; }

    public virtual DbSet<worklog_version> worklog_version { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<app_user>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.lower_user_name, "uk_lower_user_name").IsUnique();

            entity.HasIndex(e => e.user_key, "uk_user_key").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
        });

        modelBuilder.Entity<audit_changed_value>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.LOG_ID, "idx_changed_value_log_id");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.LOG_ID).HasPrecision(18);
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<audit_item>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.LOG_ID, "idx_audit_item_log_id2");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.LOG_ID).HasPrecision(18);
            entity.Property(e => e.OBJECT_ID).HasMaxLength(255);
            entity.Property(e => e.OBJECT_NAME).HasMaxLength(255);
            entity.Property(e => e.OBJECT_PARENT_ID).HasMaxLength(255);
            entity.Property(e => e.OBJECT_PARENT_NAME).HasMaxLength(255);
            entity.Property(e => e.OBJECT_TYPE).HasMaxLength(60);
        });

        modelBuilder.Entity<audit_log>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.CREATED, "idx_audit_log_created");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.AUTHOR_KEY).HasMaxLength(255);
            entity.Property(e => e.AUTHOR_TYPE).HasPrecision(9);
            entity.Property(e => e.CATEGORY).HasMaxLength(255);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.DESCRIPTION).HasMaxLength(255);
            entity.Property(e => e.EVENT_SOURCE_NAME).HasMaxLength(255);
            entity.Property(e => e.OBJECT_ID).HasMaxLength(255);
            entity.Property(e => e.OBJECT_NAME).HasMaxLength(255);
            entity.Property(e => e.OBJECT_PARENT_ID).HasMaxLength(255);
            entity.Property(e => e.OBJECT_PARENT_NAME).HasMaxLength(255);
            entity.Property(e => e.OBJECT_TYPE).HasMaxLength(60);
            entity.Property(e => e.REMOTE_ADDRESS).HasMaxLength(60);
            entity.Property(e => e.SUMMARY).HasMaxLength(255);
        });

        modelBuilder.Entity<avatar>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.filename, e.avatartype, e.systemavatar }, "avatar_filename_index");

            entity.HasIndex(e => new { e.avatartype, e.owner }, "avatar_index");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.avatartype).HasMaxLength(60);
            entity.Property(e => e.contenttype).HasMaxLength(255);
            entity.Property(e => e.systemavatar).HasPrecision(9);
        });

        modelBuilder.Entity<board>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
        });

        modelBuilder.Entity<boardproject>(entity =>
        {
            entity.HasKey(e => new { e.BOARD_ID, e.PROJECT_ID })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasIndex(e => e.BOARD_ID, "idx_board_board_ids");

            entity.HasIndex(e => e.PROJECT_ID, "idx_board_project_ids");

            entity.Property(e => e.BOARD_ID).HasPrecision(18);
            entity.Property(e => e.PROJECT_ID).HasPrecision(18);
        });

        modelBuilder.Entity<changegroup>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.AUTHOR, e.CREATED }, "chggroup_author_created");

            entity.HasIndex(e => new { e.issueid, e.ID }, "chggroup_issue_id");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.issueid).HasPrecision(18);
        });

        modelBuilder.Entity<changeitem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.groupid, e.FIELD }, "chgitem_group_field");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDTYPE).HasMaxLength(255);
            entity.Property(e => e.groupid).HasPrecision(18);
        });

        modelBuilder.Entity<columnlayout>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.SEARCHREQUEST, "cl_searchrequest");

            entity.HasIndex(e => e.USERNAME, "cl_username");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.SEARCHREQUEST).HasPrecision(18);
        });

        modelBuilder.Entity<columnlayoutitem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDIDENTIFIER, "idx_cli_fieldidentifier");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.COLUMNLAYOUT).HasPrecision(18);
            entity.Property(e => e.HORIZONTALPOSITION).HasPrecision(18);
        });

        modelBuilder.Entity<comment_reaction>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.comment_id, "comment_reaction_comment_id");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.AUTHOR).HasMaxLength(255);
            entity.Property(e => e.EMOTICON).HasMaxLength(255);
            entity.Property(e => e.comment_id).HasPrecision(18);
            entity.Property(e => e.created_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<comment_version>(entity =>
        {
            entity.HasKey(e => e.COMMENT_ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.DELETED, e.UPDATE_TIME }, "cv_deleted_update_time_idx");

            entity.HasIndex(e => e.PARENT_ISSUE_ID, "cv_parent_id");

            entity.HasIndex(e => e.UPDATE_TIME, "cv_update_time");

            entity.Property(e => e.COMMENT_ID).HasPrecision(18);
            entity.Property(e => e.DELETED)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.INDEX_VERSION).HasPrecision(18);
            entity.Property(e => e.PARENT_ISSUE_ID).HasPrecision(18);
            entity.Property(e => e.UPDATE_TIME).HasColumnType("datetime");
        });

        modelBuilder.Entity<component>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.cname, "idx_component_name");

            entity.HasIndex(e => e.PROJECT, "idx_component_project");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ARCHIVED).HasMaxLength(10);
            entity.Property(e => e.ASSIGNEETYPE).HasPrecision(18);
            entity.Property(e => e.DELETED).HasMaxLength(10);
            entity.Property(e => e.LEAD).HasMaxLength(255);
            entity.Property(e => e.PROJECT).HasPrecision(18);
            entity.Property(e => e.URL).HasMaxLength(255);
            entity.Property(e => e.description).HasColumnType("mediumtext");
        });

        modelBuilder.Entity<configurationcontext>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.PROJECTCATEGORY, e.PROJECT, e.customfield }, "confcontext");

            entity.HasIndex(e => e.FIELDCONFIGSCHEME, "confcontextfieldconfigscheme");

            entity.HasIndex(e => e.customfield, "confcontextkey");

            entity.HasIndex(e => new { e.PROJECT, e.customfield }, "confcontextprojectkey");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDCONFIGSCHEME).HasPrecision(18);
            entity.Property(e => e.PROJECT).HasPrecision(18);
            entity.Property(e => e.PROJECTCATEGORY).HasPrecision(18);
        });

        modelBuilder.Entity<customfield>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CUSTOMFIELDSEARCHERKEY).HasMaxLength(255);
            entity.Property(e => e.CUSTOMFIELDTYPEKEY).HasMaxLength(255);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.FIELDTYPE).HasPrecision(18);
            entity.Property(e => e.ISSUETYPE).HasMaxLength(255);
            entity.Property(e => e.PROJECT).HasPrecision(18);
            entity.Property(e => e.cfkey).HasMaxLength(255);
            entity.Property(e => e.cfname).HasMaxLength(255);
            entity.Property(e => e.defaultvalue).HasMaxLength(255);
            entity.Property(e => e.issueswithvalue).HasPrecision(18);
            entity.Property(e => e.lastvalueupdate).HasColumnType("datetime");
        });

        modelBuilder.Entity<customfieldoption>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.CUSTOMFIELD, "cf_cfoption");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CUSTOMFIELD).HasPrecision(18);
            entity.Property(e => e.CUSTOMFIELDCONFIG).HasPrecision(18);
            entity.Property(e => e.PARENTOPTIONID).HasPrecision(18);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.customvalue).HasMaxLength(255);
            entity.Property(e => e.disabled).HasMaxLength(60);
            entity.Property(e => e.optiontype).HasMaxLength(60);
        });

        modelBuilder.Entity<customfieldvalue>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.ISSUE, e.CUSTOMFIELD }, "cfvalue_issue");

            entity.HasIndex(e => e.NUMBERVALUE, "customfieldvalue_number_value_type_key");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CUSTOMFIELD).HasPrecision(18);
            entity.Property(e => e.DATEVALUE).HasColumnType("datetime");
            entity.Property(e => e.ISSUE).HasPrecision(18);
            entity.Property(e => e.NUMBERVALUE).HasPrecision(18, 6);
            entity.Property(e => e.PARENTKEY).HasMaxLength(255);
            entity.Property(e => e.STRINGVALUE).HasMaxLength(255);
            entity.Property(e => e.UPDATED).HasPrecision(18);
            entity.Property(e => e.VALUETYPE).HasMaxLength(255);
        });

        modelBuilder.Entity<cwd_application>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.lower_application_name, "uk_application_name").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.active).HasPrecision(9);
            entity.Property(e => e.application_name).HasMaxLength(255);
            entity.Property(e => e.application_type).HasMaxLength(255);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.credential).HasMaxLength(255);
            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<cwd_application_address>(entity =>
        {
            entity.HasKey(e => new { e.application_id, e.remote_address })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.application_id).HasPrecision(18);
            entity.Property(e => e.encoded_address_binary).HasMaxLength(255);
            entity.Property(e => e.remote_address_mask).HasPrecision(9);
        });

        modelBuilder.Entity<cwd_directory>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.active, "idx_directory_active");

            entity.HasIndex(e => e.lower_impl_class, "idx_directory_impl");

            entity.HasIndex(e => e.directory_type, "idx_directory_type");

            entity.HasIndex(e => e.lower_directory_name, "uk_directory_name");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.active).HasPrecision(9);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.directory_name).HasMaxLength(255);
            entity.Property(e => e.directory_position).HasPrecision(18);
            entity.Property(e => e.directory_type).HasMaxLength(60);
            entity.Property(e => e.impl_class).HasMaxLength(255);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<cwd_directory_attribute>(entity =>
        {
            entity.HasKey(e => new { e.directory_id, e.attribute_name })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.directory_id).HasPrecision(18);
        });

        modelBuilder.Entity<cwd_directory_operation>(entity =>
        {
            entity.HasKey(e => new { e.directory_id, e.operation_type })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.directory_id).HasPrecision(18);
            entity.Property(e => e.operation_type).HasMaxLength(60);
        });

        modelBuilder.Entity<cwd_group>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.lower_group_name, e.active }, "idx_group_active");

            entity.HasIndex(e => e.directory_id, "idx_group_dir_id");

            entity.HasIndex(e => new { e.lower_group_name, e.directory_id }, "uk_group_name_dir_id").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.active).HasPrecision(9);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.description).HasMaxLength(255);
            entity.Property(e => e.directory_id).HasPrecision(18);
            entity.Property(e => e.external_id).HasMaxLength(255);
            entity.Property(e => e.group_name).HasMaxLength(255);
            entity.Property(e => e.group_type).HasMaxLength(60);
            entity.Property(e => e.local).HasPrecision(9);
            entity.Property(e => e.lower_description).HasMaxLength(255);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<cwd_group_attributes>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.directory_id, e.attribute_name, e.lower_attribute_value }, "idx_group_attr_dir_name_lval");

            entity.HasIndex(e => new { e.group_id, e.attribute_name, e.lower_attribute_value }, "uk_group_attr_name_lval").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.attribute_value).HasMaxLength(255);
            entity.Property(e => e.directory_id).HasPrecision(18);
            entity.Property(e => e.group_id).HasPrecision(18);
        });

        modelBuilder.Entity<cwd_membership>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.lower_child_name, e.membership_type, e.directory_id }, "idx_mem_dir_child");

            entity.HasIndex(e => new { e.lower_parent_name, e.membership_type, e.directory_id }, "idx_mem_dir_parent");

            entity.HasIndex(e => new { e.membership_type, e.lower_parent_name, e.lower_child_name }, "idx_mem_type_child_name");

            entity.HasIndex(e => new { e.lower_parent_name, e.lower_child_name, e.membership_type, e.directory_id }, "uk_mem_dir_parent_child").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.child_id).HasPrecision(18);
            entity.Property(e => e.child_name).HasMaxLength(255);
            entity.Property(e => e.directory_id).HasPrecision(18);
            entity.Property(e => e.group_type).HasMaxLength(60);
            entity.Property(e => e.membership_type).HasMaxLength(60);
            entity.Property(e => e.parent_id).HasPrecision(18);
            entity.Property(e => e.parent_name).HasMaxLength(255);
        });

        modelBuilder.Entity<cwd_synchronisation_status>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.directory_id, "idx_sync_status_dir");

            entity.HasIndex(e => e.sync_end, "idx_sync_status_end");

            entity.HasIndex(e => e.node_id, "idx_sync_status_node");

            entity.Property(e => e.id).HasPrecision(18);
            entity.Property(e => e.directory_id).HasPrecision(18);
            entity.Property(e => e.node_id).HasMaxLength(60);
            entity.Property(e => e.sync_end).HasPrecision(18);
            entity.Property(e => e.sync_start).HasPrecision(18);
            entity.Property(e => e.sync_status).HasMaxLength(60);
        });

        modelBuilder.Entity<cwd_synchronisation_token>(entity =>
        {
            entity.HasKey(e => e.directory_id).HasName("PRIMARY");

            entity.Property(e => e.directory_id).HasPrecision(18);
        });

        modelBuilder.Entity<cwd_user>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.lower_display_name, "idx_display_name");

            entity.HasIndex(e => e.lower_email_address, "idx_email_address");

            entity.HasIndex(e => e.lower_first_name, "idx_first_name");

            entity.HasIndex(e => e.lower_last_name, "idx_last_name");

            entity.HasIndex(e => new { e.EXTERNAL_ID, e.directory_id }, "uk_user_externalid_dir_id");

            entity.HasIndex(e => new { e.lower_user_name, e.directory_id }, "uk_user_name_dir_id").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CREDENTIAL).HasMaxLength(255);
            entity.Property(e => e.active).HasPrecision(9);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.deleted_externally).HasPrecision(9);
            entity.Property(e => e.directory_id).HasPrecision(18);
            entity.Property(e => e.display_name).HasMaxLength(255);
            entity.Property(e => e.email_address).HasMaxLength(255);
            entity.Property(e => e.first_name).HasMaxLength(255);
            entity.Property(e => e.last_name).HasMaxLength(255);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
            entity.Property(e => e.user_name).HasMaxLength(255);
        });

        modelBuilder.Entity<cwd_user_attributes>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.directory_id, e.attribute_name, e.lower_attribute_value }, "idx_user_attr_dir_name_lval");

            entity.HasIndex(e => new { e.user_id, e.attribute_name }, "uk_user_attr_name_lval");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.attribute_value).HasMaxLength(255);
            entity.Property(e => e.directory_id).HasPrecision(18);
            entity.Property(e => e.user_id).HasPrecision(18);
        });

        modelBuilder.Entity<deadletter>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.LAST_SEEN, "deadletter_lastSeen");

            entity.HasIndex(e => new { e.MESSAGE_ID, e.MAIL_SERVER_ID, e.FOLDER_NAME }, "deadletter_msg_server_folder");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.LAST_SEEN).HasPrecision(18);
            entity.Property(e => e.MAIL_SERVER_ID).HasPrecision(18);
        });

        modelBuilder.Entity<draftworkflowscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.WORKFLOW_SCHEME_ID, "draft_workflow_scheme_parent").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.LAST_MODIFIED_DATE).HasColumnType("datetime");
            entity.Property(e => e.LAST_MODIFIED_USER).HasMaxLength(255);
            entity.Property(e => e.NAME).HasMaxLength(255);
            entity.Property(e => e.WORKFLOW_SCHEME_ID).HasPrecision(18);
        });

        modelBuilder.Entity<draftworkflowschemeentity>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.SCHEME, "draft_workflow_scheme");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.SCHEME).HasPrecision(18);
            entity.Property(e => e.WORKFLOW).HasMaxLength(255);
            entity.Property(e => e.issuetype).HasMaxLength(255);
        });

        modelBuilder.Entity<entity_property>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.ENTITY_ID, e.ENTITY_NAME, e.PROPERTY_KEY }, "entityproperty_id_name_key");

            entity.HasIndex(e => new { e.PROPERTY_KEY, e.ENTITY_NAME }, "entityproperty_key_name");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.ENTITY_ID).HasPrecision(18);
            entity.Property(e => e.UPDATED).HasColumnType("datetime");
        });

        modelBuilder.Entity<entity_property_index_document>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.PLUGIN_KEY, e.MODULE_KEY }, "entpropindexdoc_module").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ENTITY_KEY).HasMaxLength(255);
            entity.Property(e => e.UPDATED).HasColumnType("datetime");
        });

        modelBuilder.Entity<entity_translation>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.LOCALE, "entitytranslation_locale");

            entity.HasIndex(e => new { e.ENTITY_NAME, e.ENTITY_ID, e.LOCALE }, "uk_entitytranslation").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ENTITY_ID).HasPrecision(18);
            entity.Property(e => e.LOCALE).HasMaxLength(60);
            entity.Property(e => e.TRANS_DESC).HasMaxLength(255);
            entity.Property(e => e.TRANS_NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<external_entities>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.NAME, "ext_entity_name");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.entitytype).HasMaxLength(255);
        });

        modelBuilder.Entity<externalgadget>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.GADGET_XML).HasColumnType("mediumtext");
        });

        modelBuilder.Entity<favouriteassociations>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.USERNAME, e.entitytype, e.entityid }, "favourite_index").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.entityid).HasPrecision(18);
            entity.Property(e => e.entitytype).HasMaxLength(60);
        });

        modelBuilder.Entity<feature>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.ID, e.USER_KEY }, "feature_id_userkey");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FEATURE_NAME).HasMaxLength(255);
            entity.Property(e => e.FEATURE_TYPE).HasMaxLength(10);
        });

        modelBuilder.Entity<fieldconfigscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDID, "fcs_fieldid");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CUSTOMFIELD).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.FIELDID).HasMaxLength(60);
            entity.Property(e => e.configname).HasMaxLength(255);
        });

        modelBuilder.Entity<fieldconfigschemeissuetype>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.ISSUETYPE, "fcs_issuetype");

            entity.HasIndex(e => e.FIELDCONFIGSCHEME, "fcs_scheme");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDCONFIGSCHEME).HasPrecision(18);
            entity.Property(e => e.FIELDCONFIGURATION).HasPrecision(18);
        });

        modelBuilder.Entity<fieldconfiguration>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDID, "fc_fieldid");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CUSTOMFIELD).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.FIELDID).HasMaxLength(60);
            entity.Property(e => e.configname).HasMaxLength(255);
        });

        modelBuilder.Entity<fieldlayout>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasMaxLength(255);
            entity.Property(e => e.LAYOUTSCHEME).HasPrecision(18);
            entity.Property(e => e.NAME).HasMaxLength(255);
            entity.Property(e => e.layout_type).HasMaxLength(255);
        });

        modelBuilder.Entity<fieldlayoutitem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDIDENTIFIER, "idx_fli_fieldidentifier");

            entity.HasIndex(e => e.FIELDLAYOUT, "idx_fli_fieldlayout");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.FIELDLAYOUT).HasPrecision(18);
            entity.Property(e => e.ISHIDDEN).HasMaxLength(60);
            entity.Property(e => e.ISREQUIRED).HasMaxLength(60);
            entity.Property(e => e.RENDERERTYPE).HasMaxLength(255);
            entity.Property(e => e.VERTICALPOSITION).HasPrecision(18);
        });

        modelBuilder.Entity<fieldlayoutscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<fieldlayoutschemeassociation>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.PROJECT, e.ISSUETYPE }, "fl_scheme_assoc");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDLAYOUTSCHEME).HasPrecision(18);
            entity.Property(e => e.PROJECT).HasPrecision(18);
        });

        modelBuilder.Entity<fieldlayoutschemeentity>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDLAYOUT, "fieldlayout_layout");

            entity.HasIndex(e => e.SCHEME, "fieldlayout_scheme");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDLAYOUT).HasPrecision(18);
            entity.Property(e => e.SCHEME).HasPrecision(18);
            entity.Property(e => e.issuetype).HasMaxLength(255);
        });

        modelBuilder.Entity<fieldscreen>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasMaxLength(255);
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<fieldscreenlayoutitem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDSCREENTAB, "fieldscitem_tab");

            entity.HasIndex(e => e.FIELDIDENTIFIER, "fieldscreen_field");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDSCREENTAB).HasPrecision(18);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
        });

        modelBuilder.Entity<fieldscreenscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasMaxLength(255);
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<fieldscreenschemeitem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDSCREENSCHEME, "screenitem_scheme");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDSCREEN).HasPrecision(18);
            entity.Property(e => e.FIELDSCREENSCHEME).HasPrecision(18);
            entity.Property(e => e.OPERATION).HasPrecision(18);
        });

        modelBuilder.Entity<fieldscreentab>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDSCREEN, "fieldscreen_tab");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasMaxLength(255);
            entity.Property(e => e.FIELDSCREEN).HasPrecision(18);
            entity.Property(e => e.NAME).HasMaxLength(255);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
        });

        modelBuilder.Entity<fileattachment>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.issueid, "attach_issue");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.AUTHOR).HasMaxLength(255);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.FILENAME).HasMaxLength(255);
            entity.Property(e => e.FILESIZE).HasPrecision(18);
            entity.Property(e => e.MIMETYPE).HasMaxLength(255);
            entity.Property(e => e.issueid).HasPrecision(18);
            entity.Property(e => e.thumbnailable).HasPrecision(9);
            entity.Property(e => e.zip).HasPrecision(9);
        });

        modelBuilder.Entity<filtersubscription>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.FILTER_I_D, e.USERNAME }, "subscrpt_user");

            entity.HasIndex(e => new { e.FILTER_I_D, e.groupname }, "subscrptn_group");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.EMAIL_ON_EMPTY).HasMaxLength(10);
            entity.Property(e => e.FILTER_I_D).HasPrecision(18);
            entity.Property(e => e.LAST_RUN).HasColumnType("datetime");
            entity.Property(e => e.USERNAME).HasMaxLength(60);
            entity.Property(e => e.groupname).HasMaxLength(60);
        });

        modelBuilder.Entity<gadgetuserpreference>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.PORTLETCONFIGURATION, "userpref_portletconfiguration");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PORTLETCONFIGURATION).HasPrecision(18);
            entity.Property(e => e.USERPREFKEY).HasMaxLength(255);
        });

        modelBuilder.Entity<genericconfiguration>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.DATATYPE, e.DATAKEY }, "type_key").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DATAKEY).HasMaxLength(60);
            entity.Property(e => e.DATATYPE).HasMaxLength(60);
            entity.Property(e => e.XMLVALUE).HasColumnType("mediumtext");
        });

        modelBuilder.Entity<globalpermissionentry>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.GROUP_ID).HasMaxLength(255);
            entity.Property(e => e.PERMISSION).HasMaxLength(255);
        });

        modelBuilder.Entity<groupbase>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.groupname, "osgroup_name");

            entity.Property(e => e.ID).HasPrecision(18);
        });

        modelBuilder.Entity<issue_field_option>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELD_KEY).HasMaxLength(255);
            entity.Property(e => e.OPTION_ID).HasPrecision(18);
            entity.Property(e => e.PROPERTIES).HasColumnType("mediumtext");
            entity.Property(e => e.option_value).HasMaxLength(255);
        });

        modelBuilder.Entity<issue_field_option_scope>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ENTITY_ID).HasMaxLength(255);
            entity.Property(e => e.OPTION_ID).HasPrecision(18);
            entity.Property(e => e.SCOPE_TYPE).HasMaxLength(255);
        });

        modelBuilder.Entity<issue_version>(entity =>
        {
            entity.HasKey(e => e.ISSUE_ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.DELETED, e.UPDATE_TIME }, "iv_deleted_update_time_idx");

            entity.HasIndex(e => e.PARENT_ISSUE_ID, "iv_parent_id");

            entity.HasIndex(e => e.UPDATE_TIME, "iv_update_time");

            entity.Property(e => e.ISSUE_ID).HasPrecision(18);
            entity.Property(e => e.DELETED)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.INDEX_VERSION).HasPrecision(18);
            entity.Property(e => e.PARENT_ISSUE_ID).HasPrecision(18);
            entity.Property(e => e.UPDATE_TIME).HasColumnType("datetime");
        });

        modelBuilder.Entity<issuelink>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.DESTINATION, "issuelink_dest");

            entity.HasIndex(e => e.SOURCE, "issuelink_src");

            entity.HasIndex(e => e.LINKTYPE, "issuelink_type");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESTINATION).HasPrecision(18);
            entity.Property(e => e.LINKTYPE).HasPrecision(18);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.SOURCE).HasPrecision(18);
        });

        modelBuilder.Entity<issuelinktype>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.LINKNAME, "linktypename");

            entity.HasIndex(e => e.pstyle, "linktypestyle");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.INWARD).HasMaxLength(255);
            entity.Property(e => e.OUTWARD).HasMaxLength(255);
            entity.Property(e => e.pstyle).HasMaxLength(60);
        });

        modelBuilder.Entity<issuesecurityscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DEFAULTLEVEL).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<issuestatus>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasMaxLength(60);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.ICONURL).HasMaxLength(255);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.STATUSCATEGORY).HasPrecision(18);
            entity.Property(e => e.pname).HasMaxLength(60);
        });

        modelBuilder.Entity<issuetype>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasMaxLength(60);
            entity.Property(e => e.AVATAR).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.ICONURL).HasMaxLength(255);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.pname).HasMaxLength(60);
            entity.Property(e => e.pstyle).HasMaxLength(60);
        });

        modelBuilder.Entity<issuetypescreenscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasMaxLength(255);
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<issuetypescreenschemeentity>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.FIELDSCREENSCHEME, "fieldscreen_scheme");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDSCREENSCHEME).HasPrecision(18);
            entity.Property(e => e.ISSUETYPE).HasMaxLength(255);
            entity.Property(e => e.SCHEME).HasPrecision(18);
        });

        modelBuilder.Entity<jiraaction>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.AUTHOR, e.CREATED }, "action_author_created");

            entity.HasIndex(e => e.issueid, "action_issue");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.UPDATEAUTHOR).HasMaxLength(255);
            entity.Property(e => e.UPDATED).HasColumnType("datetime");
            entity.Property(e => e.actionlevel).HasMaxLength(255);
            entity.Property(e => e.actionnum).HasPrecision(18);
            entity.Property(e => e.actiontype).HasMaxLength(255);
            entity.Property(e => e.issueid).HasPrecision(18);
            entity.Property(e => e.rolelevel).HasPrecision(18);
        });

        modelBuilder.Entity<jiradraftworkflows>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PARENTNAME).HasMaxLength(255);
        });

        modelBuilder.Entity<jiraeventtype>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
            entity.Property(e => e.TEMPLATE_ID).HasPrecision(18);
            entity.Property(e => e.event_type).HasMaxLength(60);
        });

        modelBuilder.Entity<jiraissue>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.ARCHIVED, "issue_archived");

            entity.HasIndex(e => e.ASSIGNEE, "issue_assignee");

            entity.HasIndex(e => e.CREATED, "issue_created");

            entity.HasIndex(e => e.DUEDATE, "issue_duedate");

            entity.HasIndex(e => new { e.issuenum, e.PROJECT }, "issue_proj_num");

            entity.HasIndex(e => new { e.PROJECT, e.issuestatus }, "issue_proj_status");

            entity.HasIndex(e => e.REPORTER, "issue_reporter");

            entity.HasIndex(e => e.RESOLUTIONDATE, "issue_resolutiondate");

            entity.HasIndex(e => e.UPDATED, "issue_updated");

            entity.HasIndex(e => e.VOTES, "issue_votes");

            entity.HasIndex(e => e.WATCHES, "issue_watches");

            entity.HasIndex(e => e.WORKFLOW_ID, "issue_workflow");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ARCHIVED)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.ARCHIVEDBY).HasMaxLength(255);
            entity.Property(e => e.ARCHIVEDDATE).HasColumnType("datetime");
            entity.Property(e => e.COMPONENT).HasPrecision(18);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.CREATOR).HasMaxLength(255);
            entity.Property(e => e.DUEDATE).HasColumnType("datetime");
            entity.Property(e => e.FIXFOR).HasPrecision(18);
            entity.Property(e => e.PRIORITY).HasMaxLength(255);
            entity.Property(e => e.PROJECT).HasPrecision(18);
            entity.Property(e => e.RESOLUTION).HasMaxLength(255);
            entity.Property(e => e.RESOLUTIONDATE).HasColumnType("datetime");
            entity.Property(e => e.SECURITY).HasPrecision(18);
            entity.Property(e => e.SUMMARY).HasMaxLength(255);
            entity.Property(e => e.TIMEESTIMATE).HasPrecision(18);
            entity.Property(e => e.TIMEORIGINALESTIMATE).HasPrecision(18);
            entity.Property(e => e.TIMESPENT).HasPrecision(18);
            entity.Property(e => e.UPDATED).HasColumnType("datetime");
            entity.Property(e => e.VOTES).HasPrecision(18);
            entity.Property(e => e.WATCHES).HasPrecision(18);
            entity.Property(e => e.WORKFLOW_ID).HasPrecision(18);
            entity.Property(e => e.issuenum).HasPrecision(18);
            entity.Property(e => e.issuetype).HasMaxLength(255);
            entity.Property(e => e.pkey).HasMaxLength(255);
        });

        modelBuilder.Entity<jiraperms>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.groupname).HasMaxLength(255);
            entity.Property(e => e.permtype).HasPrecision(18);
            entity.Property(e => e.projectid).HasPrecision(18);
        });

        modelBuilder.Entity<jiraworkflows>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ISLOCKED).HasMaxLength(60);
            entity.Property(e => e.creatorname).HasMaxLength(255);
            entity.Property(e => e.workflowname).HasMaxLength(255);
        });

        modelBuilder.Entity<jiraworkflowstatuses>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.parentname, "idx_parent_name");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.status).HasMaxLength(255);
        });

        modelBuilder.Entity<label>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.ISSUE, e.FIELDID }, "label_fieldissue");

            entity.HasIndex(e => new { e.ISSUE, e.FIELDID, e.LABEL1 }, "label_fieldissuelabel");

            entity.HasIndex(e => e.ISSUE, "label_issue");

            entity.HasIndex(e => e.LABEL1, "label_label");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDID).HasPrecision(18);
            entity.Property(e => e.ISSUE).HasPrecision(18);
            entity.Property(e => e.LABEL1).HasColumnName("LABEL");
        });

        modelBuilder.Entity<licenserolesdefault>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.LICENSE_ROLE_NAME, "licenseroledefault_index").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
        });

        modelBuilder.Entity<licenserolesgroup>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.LICENSE_ROLE_NAME, e.GROUP_ID }, "licenserolegroup_index").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PRIMARY_GROUP)
                .HasMaxLength(1)
                .IsFixedLength();
        });

        modelBuilder.Entity<listenerconfig>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CLAZZ).HasMaxLength(255);
            entity.Property(e => e.listenername).HasMaxLength(255);
        });

        modelBuilder.Entity<mailserver>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.ISTLSREQUIRED).HasMaxLength(60);
            entity.Property(e => e.JNDILOCATION).HasMaxLength(255);
            entity.Property(e => e.NAME).HasMaxLength(255);
            entity.Property(e => e.PREFIX).HasMaxLength(60);
            entity.Property(e => e.SERVERNAME).HasMaxLength(255);
            entity.Property(e => e.TIMEOUT).HasPrecision(18);
            entity.Property(e => e.cipher_type).HasMaxLength(255);
            entity.Property(e => e.mailfrom).HasMaxLength(255);
            entity.Property(e => e.mailpassword).HasMaxLength(255);
            entity.Property(e => e.mailusername).HasMaxLength(255);
            entity.Property(e => e.protocol).HasMaxLength(60);
            entity.Property(e => e.server_type).HasMaxLength(60);
            entity.Property(e => e.smtp_port).HasMaxLength(60);
            entity.Property(e => e.socks_host).HasMaxLength(60);
            entity.Property(e => e.socks_port).HasMaxLength(60);
        });

        modelBuilder.Entity<managedconfigurationitem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.ITEM_ID, e.ITEM_TYPE }, "managedconfigitem_id_type_idx");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ACCESS_LEVEL).HasMaxLength(255);
            entity.Property(e => e.DESCRIPTION_KEY).HasMaxLength(255);
            entity.Property(e => e.MANAGED).HasMaxLength(10);
            entity.Property(e => e.SOURCE).HasMaxLength(255);
        });

        modelBuilder.Entity<membershipbase>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.GROUP_NAME, "mshipbase_group");

            entity.HasIndex(e => e.USER_NAME, "mshipbase_user");

            entity.Property(e => e.ID).HasPrecision(18);
        });

        modelBuilder.Entity<moved_issue_key>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.OLD_ISSUE_KEY, "idx_old_issue_key").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ISSUE_ID).HasPrecision(18);
        });

        modelBuilder.Entity<nodeassociation>(entity =>
        {
            entity.HasKey(e => new { e.SOURCE_NODE_ID, e.SOURCE_NODE_ENTITY, e.SINK_NODE_ID, e.SINK_NODE_ENTITY, e.ASSOCIATION_TYPE })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0, 0 });

            entity.HasIndex(e => new { e.SINK_NODE_ID, e.SINK_NODE_ENTITY }, "node_sink");

            entity.HasIndex(e => new { e.SOURCE_NODE_ID, e.SOURCE_NODE_ENTITY }, "node_source");

            entity.Property(e => e.SOURCE_NODE_ID).HasPrecision(18);
            entity.Property(e => e.SOURCE_NODE_ENTITY).HasMaxLength(60);
            entity.Property(e => e.SINK_NODE_ID).HasPrecision(18);
            entity.Property(e => e.SINK_NODE_ENTITY).HasMaxLength(60);
            entity.Property(e => e.ASSOCIATION_TYPE).HasMaxLength(60);
            entity.Property(e => e.SEQUENCE).HasPrecision(9);
        });

        modelBuilder.Entity<nodeindexcounter>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.NODE_ID, e.SENDING_NODE_ID }, "node_id_idx").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.INDEX_OPERATION_ID).HasPrecision(18);
            entity.Property(e => e.NODE_ID).HasMaxLength(60);
            entity.Property(e => e.SENDING_NODE_ID).HasMaxLength(60);
        });

        modelBuilder.Entity<nomenclature_entries>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.NEW_NAME).HasMaxLength(255);
            entity.Property(e => e.NEW_NAME_PLURAL).HasMaxLength(255);
            entity.Property(e => e.ORIGINAL_NAME).HasMaxLength(255);
            entity.Property(e => e.TIMESTAMP).HasPrecision(18);
        });

        modelBuilder.Entity<notification>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.SCHEME, "ntfctn_scheme");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.EVENT).HasMaxLength(60);
            entity.Property(e => e.EVENT_TYPE_ID).HasPrecision(18);
            entity.Property(e => e.SCHEME).HasPrecision(18);
            entity.Property(e => e.TEMPLATE_ID).HasPrecision(18);
            entity.Property(e => e.notif_parameter).HasMaxLength(60);
            entity.Property(e => e.notif_type).HasMaxLength(60);
        });

        modelBuilder.Entity<notificationinstance>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.MESSAGEID, "notif_messageid");

            entity.HasIndex(e => e.SOURCE, "notif_source");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.SOURCE).HasPrecision(18);
            entity.Property(e => e.emailaddress).HasMaxLength(255);
            entity.Property(e => e.notificationtype).HasMaxLength(60);
        });

        modelBuilder.Entity<notificationscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<oauthconsumer>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.CONSUMER_KEY, "oauth_consumer_index").IsUnique();

            entity.HasIndex(e => e.consumerservice, "oauth_consumer_service_index").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CALLBACK).HasColumnType("mediumtext");
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.PRIVATE_KEY).HasColumnType("mediumtext");
            entity.Property(e => e.PUBLIC_KEY).HasColumnType("mediumtext");
            entity.Property(e => e.SHARED_SECRET).HasColumnType("mediumtext");
            entity.Property(e => e.SIGNATURE_METHOD).HasMaxLength(60);
            entity.Property(e => e.consumername).HasMaxLength(255);
        });

        modelBuilder.Entity<oauthconsumertoken>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.TOKEN, "oauth_consumer_token_index");

            entity.HasIndex(e => e.TOKEN_KEY, "oauth_consumer_token_key_index").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CONSUMER_KEY).HasMaxLength(255);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.TOKEN_SECRET).HasMaxLength(255);
            entity.Property(e => e.TOKEN_TYPE).HasMaxLength(60);
        });

        modelBuilder.Entity<oauthspconsumer>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.CONSUMER_KEY, "oauth_sp_consumer_index").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CALLBACK).HasColumnType("mediumtext");
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.EXECUTING_TWO_L_O_USER).HasMaxLength(255);
            entity.Property(e => e.PUBLIC_KEY).HasColumnType("mediumtext");
            entity.Property(e => e.THREE_L_O_ALLOWED).HasMaxLength(60);
            entity.Property(e => e.TWO_L_O_ALLOWED).HasMaxLength(60);
            entity.Property(e => e.TWO_L_O_IMPERSONATION_ALLOWED).HasMaxLength(60);
            entity.Property(e => e.consumername).HasMaxLength(255);
        });

        modelBuilder.Entity<oauthsptoken>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.CONSUMER_KEY, "oauth_sp_consumer_key_index");

            entity.HasIndex(e => e.TOKEN, "oauth_sp_token_index").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CALLBACK).HasColumnType("mediumtext");
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.SESSION_CREATION_TIME).HasColumnType("datetime");
            entity.Property(e => e.SESSION_HANDLE).HasMaxLength(255);
            entity.Property(e => e.SESSION_LAST_RENEWAL_TIME).HasColumnType("datetime");
            entity.Property(e => e.SESSION_TIME_TO_LIVE).HasColumnType("datetime");
            entity.Property(e => e.TOKEN_SECRET).HasMaxLength(255);
            entity.Property(e => e.TOKEN_TYPE).HasMaxLength(60);
            entity.Property(e => e.TTL).HasPrecision(18);
            entity.Property(e => e.USERNAME).HasMaxLength(255);
            entity.Property(e => e.spauth).HasMaxLength(60);
            entity.Property(e => e.spverifier).HasMaxLength(255);
            entity.Property(e => e.spversion).HasMaxLength(60);
        });

        modelBuilder.Entity<optionconfiguration>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.FIELDID, e.FIELDCONFIG }, "fieldid_fieldconf");

            entity.HasIndex(e => new { e.FIELDID, e.OPTIONID }, "fieldid_optionid");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.FIELDCONFIG).HasPrecision(18);
            entity.Property(e => e.FIELDID).HasMaxLength(60);
            entity.Property(e => e.OPTIONID).HasMaxLength(60);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
        });

        modelBuilder.Entity<os_currentstep>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.ENTRY_ID, "wf_entryid");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ACTION_ID).HasPrecision(9);
            entity.Property(e => e.CALLER).HasMaxLength(255);
            entity.Property(e => e.DUE_DATE).HasColumnType("datetime");
            entity.Property(e => e.ENTRY_ID).HasPrecision(18);
            entity.Property(e => e.FINISH_DATE).HasColumnType("datetime");
            entity.Property(e => e.OWNER).HasMaxLength(255);
            entity.Property(e => e.START_DATE).HasColumnType("datetime");
            entity.Property(e => e.STATUS).HasMaxLength(60);
            entity.Property(e => e.STEP_ID).HasPrecision(9);
        });

        modelBuilder.Entity<os_currentstep_prev>(entity =>
        {
            entity.HasKey(e => new { e.ID, e.PREVIOUS_ID })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PREVIOUS_ID).HasPrecision(18);
        });

        modelBuilder.Entity<os_historystep>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.ENTRY_ID, "historystep_entryid");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ACTION_ID).HasPrecision(9);
            entity.Property(e => e.CALLER).HasMaxLength(255);
            entity.Property(e => e.DUE_DATE).HasColumnType("datetime");
            entity.Property(e => e.ENTRY_ID).HasPrecision(18);
            entity.Property(e => e.FINISH_DATE).HasColumnType("datetime");
            entity.Property(e => e.OWNER).HasMaxLength(255);
            entity.Property(e => e.START_DATE).HasColumnType("datetime");
            entity.Property(e => e.STATUS).HasMaxLength(60);
            entity.Property(e => e.STEP_ID).HasPrecision(9);
        });

        modelBuilder.Entity<os_historystep_prev>(entity =>
        {
            entity.HasKey(e => new { e.ID, e.PREVIOUS_ID })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PREVIOUS_ID).HasPrecision(18);
        });

        modelBuilder.Entity<os_wfentry>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.INITIALIZED).HasPrecision(9);
            entity.Property(e => e.NAME).HasMaxLength(255);
            entity.Property(e => e.STATE).HasPrecision(9);
        });

        modelBuilder.Entity<permissionscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<permissionschemeattribute>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.SCHEME, "prmssn_scheme_attr_idx");

            entity.HasIndex(e => new { e.SCHEME, e.ATTRIBUTE_KEY }, "prmssn_scheme_attr_key_idx");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ATTRIBUTE_VALUE).HasColumnType("mediumtext");
            entity.Property(e => e.SCHEME).HasPrecision(18);
        });

        modelBuilder.Entity<pluginstate>(entity =>
        {
            entity.HasKey(e => e.pluginkey).HasName("PRIMARY");

            entity.Property(e => e.pluginenabled).HasMaxLength(60);
        });

        modelBuilder.Entity<pluginversion>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.pluginkey).HasMaxLength(255);
            entity.Property(e => e.pluginname).HasMaxLength(255);
            entity.Property(e => e.pluginversion1)
                .HasMaxLength(255)
                .HasColumnName("pluginversion");
        });

        modelBuilder.Entity<portalpage>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.USERNAME, "ppage_username");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasMaxLength(255);
            entity.Property(e => e.FAV_COUNT).HasPrecision(18);
            entity.Property(e => e.LAYOUT).HasMaxLength(255);
            entity.Property(e => e.PAGENAME).HasMaxLength(255);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.ppversion).HasPrecision(18);
        });

        modelBuilder.Entity<portletconfiguration>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.COLOR).HasMaxLength(255);
            entity.Property(e => e.COLUMN_NUMBER).HasPrecision(9);
            entity.Property(e => e.DASHBOARD_MODULE_COMPLETE_KEY).HasColumnType("mediumtext");
            entity.Property(e => e.GADGET_XML).HasColumnType("mediumtext");
            entity.Property(e => e.PORTALPAGE).HasPrecision(18);
            entity.Property(e => e.PORTLET_ID).HasMaxLength(255);
            entity.Property(e => e.positionseq).HasPrecision(9);
        });

        modelBuilder.Entity<priority>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasMaxLength(60);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.ICONURL).HasMaxLength(255);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.STATUS_COLOR).HasMaxLength(60);
            entity.Property(e => e.pname).HasMaxLength(60);
        });

        modelBuilder.Entity<productlicense>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
        });

        modelBuilder.Entity<project>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.pkey, "idx_project_key").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ASSIGNEETYPE).HasPrecision(18);
            entity.Property(e => e.AVATAR).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.LEAD).HasMaxLength(255);
            entity.Property(e => e.ORIGINALKEY).HasMaxLength(255);
            entity.Property(e => e.PROJECTTYPE).HasMaxLength(255);
            entity.Property(e => e.URL).HasMaxLength(255);
            entity.Property(e => e.pcounter).HasPrecision(18);
            entity.Property(e => e.pname).HasMaxLength(255);
        });

        modelBuilder.Entity<project_key>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.PROJECT_ID, "idx_all_project_ids");

            entity.HasIndex(e => e.PROJECT_KEY1, "idx_all_project_keys").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PROJECT_ID).HasPrecision(18);
            entity.Property(e => e.PROJECT_KEY1).HasColumnName("PROJECT_KEY");
        });

        modelBuilder.Entity<projectcategory>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.cname, "idx_project_category_name");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.description).HasColumnType("mediumtext");
        });

        modelBuilder.Entity<projectchangedtime>(entity =>
        {
            entity.HasKey(e => e.PROJECT_ID).HasName("PRIMARY");

            entity.Property(e => e.PROJECT_ID).HasPrecision(18);
            entity.Property(e => e.ISSUE_CHANGED_TIME).HasColumnType("datetime");
        });

        modelBuilder.Entity<projectrole>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<projectroleactor>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.PID, "role_pid_idx");

            entity.HasIndex(e => new { e.PROJECTROLEID, e.PID }, "role_player_idx");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PID).HasPrecision(18);
            entity.Property(e => e.PROJECTROLEID).HasPrecision(18);
            entity.Property(e => e.ROLETYPE).HasMaxLength(255);
            entity.Property(e => e.ROLETYPEPARAMETER).HasMaxLength(255);
        });

        modelBuilder.Entity<projectversion>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.PROJECT, "idx_version_project");

            entity.HasIndex(e => e.SEQUENCE, "idx_version_sequence");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ARCHIVED).HasMaxLength(10);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.PROJECT).HasPrecision(18);
            entity.Property(e => e.RELEASED).HasMaxLength(10);
            entity.Property(e => e.RELEASEDATE).HasColumnType("datetime");
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.STARTDATE).HasColumnType("datetime");
            entity.Property(e => e.URL).HasMaxLength(255);
            entity.Property(e => e.vname).HasMaxLength(255);
        });

        modelBuilder.Entity<propertydata>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.propertyvalue).HasColumnType("blob");
        });

        modelBuilder.Entity<propertydate>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.propertyvalue).HasColumnType("datetime");
        });

        modelBuilder.Entity<propertydecimal>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.propertyvalue).HasPrecision(18, 6);
        });

        modelBuilder.Entity<propertyentry>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.ENTITY_ID, e.ENTITY_NAME, e.PROPERTY_KEY }, "osproperty_entId_name_propKey");

            entity.HasIndex(e => e.PROPERTY_KEY, "osproperty_propertyKey");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ENTITY_ID).HasPrecision(18);
            entity.Property(e => e.propertytype).HasPrecision(9);
        });

        modelBuilder.Entity<propertynumber>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.propertyvalue).HasPrecision(18);
        });

        modelBuilder.Entity<propertystring>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.propertyvalue).HasColumnType("mediumtext");
        });

        modelBuilder.Entity<propertytext>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
        });

        modelBuilder.Entity<reindex_component>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.REQUEST_ID, "idx_reindex_component_req_id");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.AFFECTED_INDEX).HasMaxLength(60);
            entity.Property(e => e.ENTITY_TYPE).HasMaxLength(60);
            entity.Property(e => e.REQUEST_ID).HasPrecision(18);
        });

        modelBuilder.Entity<reindex_request>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.COMPLETION_TIME).HasColumnType("datetime");
            entity.Property(e => e.EXECUTION_NODE_ID).HasMaxLength(60);
            entity.Property(e => e.QUERY).HasColumnType("mediumtext");
            entity.Property(e => e.REQUEST_TIME).HasColumnType("datetime");
            entity.Property(e => e.START_TIME).HasColumnType("datetime");
            entity.Property(e => e.STATUS).HasMaxLength(60);
            entity.Property(e => e.TYPE).HasMaxLength(60);
        });

        modelBuilder.Entity<remembermetoken>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.USERNAME, "remembermetoken_username_index");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.TOKEN).HasMaxLength(255);
        });

        modelBuilder.Entity<remotelink>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.GLOBALID, "remotelink_globalid");

            entity.HasIndex(e => new { e.ISSUEID, e.GLOBALID }, "remotelink_issueid");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.APPLICATIONNAME).HasMaxLength(255);
            entity.Property(e => e.APPLICATIONTYPE).HasMaxLength(255);
            entity.Property(e => e.ICONTITLE).HasColumnType("mediumtext");
            entity.Property(e => e.ICONURL).HasColumnType("mediumtext");
            entity.Property(e => e.ISSUEID).HasPrecision(18);
            entity.Property(e => e.RELATIONSHIP).HasMaxLength(255);
            entity.Property(e => e.RESOLVED)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.STATUSCATEGORYCOLORNAME).HasMaxLength(255);
            entity.Property(e => e.STATUSCATEGORYKEY).HasMaxLength(255);
            entity.Property(e => e.STATUSDESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.STATUSICONLINK).HasColumnType("mediumtext");
            entity.Property(e => e.STATUSICONTITLE).HasColumnType("mediumtext");
            entity.Property(e => e.STATUSICONURL).HasColumnType("mediumtext");
            entity.Property(e => e.STATUSNAME).HasMaxLength(255);
            entity.Property(e => e.SUMMARY).HasColumnType("mediumtext");
            entity.Property(e => e.TITLE).HasMaxLength(255);
            entity.Property(e => e.URL).HasColumnType("mediumtext");
        });

        modelBuilder.Entity<replicatedindexoperation>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.NODE_ID, e.AFFECTED_INDEX, e.OPERATION, e.INDEX_TIME }, "node_operation_idx");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.AFFECTED_INDEX).HasMaxLength(60);
            entity.Property(e => e.ENTITY_TYPE).HasMaxLength(60);
            entity.Property(e => e.FILENAME).HasMaxLength(255);
            entity.Property(e => e.INDEX_TIME).HasColumnType("datetime");
            entity.Property(e => e.NODE_ID).HasMaxLength(60);
            entity.Property(e => e.OPERATION).HasMaxLength(60);
        });

        modelBuilder.Entity<resolution>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasMaxLength(60);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.ICONURL).HasMaxLength(255);
            entity.Property(e => e.SEQUENCE).HasPrecision(18);
            entity.Property(e => e.pname).HasMaxLength(60);
        });

        modelBuilder.Entity<rundetails>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.JOB_ID, "rundetails_jobid_idx");

            entity.HasIndex(e => e.START_TIME, "rundetails_starttime_idx");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.INFO_MESSAGE).HasMaxLength(255);
            entity.Property(e => e.RUN_DURATION).HasPrecision(18);
            entity.Property(e => e.RUN_OUTCOME)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.START_TIME).HasColumnType("datetime");
        });

        modelBuilder.Entity<schemeissuesecurities>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.SCHEME, "sec_scheme");

            entity.HasIndex(e => e.SECURITY, "sec_security");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.SCHEME).HasPrecision(18);
            entity.Property(e => e.SECURITY).HasPrecision(18);
            entity.Property(e => e.sec_parameter).HasMaxLength(255);
            entity.Property(e => e.sec_type).HasMaxLength(255);
        });

        modelBuilder.Entity<schemeissuesecuritylevels>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
            entity.Property(e => e.SCHEME).HasPrecision(18);
        });

        modelBuilder.Entity<schemepermissions>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.PERMISSION_KEY, "permission_key_idx");

            entity.HasIndex(e => e.SCHEME, "prmssn_scheme");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PERMISSION).HasPrecision(18);
            entity.Property(e => e.SCHEME).HasPrecision(18);
            entity.Property(e => e.perm_parameter).HasMaxLength(255);
            entity.Property(e => e.perm_type).HasMaxLength(255);
        });

        modelBuilder.Entity<searchrequest>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.filtername_lower, "searchrequest_filternameLower");

            entity.HasIndex(e => e.authorname, "sr_author");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.FAV_COUNT).HasPrecision(18);
            entity.Property(e => e.filtername).HasMaxLength(255);
            entity.Property(e => e.groupname).HasMaxLength(255);
            entity.Property(e => e.projectid).HasPrecision(18);
            entity.Property(e => e.username).HasMaxLength(255);
        });

        modelBuilder.Entity<securityproperty>(entity =>
        {
            entity.HasKey(e => e.PROPERTY_KEY).HasName("PRIMARY");

            entity.Property(e => e.propertyvalue).HasColumnType("text");
        });

        modelBuilder.Entity<sequence_value_item>(entity =>
        {
            entity.HasKey(e => e.SEQ_NAME).HasName("PRIMARY");

            entity.Property(e => e.SEQ_NAME).HasMaxLength(60);
            entity.Property(e => e.SEQ_ID).HasPrecision(18);
        });

        modelBuilder.Entity<serviceconfig>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CLAZZ).HasMaxLength(255);
            entity.Property(e => e.CRON_EXPRESSION).HasMaxLength(255);
            entity.Property(e => e.delaytime).HasPrecision(18);
            entity.Property(e => e.servicename).HasMaxLength(255);
        });

        modelBuilder.Entity<sharepermissions>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.entityid, e.entitytype }, "share_index");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PARAM1).HasMaxLength(255);
            entity.Property(e => e.PARAM2).HasMaxLength(60);
            entity.Property(e => e.RIGHTS).HasPrecision(9);
            entity.Property(e => e.entityid).HasPrecision(18);
            entity.Property(e => e.entitytype).HasMaxLength(60);
            entity.Property(e => e.sharetype).HasMaxLength(10);
        });

        modelBuilder.Entity<tempattachmentsmonitor>(entity =>
        {
            entity.HasKey(e => e.TEMPORARY_ATTACHMENT_ID).HasName("PRIMARY");

            entity.HasIndex(e => e.CREATED_TIME, "idx_tam_by_created_time");

            entity.HasIndex(e => e.FORM_TOKEN, "idx_tam_by_form_token");

            entity.Property(e => e.CONTENT_TYPE).HasMaxLength(255);
            entity.Property(e => e.CREATED_TIME).HasPrecision(18);
            entity.Property(e => e.FILE_NAME).HasMaxLength(255);
            entity.Property(e => e.FILE_SIZE).HasPrecision(18);
        });

        modelBuilder.Entity<trackback_ping>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.BLOGNAME).HasMaxLength(255);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.EXCERPT).HasMaxLength(255);
            entity.Property(e => e.ISSUE).HasPrecision(18);
            entity.Property(e => e.TITLE).HasMaxLength(255);
            entity.Property(e => e.URL).HasMaxLength(255);
        });

        modelBuilder.Entity<trustedapp>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.APPLICATION_ID, "trustedapp_id").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.CREATED_BY).HasMaxLength(255);
            entity.Property(e => e.IP_MATCH).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
            entity.Property(e => e.PUBLIC_KEY).HasColumnType("mediumtext");
            entity.Property(e => e.TIMEOUT).HasPrecision(18);
            entity.Property(e => e.UPDATED).HasColumnType("datetime");
            entity.Property(e => e.UPDATED_BY).HasMaxLength(255);
            entity.Property(e => e.URL_MATCH).HasColumnType("mediumtext");
        });

        modelBuilder.Entity<upgradehistory>(entity =>
        {
            entity.HasKey(e => e.UPGRADECLASS).HasName("PRIMARY");

            entity.Property(e => e.DOWNGRADETASKREQUIRED)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.STATUS).HasMaxLength(255);
            entity.Property(e => e.TARGETBUILD).HasMaxLength(255);
        });

        modelBuilder.Entity<upgradetaskhistory>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.BUILD_NUMBER).HasPrecision(9);
            entity.Property(e => e.STATUS).HasMaxLength(60);
            entity.Property(e => e.UPGRADE_TASK_FACTORY_KEY).HasMaxLength(255);
            entity.Property(e => e.UPGRADE_TYPE).HasMaxLength(10);
        });

        modelBuilder.Entity<upgradetaskhistoryauditlog>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.ACTION).HasMaxLength(10);
            entity.Property(e => e.BUILD_NUMBER).HasPrecision(9);
            entity.Property(e => e.STATUS).HasMaxLength(60);
            entity.Property(e => e.TIMEPERFORMED).HasColumnType("datetime");
            entity.Property(e => e.UPGRADE_TASK_FACTORY_KEY).HasMaxLength(255);
            entity.Property(e => e.UPGRADE_TYPE).HasMaxLength(10);
        });

        modelBuilder.Entity<upgradeversionhistory>(entity =>
        {
            entity.HasKey(e => e.TARGETBUILD).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.TARGETVERSION).HasMaxLength(255);
            entity.Property(e => e.TIMEPERFORMED).HasColumnType("datetime");
        });

        modelBuilder.Entity<userassociation>(entity =>
        {
            entity.HasKey(e => new { e.SOURCE_NAME, e.SINK_NODE_ID, e.SINK_NODE_ENTITY, e.ASSOCIATION_TYPE })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

            entity.HasIndex(e => new { e.SINK_NODE_ID, e.SINK_NODE_ENTITY }, "user_sink");

            entity.HasIndex(e => e.SOURCE_NAME, "user_source");

            entity.Property(e => e.SOURCE_NAME).HasMaxLength(60);
            entity.Property(e => e.SINK_NODE_ID).HasPrecision(18);
            entity.Property(e => e.SINK_NODE_ENTITY).HasMaxLength(60);
            entity.Property(e => e.ASSOCIATION_TYPE).HasMaxLength(60);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.SEQUENCE).HasPrecision(9);
        });

        modelBuilder.Entity<userbase>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.username, "osuser_name");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PASSWORD_HASH).HasMaxLength(255);
        });

        modelBuilder.Entity<userhistoryitem>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.entitytype, e.USERNAME, e.entityid }, "uh_type_user_entity").IsUnique();

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.entityid).HasMaxLength(60);
            entity.Property(e => e.entitytype).HasMaxLength(10);
            entity.Property(e => e.lastviewed).HasPrecision(18);
        });

        modelBuilder.Entity<userpickerfilter>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.CUSTOMFIELD, "upf_customfield");

            entity.HasIndex(e => e.CUSTOMFIELDCONFIG, "upf_fieldconfigid");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CUSTOMFIELD).HasPrecision(18);
            entity.Property(e => e.CUSTOMFIELDCONFIG).HasPrecision(18);
            entity.Property(e => e.enabled).HasMaxLength(60);
        });

        modelBuilder.Entity<userpickerfiltergroup>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.USERPICKERFILTER, "cf_userpickerfiltergroup");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.USERPICKERFILTER).HasPrecision(18);
            entity.Property(e => e.groupname).HasMaxLength(255);
        });

        modelBuilder.Entity<userpickerfilterrole>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.USERPICKERFILTER, "cf_userpickerfilterrole");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.PROJECTROLEID).HasPrecision(18);
            entity.Property(e => e.USERPICKERFILTER).HasPrecision(18);
        });

        modelBuilder.Entity<versioncontrol>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.vcsdescription).HasMaxLength(255);
            entity.Property(e => e.vcsname).HasMaxLength(255);
            entity.Property(e => e.vcstype).HasMaxLength(255);
        });

        modelBuilder.Entity<votehistory>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.issueid, "votehistory_issue_index");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.TIMESTAMP).HasColumnType("datetime");
            entity.Property(e => e.VOTES).HasPrecision(18);
            entity.Property(e => e.issueid).HasPrecision(18);
        });

        modelBuilder.Entity<workflowscheme>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.DESCRIPTION).HasColumnType("mediumtext");
            entity.Property(e => e.NAME).HasMaxLength(255);
        });

        modelBuilder.Entity<workflowschemeentity>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.SCHEME, "workflow_scheme");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.SCHEME).HasPrecision(18);
            entity.Property(e => e.WORKFLOW).HasMaxLength(255);
            entity.Property(e => e.issuetype).HasMaxLength(255);
        });

        modelBuilder.Entity<worklog>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.AUTHOR, "worklog_author");

            entity.HasIndex(e => e.issueid, "worklog_issue");

            entity.Property(e => e.ID).HasPrecision(18);
            entity.Property(e => e.CREATED).HasColumnType("datetime");
            entity.Property(e => e.STARTDATE).HasColumnType("datetime");
            entity.Property(e => e.UPDATEAUTHOR).HasMaxLength(255);
            entity.Property(e => e.UPDATED).HasColumnType("datetime");
            entity.Property(e => e.grouplevel).HasMaxLength(255);
            entity.Property(e => e.issueid).HasPrecision(18);
            entity.Property(e => e.rolelevel).HasPrecision(18);
            entity.Property(e => e.timeworked).HasPrecision(18);
        });

        modelBuilder.Entity<worklog_version>(entity =>
        {
            entity.HasKey(e => e.WORKLOG_ID).HasName("PRIMARY");

            entity.HasIndex(e => new { e.DELETED, e.UPDATE_TIME }, "wv_deleted_update_time_idx");

            entity.HasIndex(e => e.PARENT_ISSUE_ID, "wv_parent_id");

            entity.HasIndex(e => e.UPDATE_TIME, "wv_update_time");

            entity.Property(e => e.WORKLOG_ID).HasPrecision(18);
            entity.Property(e => e.DELETED)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.INDEX_VERSION).HasPrecision(18);
            entity.Property(e => e.PARENT_ISSUE_ID).HasPrecision(18);
            entity.Property(e => e.UPDATE_TIME).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

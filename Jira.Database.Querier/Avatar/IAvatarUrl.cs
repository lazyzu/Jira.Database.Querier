using Generator.Equals;

namespace lazyzu.Jira.Database.Querier.Avatar
{
    public interface IAvatarUrl
    {
        string XSmall { get; }
        string Small { get; }
        string Medium { get; }
        string Large { get; }
    }

    [Equatable]
    public partial class AvatarUrl : IAvatarUrl
    {
        public string XSmall { get; init; }

        public string Small { get; init; }

        public string Medium { get; init; }

        public string Large { get; init; }
    }
}

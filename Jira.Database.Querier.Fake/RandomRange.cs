namespace lazyzu.Jira.Database.Querier.Fake
{
    public class RandomRange
    {
        public int Min { get; init; }
        public int Max { get; init; }

        public RandomRange(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}

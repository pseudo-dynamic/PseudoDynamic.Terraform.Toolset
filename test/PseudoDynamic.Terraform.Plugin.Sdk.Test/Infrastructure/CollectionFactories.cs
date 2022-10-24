namespace PseudoDynamic.Terraform.Plugin.Infrastructure
{
    internal static class CollectionFactories
    {
        public static IList<T> List<T>(params T[] items) => new List<T>(items);
        public static ISet<T> Set<T>(params T[] items) => new HashSet<T>(items);
        public static IDictionary<string, T> Map<T>(params (string, T)[] items) => new Dictionary<string, T>(items.Select(x => new KeyValuePair<string, T>(x.Item1, x.Item2)));
    }
}

namespace WebApplication1
{
    public struct LockOption
    {
        public static TimeSpan Expiry { get; private set; } = TimeSpan.FromSeconds(1);
        public static TimeSpan Wait { get; private set; } = TimeSpan.FromSeconds(1);
        public static TimeSpan Retry { get; private set; } = TimeSpan.FromSeconds(1);
    }
}

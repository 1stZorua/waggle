using AutoMapper;

namespace Waggle.Common.Helpers
{
    public static class AutoMapperExtensions
    {
        public static T? GetItem<T>(this ResolutionContext context, string key)
        {
            return context.Items.TryGetValue(key, out var value) && value is T typedValue
                ? typedValue
                : default;
        }
    }
}

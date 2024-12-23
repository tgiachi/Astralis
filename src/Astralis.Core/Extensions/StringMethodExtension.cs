using Astralis.Core.Utils;

namespace Astralis.Core.Extensions;

public static class StringMethodExtension
{

    public static string ToSnakeCase(this string text)
    {
        return StringUtils.ToSnakeCase(text);
    }

}

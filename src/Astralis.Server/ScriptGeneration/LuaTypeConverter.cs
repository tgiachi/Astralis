using NLua;

namespace Astralis.Server.ScriptGeneration;

public static class LuaTypeConverter
{
    public static string GetLuaType(Type type)
    {
        if (type == null)
        {
            return "nil";
        }

        return type.Name switch
        {
            "String"                                                => "string",
            "Int32" or "Int64" or "Single" or "Double" or "Decimal" => "number",
            "Boolean"                                               => "boolean",
            "Void"                                                  => "nil",
            "Object"                                                => "any",
            "LuaFunction"                                           => "function",
            "LuaTable"                                              => "table",
            var name when type.IsArray                              => $"{GetLuaType(type.GetElementType())}[]",
            var name when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)
                => $"{GetLuaType(type.GetGenericArguments()[0])}[]",
            var name when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                => "table",
            _ => type.Name.ToLower()
        };
    }

    public static string GetLuaType(object value)
    {
        if (value == null)
        {
            return "nil";
        }

        return value switch
        {
            string                                    => "string",
            int or long or float or double or decimal => "number",
            bool                                      => "boolean",
            LuaFunction                               => "function",
            LuaTable                                  => "table",
            System.Collections.IEnumerable            => "table", // Arrays e collections
            _                                         => value.GetType().Name.ToLower()
        };
    }


    public static string GetDetailedLuaType(Type type)
    {
        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(Dictionary<,>))
            {
                var keyType = GetLuaType(type.GetGenericArguments()[0]);
                var valueType = GetLuaType(type.GetGenericArguments()[1]);
                return $"table<{keyType}, {valueType}>";
            }
            else if (genericType == typeof(List<>) || genericType == typeof(IEnumerable<>))
            {
                var elementType = GetLuaType(type.GetGenericArguments()[0]);
                return $"{elementType}[]";
            }
        }

        if (type.IsEnum)
        {
            var values = string.Join("|", Enum.GetNames(type));
            return $"string # One of: {values}";
        }

        return GetLuaType(type);
    }

    public static T ConvertLuaValue<T>(object luaValue)
    {
        if (luaValue == null)
        {
            return default;
        }

        Type targetType = typeof(T);

        try
        {
            if (Nullable.GetUnderlyingType(targetType) != null)
            {
                if (luaValue == null) return default;
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            return (T)Convert.ChangeType(luaValue, targetType);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert Lua value of type {luaValue.GetType()} to {typeof(T)}", ex);
        }
    }

    private static LuaTable CreateLuaTable(Lua lua)
    {
        lua.NewTable("temp");
        var table = (LuaTable)lua["temp"];
        lua["temp"] = null;
        return table;
    }


    public static object ConvertToLuaValue(object value, Lua lua)
    {
        if (value == null) return null;

        return value switch
        {
            string or int or long or float or double or decimal or bool => value,


            System.Collections.IEnumerable enumerable => ConvertEnumerableToLuaTable(enumerable, lua),

            // DateTime come string
            DateTime dateTime => dateTime.ToString("O"),


            _ => ConvertObjectToLuaTable(value, lua)
        };
    }

    private static LuaTable ConvertEnumerableToLuaTable(System.Collections.IEnumerable enumerable, Lua lua)
    {
        var table = CreateLuaTable(lua);
        int index = 1;

        foreach (var item in enumerable)
        {
            table[index++] = ConvertToLuaValue(item, lua);
        }

        return table;
    }

    private static LuaTable ConvertObjectToLuaTable(object obj, Lua lua)
    {
        var table = CreateLuaTable(lua);

        foreach (var prop in obj.GetType().GetProperties())
        {
            var value = prop.GetValue(obj);
            table[prop.Name] = ConvertToLuaValue(value, lua);
        }

        return table;
    }
}

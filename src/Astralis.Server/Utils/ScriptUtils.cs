using NLua;

namespace Astralis.Server.Utils;

public static class ScriptUtils
{
    public static Dictionary<string, object> LuaTableToDictionary(LuaTable luaTable)
    {
        var dict = new Dictionary<string, object>();

        foreach (var key in luaTable.Keys)
        {
            dict[key.ToString()] = luaTable[key];

            if (luaTable[key] is LuaTable table)
            {
                dict[key.ToString()] = LuaTableToDictionary(table);
            }
        }

        return dict;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localization;
using UnityEngine;

using FuckCSharpWhyDoIHaveToRelayEverything = DialogueTableProviderExtensions;

public interface IDialogueTableProvider
{
    public Dictionary<string, (string original, string translated)> TranslationTable { get; }

    /// <summary>
    /// Initialization helper to shorten code size for describing the initial value of the translation table implementation
    /// Supports 1:1 mapping from enum value to string, will contain a default integer index of 0 for each entry
    /// </summary>
    /// <typeparam name="T">Enum type to be used by the implementer</typeparam>
    /// <returns></returns>
    protected static Dictionary<string, (string original, string translated)> InitializeTable<T>(Dictionary<T, string> input) where T: Enum
    {
        return input.ToDictionary(kv => LocalizationKey(kv.Key) , kv => (kv.Value, null as string) );
    }
    
    /// <summary>
    /// Initialization helper to shorten code size for describing the initial value of the translation table implementation
    /// Supports 1:X mapping from enum value to string, where each mapping is additionally described by an integer index (Such as `ShopDialogue.Default:2`)
    /// </summary>
    /// <typeparam name="T">Enum type to be used by the implementer</typeparam>
    /// <typeparam name="V">Any enumerable string container, such as array, list, etc.</typeparam>
    /// <returns></returns>
    protected static Dictionary<string, (string original, string translated)> InitializeTable<T, V>(Dictionary<T, V> input) where T: Enum where V: IEnumerable<string>
    {
        return input
            .SelectMany(kv => kv.Value.Select((str, idx) => new KeyValuePair<string, string>(LocalizationKey(kv.Key, idx), str)))
            .ToDictionary(kv => kv.Key, kv => (kv.Value, null as string) );
    }
    
    /// <summary>
    /// Exposed translation logic from entry index specification to a serialized string (seen in localization CSV file paths)
    /// </summary>
    /// <param name="key">Enum value</param>
    /// <param name="i">Integer index in case enum value is mapped to multiple consecutive entries</param>
    /// <typeparam name="T">Enum type to be used by the implementer</typeparam>
    /// <returns></returns>
    protected static string LocalizationKey<T>(T key, int i = 0) where T: Enum
    {
        return Enum.GetName(typeof(T), key) + Localizable.indexSeparatorSecondary + i.ToString();
    }

    /// <summary>
    /// Retrieves localization from table, translation is inserted by the LocalizationLoader at runtime
    /// </summary>
    /// <param name="key">Enum value</param>
    /// <param name="i">Integer index in case enum value is mapped to multiple consecutive entries</param>
    /// <typeparam name="T">Enum type to be used by the implementer</typeparam>
    /// <returns>Pair of (original, translated) strings</returns>
    public (string original, string translated) GetLocalized<T>(T key, int i = 0) where T: Enum
    {
        string index = LocalizationKey(key, i);
        return GetLocalized(index);
    }

    /// <summary>
    /// Retrieves localization from table, translation is inserted by the LocalizationLoader at runtime
    /// </summary>
    /// <param name="formattedIndex">Serialized result of LocalizationKey, please do not come up with your own string to put here :)</param>
    /// <returns>Pair of (original, translated) strings</returns>
    public (string original, string translated) GetLocalized(string formattedIndex)
    {
        if (TranslationTable.TryGetValue(formattedIndex, out var val))
        {
            return val;
        }
        return ("ERROR: NOT FOUND", null);
    }
    
    public bool LocalizeEntry(string key, string translated)
    {
        if (TranslationTable.ContainsKey(key))
        {
            TranslationTable[key] = (TranslationTable[key].original, translated);
            return true;
        }
    
        return false;
    }
}

public static class DialogueTableProviderExtensions
{
    /// <summary>
    /// Retrieves localization from table, translation is inserted by the LocalizationLoader at runtime
    /// </summary>
    /// <param name="key">Enum value</param>
    /// <param name="i">Integer index in case enum value is mapped to multiple consecutive entries</param>
    /// <typeparam name="T">Enum type to be used by the implementer</typeparam>
    /// <returns>Pair of (original, translated) strings</returns>
    public static (string original, string translated) GetLocalized<I, T>(this I self,T key, int i = 0) where I: MonoBehaviour, IDialogueTableProvider where T : Enum
    {
        return self.GetLocalized(key, i);
    }
}
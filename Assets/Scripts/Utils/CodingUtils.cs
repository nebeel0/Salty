using System.Collections;
using System.Collections.Generic;

public static class CodingUtils
{
    public static IEnumerable<DictionaryEntry> CastDict(IDictionary dictionary)
    {
        foreach (DictionaryEntry entry in dictionary)
        {
            yield return entry;
        }
    }

}


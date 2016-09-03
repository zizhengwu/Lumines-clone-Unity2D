using System.Collections.Generic;

internal static class ListExtension {

    public static T PopAt<T>(this List<T> list, int index) {
        T r = list[index];
        list.RemoveAt(index);
        return r;
    }
}
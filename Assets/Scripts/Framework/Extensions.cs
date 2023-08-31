using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class Extensions
    {
        public static T GetRandom<T>(this List<T> list)
        {
            var randomItem = list[Random.Range(0, list.Count)];
            return randomItem;
        }
    }
}
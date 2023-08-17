using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtilities
{
    public class SingletonScriptableObjects<T> : ScriptableObject where T : SingletonScriptableObjects<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] assets = Resources.LoadAll<T>("");
                    if (assets == null || assets.Length < 1)
                    {
                        throw new System.Exception("No File Found of Type: " + typeof(T));
                    }
                    else if (assets.Length > 1)
                    {
                        Debug.LogWarning("More than 1 File of Type: " + typeof(T));
                    }

                    instance = assets[0];
                }
                return instance;
            }
        }

    }
}
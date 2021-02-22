using UnityEngine;

namespace MyBrokenReality
{
    public class Loader
    {
        public static void Init()
        {
            Load = new GameObject();
            Load.AddComponent<BreakOutOfReality>();
            UnityEngine.Object.DontDestroyOnLoad(Load);
        }

        private static void _Unload()
        {
            UnityEngine.Object.Destroy(Load); //this will be your gameobject you created
        }

        public static void Unload()
        {
            _Unload();
        }

        private static GameObject Load;
    }
}

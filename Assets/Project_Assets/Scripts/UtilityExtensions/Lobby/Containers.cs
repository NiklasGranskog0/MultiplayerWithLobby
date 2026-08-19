namespace Project_Assets.Scripts.UtilityExtensions.Lobby
{
    public static class Containers
    {
        public static void ClearContainer(this UnityEngine.Transform container)
        {
            foreach (UnityEngine.Transform child in container)
            {
                if (child != null)
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }
            }
        }
    }
}
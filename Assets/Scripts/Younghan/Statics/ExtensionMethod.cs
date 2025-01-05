using UnityEngine;
public static partial class ExtensionMethod
{

    public static bool IsFinished(this Spell.Info info, float deltaTime)
    {
        if (info != null)
        {
            return info.IsFinished(deltaTime);
        }
        return false;
    }
}

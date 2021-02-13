using UnityEngine;

public static class Utilities
{
    private static System.Random random = new System.Random();

    public static string RandomId(int length)
    {
        char[] newString = new char[length];
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        for (int i = 0; i < length; i++)
        {
            newString[i] = chars[random.Next(chars.Length - 1)];
        }
        return new string(newString);
    }

    public static void EnableCanvas(CanvasGroup cg, bool v)
    {
        cg.interactable = v;
        cg.blocksRaycasts = v;
        cg.alpha = v ? 1 : 0;
    }
}

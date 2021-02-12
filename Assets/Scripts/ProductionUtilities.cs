namespace ProductionPipeline
{
    public static class ProductionUtilities
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
    }

}

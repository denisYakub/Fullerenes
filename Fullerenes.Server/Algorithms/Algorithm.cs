namespace Fullerenes.Server.Algorithms
{
    public static class Algorithm
    {
        public static int RodHousesAlgorithmVol1(int[] houses)
        {
            ArgumentNullException.ThrowIfNull(houses);

            switch (houses.Length)
            {
                case 0:
                    return 0;
                case 1:
                    return houses[0];
                case 2:
                    return int.Max(houses[0], houses[1]);
            }

            int a = houses[0], b = houses[1];

            for (int i = 2; i < houses.Length; i++)
            {
                int max = a > b ? a : b;
                b = a + houses[i];
                a = max;
            }

            return a > b ? a : b;
        }
    }
}

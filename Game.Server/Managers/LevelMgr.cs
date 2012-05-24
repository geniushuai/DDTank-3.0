using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Managers
{
    public class LevelMgr
    {
//        static int[] levels = new int[] { 0, 37, 162, 505, 1283, 2801, 5462, 9771, 16341, 25899, 39291, 57489, 81594, 112847, 152630, 202472, 264058, 339232, 430003, 538554, 667242, 818609, 995383, 1200490, 1437053 ,1753103,
//2112735,2519637,2977665,3490849,int.MaxValue };
        static int[] levels = new int[] {0,37,162,505,1283,2801,5462,9771,16341,25899,39291,57489,81594,112847,152630,202472,264058,339232,430003,538554,667242,818609,995383,1200490,1437053,1753103,2112735,2519637,2977665,3490849,4145185,4873978,5684269,6583537,7579710,8681174,9896788,11235892,12708322,14324419,14555555,int.MaxValue};
        public static int GetLevel(int GP)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                if (GP < levels[i])
                    return i;
            }

            return 1;
        }

        public static int GetGP(int level)
        {
            if (levels.Length > level && level > 0)
            {
                return levels[level - 1];
            }
            return 0;
        }

        public static int ReduceGP(int level, int totalGP)
        {
            if (levels.Length > level && level > 0)
            {

                totalGP = totalGP - levels[level - 1];
                if (totalGP < level * 12)
                {
                    return totalGP < 0 ? 0 : totalGP;
                }
                return level * 12;
            }
            return 0;
        }

        public static int IncreaseGP(int level, int totalGP)
        {
            if (levels.Length > level && level > 0)
            {
                //totalGP = totalGP + levels[level - 1];
                //if (totalGP < level * 12)
                //{
                //    return totalGP < 0 ? 0 : totalGP;
                //}
                return level * 12;
            }
            return 0;
        }
    }
}

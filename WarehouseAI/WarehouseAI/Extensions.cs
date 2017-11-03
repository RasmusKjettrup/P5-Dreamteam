using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the powerset of the enumeration
        /// </summary>
        /// <typeparam name="T">The type of the data getting enumerated</typeparam>
        /// <param name="x">The given enumeration</param>
        /// <returns>result</returns>
        public static IEnumerable<T[]> Power<T>(this IEnumerable<T> x)
        {
            //Returns the empty set
            yield return new T[0];

            //Return all sets in the power set of length 1 and higher
            foreach (T[] t in Power(x, 1))
            {
                yield return t;
            }
        }

        private static IEnumerable<T[]> Power<T>(this IEnumerable<T> x, int length)
        {
            //Get the full set enumerated over
            T[] fullSet = x.ToArray();

            //This is a recursive function, and if the requested length is over the length of the set, stop the recursion
            if (length > fullSet.Length)
            {
                yield break;
            }

            //"picks" is the array of indexes that need to be returned in a specific iteration of the while(true) loop.
            int[] picks = new int[length];

            //It is initialized to be all the first indexes.
            for (int i = 0; i < picks.Length; i++)
            {
                picks[i] = i;
            }

            //While new values are still being found
            while (true)
            {
                //Return all items in the set, where the index is found in "picks"
                yield return fullSet.Where((t, j) => picks.Contains(j)).ToArray();

                //Breaking value
                bool brea = true;

                //Finds a new set of indexes to pick from the full set
                for (int i = picks.Length - 1; i >= 0; i--)
                {
                    //Find a new potential value.
                    int potentialVal = picks[i] + 1;

                    //If the is potential value is under the length of the full set, and picks does not contain the potential value...
                    if (potentialVal < fullSet.Length && !picks.Contains(potentialVal))
                    {
                        //Apply this new value
                        picks[i]++;

                        //When the new value is applied, update all picks after the new value to be the new value plus one
                        //Set all values after the pick to their preceeding, plus one
                        for (int j = i+1; j < picks.Length; j++)
                        {
                            picks[j] = picks[j-1]+1;
                        }
                        //Make the while loop loop
                        brea = false;
                        //And stop updating "picks"
                        break;
                    }
                }
                //No new values can be found of the given length
                if (brea)
                {
                    break;
                }
            }

            //Return all sets of this length, plus one.
            foreach (T[] t in Power(fullSet, length + 1))
            {
                yield return t;
            }
        }
    }
}
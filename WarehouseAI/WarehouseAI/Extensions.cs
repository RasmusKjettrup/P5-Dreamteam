using System.Collections.Generic;
using System.Linq;
using System;

namespace WarehouseAI
{
    public static class Extensions
    {
        /// <summary>
        /// Appends the set to the end of an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">Base object</param>
        /// <param name="y">Set to add</param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this T x, IEnumerable<T> y)
        {
            yield return x;

            foreach (T t in y)
            {
                yield return t;
            }
        }

        /// <summary>
        /// Appends the object to the end of a set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">Base set</param>
        /// <param name="y">Object to add</param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> x, T y)
        {
            foreach (T t in x)
            {
                yield return t;
            }
            yield return y;
        }

        /// <summary>
        /// Gets the powerset of the enumeration
        /// </summary>
        /// <typeparam name="T">The type of the data getting enumerated</typeparam>
        /// <param name="x">The given enumeration</param>
        /// <returns>result</returns>
        public static IEnumerable<T[]> Power<T>(this IEnumerable<T> x)
        {
            //Return the power set of length 0 and higher
            return Power(x.ToArray(), 0);
        }

        private static IEnumerable<T[]> Power<T>(T[] fullSet, int length)
        {
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

            //While new sets are still being found
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
                //No new sets can be found of the given length
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

        private static Random _random;

        public static T Random<T>(this IEnumerable<T> x)
        {
            if (_random == null)
            {
                _random = new Random(0);
            }
            T[] array = x.ToArray();
            return array[_random.Next(0, array.Length)];
        }
    }
}
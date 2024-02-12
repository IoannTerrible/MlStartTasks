using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MlStartTask2
{
    public class MathSolver
    {
        public static double[,] Solve(double[,] emptyDoubleArray, int[] arrayWithNumbersForСondition, int[] firstBaseArray, double[] secondBaseArray )
        {
            for (int i = 0; i < firstBaseArray.Length; i++)
            {
                if (firstBaseArray[i] == 9)
                {
                    for (int j = 0; j < emptyDoubleArray.GetLength(1); j++)
                    {
                        emptyDoubleArray[i, j] = Math.Sin(Math.Sin(Math.Pow((secondBaseArray[j] / (secondBaseArray[j] + 0.5)), secondBaseArray[j])));
                    }
                }
                else if (arrayWithNumbersForСondition.Contains(firstBaseArray[i]))
                {
                    for (int j = 0; j < emptyDoubleArray.GetLength(1); j++)
                    {
                        double exp = 0.5 / (Math.Tan(2 * secondBaseArray[j]) + (2.0 / 3.0));
                        emptyDoubleArray[i, j] = Math.Pow(exp, Math.Pow(Math.Pow(secondBaseArray[j], 1.0 / 3.0), 1.0 / 3.0));
                    }
                }
                else
                {
                    for (int j = 0; j < emptyDoubleArray.GetLength(1); j++)
                    {
                        emptyDoubleArray[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - secondBaseArray[j] / Math.PI) / 3.0) / 4.0), 3));
                    }
                }
            }
            return emptyDoubleArray;
        }
    }
}

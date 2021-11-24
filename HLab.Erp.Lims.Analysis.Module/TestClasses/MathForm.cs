using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public static class MathExtentions
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double average = values.Average();
            double somme = 0.0;
            int nb = 0;

            foreach(var value in values)
            {
                double delta = value - average;
                somme += delta * delta;
                nb++;
            }

            return Math.Sqrt(somme/(nb-1));

        }
        public static double CV(this IEnumerable<double> values)
        {
            return 100.0 * values.StandardDeviation() / values.Average();
        }
    }
}

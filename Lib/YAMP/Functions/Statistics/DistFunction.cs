﻿namespace YAMP
{
    using System;
    using YAMP.Exceptions;

    [Description("DistFunctionDescription")]
    [Kind(PopularKinds.Statistic)]
    internal sealed class DistFunction : SystemFunction
    {
        public DistFunction(ParseContext context)
            : base(context)
        {
        }

        [Description("DistFunctionDescriptionForMatrixScalarScalar")]
        [Example("dist([randn(500, 1); randn(1000, 1) + 5], 40, 10)", "DistFunctionExampleForMatrixScalarScalar1")]
        public FunctionValue Function(MatrixValue Y, ScalarValue nbins, ScalarValue nParameters)
        {
            var nn = nbins.GetIntegerOrThrowException("nbins", Name);
            var nP = nParameters.GetIntegerOrThrowException("nParameters", Name);
            var N = Y.Length;
            var min_idx = Y.Min();
            var min = Y[min_idx.Row, min_idx.Column];
            var max_idx = Y.Max();
            var max = Y[max_idx.Row, max_idx.Column];
            var median = YMath.Median(Y);
            
            var variance = ScalarValue.Zero;
            var mean = Y.Sum() / Y.Length;

            for (var i = 1; i <= Y.Length; i++)
            {
                variance += (Y[i] - mean).Square();
            }

            variance /= Y.Length;

            var delta = (max - min) / nn;

            var x = new MatrixValue(nn, 1);

            for (var i = 0; i < nn; i++)
            {
                x[i + 1] = min + delta * i;
            }

            var histogram = new HistogramFunction();
            var fx = histogram.Function(Y, x);
            var linearfit = new LinfitFunction(Context);

            var dist = linearfit.Function(x, fx, new FunctionValue((context, argument) =>
            {
                var _x = (argument as ScalarValue - median / 2) / (variance / 4);
                var _exp_x_2 = (-_x * _x).Exp();
                var result = new MatrixValue(1, nP - 1);

                for (var i = 0; i < nP - 1; i++)
                {
                    result[i + 1] = _exp_x_2 * _x.Pow(new ScalarValue(i));
                }

                return result;
            }, true));

            var norm = Y.Length * (max - min) / nbins;
            var normed_dist = new FunctionValue((context, argument) =>
            {
                var temp = dist.Perform(context, argument);

                if (temp is ScalarValue)
                {
                    return ((ScalarValue)temp) / norm;
                }
                else if (temp is MatrixValue)
                {
                    return ((MatrixValue)temp) / norm;
                }
                
                throw new YAMPOperationInvalidException();
            }, true);

            return normed_dist;
        }

        [Description("DistFunctionDescriptionForMatrix")]
        [Example("dist([randn(500, 1); randn(1000, 1) + 5])", "DistFunctionExampleForMatrix1")]
        public FunctionValue Function(MatrixValue Y)
        {
            var nbins = new ScalarValue(Math.Round(Math.Sqrt(Y.Length)));
            var nParameters = new ScalarValue(Math.Round(Math.Log(Y.Length)));
            return Function(Y, nbins, nParameters);
        }
    }
}

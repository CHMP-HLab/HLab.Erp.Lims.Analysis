namespace YAMP
{
    using YAMP.Numerics;

    [Description("TrilFunctionDescription")]
    [Kind(PopularKinds.Function)]
    internal sealed class TrilFunction : ArgumentFunction
    {
        [Description("TrilFunctionDescriptionForMatrix")]
        [Example("tril(rand(4))", "TrilFunctionExampleForMatrix1")]
        public MatrixValue Function(MatrixValue M)
        {
            var lu = new LUDecomposition(M);
            return lu.Pivot * lu.L;
        }
    }
}

﻿namespace YAMP
{
    using YAMP.Converter;

    /// <summary>
    /// Contains the data for barplots.
    /// </summary>
    public sealed class BarPlotValue : XYPlotValue
	{
		#region ctor

        /// <summary>
        /// Creates a new instance.
        /// </summary>
		public BarPlotValue()
        {
            MinX = 1.0;
            MaxX = 1.0;
            MinY = 0.0;
            MaxY = 0.0;
		}

		#endregion

		#region Methods

        /// <summary>
        /// Adds (multiple) series in form of a matrix.
        /// </summary>
        /// <param name="m">The matrix which contains the values.</param>
		public override void AddPoints(MatrixValue m)
		{
            if (m.IsVector)
                AddSingleSeries(m);
            else
            {
                if (m.DimensionX < m.DimensionY)
                    m = m.Transpose();

                //From here on m.DimensionX >= m.DimensionY !
                for (var j = 1; j <= m.DimensionY; j++)
                {
                    var vec = m.GetRowVector(j);
                    AddSingleSeries(vec);
                }
            }
		}

        /// <summary>
        /// Adds a single series explicitly.
        /// </summary>
        /// <param name="vec">The matrix seen as a vector.</param>
        public void AddSingleSeries(MatrixValue vec)
        {
            var values = new BarPoints();

            for (var i = 1; i <= vec.Length; i++)
            {
                var value = 0.0;

                if (vec[i].IsComplex)
                    value = vec[i].Abs();
                else
                    value = vec[i].Re;

                if (value < MinY)
                    MinY = value;
                else if (value > MaxY)
                    MaxY = value;

                if (i > MaxX)
                    MaxX = i;

                values.Add(value);
            }

            AddSeries(values);
        }

		#endregion

		#region Serialization

        /// <summary>
        /// Converts the given instance to an array of bytes.
        /// </summary>
        /// <returns>The binary representation of this instance.</returns>
		public override byte[] Serialize()
		{
			using (var s = Serializer.Create())
			{
				Serialize(s);
				s.Serialize(Count);

				for (var i = 0; i < Count; i++)
				{
					var points = this[i];
                    points.Serialize(s);
                    s.Serialize(points.Count);

                    for(var j = 0; j < points.Count; j++)
					    s.Serialize(points[j]);

					s.Serialize(points.BarWidth);
				}

				return s.Value;
			}
		}

        /// <summary>
        /// Converts a set of bytes to a new instance.
        /// </summary>
        /// <param name="content">The binary representation.</param>
        /// <returns>The new instance.</returns>
		public override Value Deserialize(byte[] content)
        {
            var bp = new BarPlotValue();

			using (var ds = Deserializer.Create(content))
			{
				bp.Deserialize(ds);
				var length = ds.GetInt();

				for (var i = 0; i < length; i++)
				{
					var points = new BarPoints();
					points.Deserialize(ds);
                    var count = ds.GetInt();

                    for(var j = 0; j < count; j++)
					    points.Add(ds.GetDouble());

					points.BarWidth = ds.GetDouble();
					bp.AddSeries(points);
				}
			}

			return bp;
		}

		#endregion

		#region Nested Type

        /// <summary>
        /// The representation of one series.
        /// </summary>
		public class BarPoints : Points<double>
		{
            /// <summary>
            /// Creates a new bar series.
            /// </summary>
			public BarPoints()
			{
				BarWidth = 1.0;
			}

            /// <summary>
            /// Gets or sets the relative width of the bars.
            /// </summary>
			[ScalarToDoubleConverter]
			public double BarWidth
			{
				get;
				set;
			}
		}

		#endregion

		#region Index

        /// <summary>
        /// Gets the series at the specified index.
        /// </summary>
        /// <param name="index">The 0-based index of the series.</param>
        /// <returns>The series (list of points and properties).</returns>
		public BarPoints this[int index]
		{
			get
			{
				return base.GetSeries(index) as BarPoints;
			}
		}

        /// <summary>
        /// Gets a certain point of the specified series.
        /// </summary>
        /// <param name="index">The 0-based index of the series.</param>
        /// <param name="point">The 0-based index of the point.</param>
        /// <returns>The point.</returns>
		public double this[int index, int point]
		{
			get
			{
				return this[index][point];
			}
		}

		#endregion
	}
}

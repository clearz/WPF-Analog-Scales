namespace Messure.Utils {
    public static class Extensions {
        
        public static double Remap(this double value, double from1, double to1, double from2, float to2)
        {
            return ( value - from1 ) / ( to1 - from1 ) * ( to2 - from2 ) + from2;
        }

        public static double Clamp(this double value, double from = double.MinValue, double to = double.MaxValue)
        {
            return value < from ? from : value > to ? to : value;
        }

        public static double Average(this double d) =>  AveragingBuffer.InsertAndAverage(d); // ManagedBuffer.Current = d;
    }
}

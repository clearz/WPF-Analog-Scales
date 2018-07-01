using System.Globalization;
using System.Runtime.InteropServices;

namespace Messure.Utils {

    // Smooth motion of scales needle by averaging over a buffer of previous weights
    unsafe struct AveragingBuffer {
        private double _value;
        public static AveragingBuffer* Current { get; set; }
        private AveragingBuffer* Next { get; set; }
        private const int SIZE = 15;

        static AveragingBuffer()
        {
            Current = (AveragingBuffer*)Marshal.AllocHGlobal(sizeof(AveragingBuffer));
            Current->Next = Current->Init();
        }

        private AveragingBuffer* Init(int size = SIZE) {
            _value = 1;
            Next = (AveragingBuffer*)Marshal.AllocHGlobal(sizeof(AveragingBuffer));
            if (size > 1)
                Next->Next = Next->Init(size - 1);
            else return Current;

            return Next;
        }
        public static double InsertAndAverage(double d)
        {
            (Current = Current->Next)->_value = d;
            double cont = Current->_value;
            AveragingBuffer* head = Current;
            while ((head = head->Next) != Current) cont += head->_value;
            return cont / SIZE;
        }

        public override string ToString() => _value.ToString(CultureInfo.CurrentCulture);
    }

    // Managed version of above ADT
    internal class ManagedBuffer
    {
        public static double Counter = 1.0;
        private double _value;
        public static ManagedBuffer Current { get;set; }

        private ManagedBuffer Next { get; set; }

        private const int SIZE = 15;
        static ManagedBuffer() {
            Current = new ManagedBuffer();
            Current.Init(SIZE);
        }
  
        private ManagedBuffer Init(int size) {
            if (size > 0)
                Next = new ManagedBuffer().Init(size - 1);
            else {
                return Current;
            }

                return this;
        }

        public static implicit operator double(ManagedBuffer a)
        {
            double cont = Current._value;
            ManagedBuffer head = Current;
            while ((head = head.Next) != Current) cont += head._value;
            return cont / SIZE;
            return Current._value;
        }

        public static implicit operator ManagedBuffer(double d)
        {
            (Current = Current.Next)._value = d;
            return Current;
        }

        public override string ToString()
        {
            return this._value.ToString();
        }
        private ManagedBuffer() { }
    }
}

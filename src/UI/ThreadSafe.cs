using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class ThreadSafe
{
    //[Serializable]
    [DebuggerDisplay("ThreadSafe.Integer: {GetValue()}")]
    [JsonConverter(typeof(ThreadSafeConverter))]
    public sealed class Integer:IComparable<Integer>, IComparable<int>
    {
        [JsonProperty]
        private int _value;

        [DebuggerStepThrough]
        //[JsonConstructor]
        public Integer(int initialValue = 0)
        {
            _value = initialValue;
        }

        public void Decrement(int minValue = int.MinValue)
        {
            int initialValue, computedValue;
            do
            {
                initialValue = Interlocked.CompareExchange(ref _value, 0, 0);
                computedValue = initialValue - 1;
                if (computedValue < minValue)
                {
                    computedValue = minValue;
                }
            } while (Interlocked.CompareExchange(ref _value, computedValue, initialValue) != initialValue);
        }

        // IComparable<ThreadSafe.Integer> implementation
        public int CompareTo(Integer other)
        {
            if (other == null) return 1;
            return GetValue().CompareTo(other.GetValue());
        }

        // IComparable<int> implementation
        public int CompareTo(int other)
        {
            return GetValue().CompareTo(other);
        }

        public static Integer operator ++(Integer tsi)
        {
            Interlocked.Increment(ref tsi._value);
            return tsi;
        }

        public static Integer operator --(Integer tsi)
        {
            Interlocked.Decrement(ref tsi._value);
            return tsi;
        }

        public static Integer operator +(Integer tsi, int value)
        {
            Interlocked.Add(ref tsi._value, value);
            return tsi;
        }

        public static Integer operator -(Integer tsi, int value)
        {
            Interlocked.Add(ref tsi._value, -value);
            return tsi;
        }

        public int GetValue()
        {
            return Interlocked.CompareExchange(ref _value, 0, 0);
        }


        public override string ToString()
        {
            return GetValue().ToString();
        }

        // Implicit conversion from int to ThreadSafeIntegerWithInterlocked
        public static implicit operator Integer(int value)
        {
            return new Integer(value);
        }

        // Implicit conversion from ThreadSafeIntegerWithInterlocked to int
        public static implicit operator int(Integer tsi)
        {
            return tsi.GetValue();
        }

        //allow (int)ThreadSafeInteger
        //public static explicit operator int(Integer tsi)
        //{
        //    return tsi.GetValue();
        //}
        // Equality check
        public override bool Equals(object obj)
        {
            if (obj is Integer other)
            {
                return GetValue() == other.GetValue();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }

        // Equality operators
        public static bool operator ==(Integer left, Integer right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.GetValue() == right.GetValue();
        }

        public static bool operator !=(Integer left, Integer right)
        {
            return !(left == right);
        }

        // Comparison with int
        public static bool operator ==(Integer left, int right)
        {
            if (left is null)
            {
                return false;
            }

            return left.GetValue() == right;
        }

        public static bool operator !=(Integer left, int right)
        {
            return !(left == right);
        }

        public static bool operator ==(int left, Integer right)
        {
            if (right is null)
            {
                return false;
            }

            return left == right.GetValue();
        }

        public static bool operator !=(int left, Integer right)
        {
            return !(left == right);
        }
    }

    //===================================================================================================================
    [DebuggerDisplay("ThreadSafe.DateTime: {GetValue()} ({GetValue().Ticks} ticks)")]
    [JsonConverter(typeof(ThreadSafeConverter))]
    public sealed class DateTime:IComparable<DateTime>, IComparable<System.DateTime>
    {
        [JsonProperty]
        private long _value;
        public static string _dateFormat = "dd.MM.yy, HH:mm:ss";
        [DebuggerStepThrough]
        //[JsonConstructor]
        public DateTime(System.DateTime initialValue, string dateFormat)
        {
            //_ticks = initialValue.Ticks;
            _value = initialValue.ToBinary();
            _dateFormat = dateFormat;
        }

        public DateTime()
        {
            //_ticks = System.DateTime.Now.Ticks;
            _value = System.DateTime.Now.ToBinary();
        }

        [JsonConstructor]
        public DateTime(long value)
        {
            _value = value;
        }

        public System.DateTime GetValue()
        {
            //long ticks = Interlocked.Read(ref _ticks);
            //return new System.DateTime(ticks);
            return System.DateTime.FromBinary(Interlocked.Read(ref _value));
        }

        public void SetValue(System.DateTime value)
        {
            //Interlocked.Exchange(ref _ticks, value.Ticks);
            Interlocked.Exchange(ref _value, value.ToBinary());
        }

        // IComparable<ThreadSafe.DateTime> implementation
        public int CompareTo(DateTime other)
        {
            if (other == null) return 1;
            return GetValue().CompareTo(other.GetValue());
        }

        // IComparable<System.DateTime> implementation
        public int CompareTo(System.DateTime other)
        {
            return GetValue().CompareTo(other);
        }
        public void Add(TimeSpan value)
        {
            long newBinaryValue, originalBinaryValue;
            do
            {
                originalBinaryValue = Interlocked.Read(ref _value);
                var newDateTime = System.DateTime.FromBinary(originalBinaryValue).Add(value);
                newBinaryValue = newDateTime.ToBinary();
            } while (Interlocked.CompareExchange(ref _value, newBinaryValue, originalBinaryValue) != originalBinaryValue);
        }

        public void AddDays(double value)
        {
            Add(TimeSpan.FromDays(value));
        }

        public void AddHours(double value)
        {
            Add(TimeSpan.FromHours(value));
        }

        public void AddMinutes(double value)
        {
            Add(TimeSpan.FromMinutes(value));
        }

        public void AddSeconds(double value)
        {
            Add(TimeSpan.FromSeconds(value));
        }

        public override string ToString()
        {
            return GetValue().ToString(_dateFormat, CultureInfo.InvariantCulture);
        }

        public string ToString(string format)
        {
            return GetValue().ToString(format, CultureInfo.InvariantCulture);
        }

        // Implicit conversion from DateTime to ThreadSafeDateTime
        public static implicit operator DateTime(System.DateTime value)
        {
            return new DateTime(value, _dateFormat);
        }

        // Implicit conversion from ThreadSafeDateTime to DateTime
        public static implicit operator System.DateTime(DateTime tsd)
        {
            return tsd.GetValue();
        }

        // Operators
        // Equality operators between ThreadSafe.DateTime and System.DateTime
        public static bool operator ==(DateTime tsd, System.DateTime dt)
        {
            return tsd.GetValue() == dt;
        }

        public static bool operator !=(DateTime tsd, System.DateTime dt)
        {
            return tsd.GetValue() != dt;
        }

        public static bool operator ==(System.DateTime dt, DateTime tsd)
        {
            return dt == tsd.GetValue();
        }

        public static bool operator !=(System.DateTime dt, DateTime tsd)
        {
            return dt != tsd.GetValue();
        }

        // Comparison operators between ThreadSafe.DateTime and System.DateTime
        public static bool operator <(DateTime tsd, System.DateTime dt)
        {
            return tsd.GetValue() < dt;
        }

        public static bool operator >(DateTime tsd, System.DateTime dt)
        {
            return tsd.GetValue() > dt;
        }

        public static bool operator <=(DateTime tsd, System.DateTime dt)
        {
            return tsd.GetValue() <= dt;
        }

        public static bool operator >=(DateTime tsd, System.DateTime dt)
        {
            return tsd.GetValue() >= dt;
        }

        public static bool operator <(System.DateTime dt, DateTime tsd)
        {
            return dt < tsd.GetValue();
        }

        public static bool operator >(System.DateTime dt, DateTime tsd)
        {
            return dt > tsd.GetValue();
        }

        public static bool operator <=(System.DateTime dt, DateTime tsd)
        {
            return dt <= tsd.GetValue();
        }

        public static bool operator >=(System.DateTime dt, DateTime tsd)
        {
            return dt >= tsd.GetValue();
        }

        // Subtraction operator between ThreadSafe.DateTime and System.DateTime
        public static TimeSpan operator -(ThreadSafe.DateTime tsd, System.DateTime dt)
        {
            return tsd.GetValue() - dt;
        }
        public static DateTime operator +(DateTime tsd, TimeSpan value)
        {
            tsd.Add(value);
            return tsd;
        }
        public static DateTime operator -(DateTime tsd, TimeSpan value)
        {
            tsd.Add(-value);
            return tsd;
        }
        public static TimeSpan operator -(DateTime tsd1, DateTime tsd2)
        {
            return tsd1.GetValue() - tsd2.GetValue();
        }

        public static TimeSpan operator -(System.DateTime dt, DateTime tsd)
        {
            return dt - tsd.GetValue();
        }
        public static bool operator ==(DateTime tsd1, DateTime tsd2)
        {
            return tsd1.GetValue() == tsd2.GetValue();
        }

        public static bool operator !=(DateTime tsd1, DateTime tsd2)
        {
            return tsd1.GetValue() != tsd2.GetValue();
        }


        public static bool operator <(DateTime tsd1, DateTime tsd2)
        {
            return tsd1.GetValue() < tsd2.GetValue();
        }

        public static bool operator >(DateTime tsd1, DateTime tsd2)
        {
            return tsd1.GetValue() > tsd2.GetValue();
        }

        public static bool operator <=(DateTime tsd1, DateTime tsd2)
        {
            return tsd1.GetValue() <= tsd2.GetValue();
        }

        public static bool operator >=(DateTime tsd1, DateTime tsd2)
        {
            return tsd1.GetValue() >= tsd2.GetValue();
        }


        public override bool Equals(object obj)
        {
            if (obj is DateTime other)
            {
                return this == other;
            }
            if (obj is System.DateTime dateTime)
            {
                return this.GetValue() == dateTime;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }




    }

    //===================================================================================================================

    [DebuggerDisplay("ThreadSafe.Boolean: {GetValue()}")]
    [JsonConverter(typeof(ThreadSafeConverter))]
    public sealed class Boolean:IComparable<Boolean>, IComparable<bool>
    {
        [JsonProperty]
        private int _value;
        private const int False = 0;
        private const int True = 1;
        //[MethodImpl(MethodImplOptions.NoOptimization)]
        [DebuggerStepThrough]
        public Boolean(bool initialValue = false)
        {
            _value = initialValue ? True : False;
        }

        //[JsonConstructor]
        private Boolean(int value)
        {
            _value = value;
        }
        //[MethodImpl(MethodImplOptions.NoOptimization)]
        public bool GetValue()
        {
            //when you're just retrieving the value (as in a getter), there's no need to use CompareExchange because you're not changing the value. You just want to get the current value, and it doesn't matter what that value is.
            //return _value == True;
            //return Volatile.Read(ref _value) == 1;
            return Interlocked.CompareExchange(ref _value, 0, 0) == True;
        }

        //A multithreaded stress test may fail randomly because the operations of reading the value, negating it, and setting it back are not atomic as a whole.
        //While the getter and setter of the ThreadSafeBoolean are thread-safe individually, the combination of these operations(startBoolean.Value = !startBoolean.Value;) is not atomic.This means that another thread can change the value of startBoolean after it has been read and before it has been set, leading to inconsistent results.
        //To make the entire operation atomic, you would need to use a lock or a similar synchronization mechanism
        //startBoolean = !startBoolean;
        //startBoolean = !startBoolean;
        public void ToggleValue()
        {
            int original, newValue;
            do
            {
                original = _value;
                newValue = original == 1 ? 0 : 1;
            }
            while (Interlocked.CompareExchange(ref _value, newValue, original) != original);
        }


        public void SetValue(bool value)
        {
            Interlocked.Exchange(ref _value, value ? True : False);
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }

        // IComparable<ThreadSafe.Boolean> implementation
        public int CompareTo(Boolean other)
        {
            if (other == null) return 1;
            return GetValue().CompareTo(other.GetValue());
        }

        // IComparable<bool> implementation
        public int CompareTo(bool other)
        {
            return GetValue().CompareTo(other);
        }

        // Implicit conversion from bool to ThreadSafe.Boolean
        public static implicit operator Boolean(bool value)
        {
            return new Boolean(value);
        }

        // Implicit conversion from ThreadSafe.Boolean to bool
        public static implicit operator bool(Boolean tsb)
        {
            return tsb.GetValue();
        }

        // Equality check
        public override bool Equals(object obj)
        {
            if (obj is Boolean other)
            {
                return GetValue() == other.GetValue();
            }
            if (obj is bool boolValue)
            {
                return GetValue() == boolValue;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }

        // Equality operators
        public static bool operator ==(Boolean left, Boolean right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.GetValue() == right.GetValue();
        }

        public static bool operator !=(Boolean left, Boolean right)
        {
            return !(left == right);
        }

        // Comparison with bool
        public static bool operator ==(Boolean left, bool right)
        {
            if (left is null)
            {
                return false;
            }

            return left.GetValue() == right;
        }

        public static bool operator !=(Boolean left, bool right)
        {
            return !(left == right);
        }

        public static bool operator ==(bool left, Boolean right)
        {
            if (right is null)
            {
                return false;
            }

            return left == right.GetValue();
        }

        public static bool operator !=(bool left, Boolean right)
        {
            return !(left == right);
        }
    }

    //===================================================================================================================

    [DebuggerDisplay("Threadsafe.Long: {GetValue()}")]
    [JsonConverter(typeof(ThreadSafeConverter))]
    public sealed class Long:IComparable<Long>, IComparable<long>
    {
        [JsonProperty]
        private long _value;

        [DebuggerStepThrough]
        //[JsonConstructor]
        public Long(long initialValue = 0)
        {
            _value = initialValue;
        }


        public long GetValue()
        {
            return Interlocked.Read(ref _value);
        }

        public void SetValue(long value)
        {
            Interlocked.Exchange(ref _value, value);
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }

        // IComparable<ThreadSafe.Long> implementation
        public int CompareTo(Long other)
        {
            if (other == null) return 1;
            return GetValue().CompareTo(other.GetValue());
        }

        // IComparable<long> implementation
        public int CompareTo(long other)
        {
            return GetValue().CompareTo(other);
        }
        // Implicit conversion from long to ThreadSafe.Long
        public static implicit operator Long(long value)
        {
            return new Long(value);
        }

        // Implicit conversion from ThreadSafe.Long to long
        public static implicit operator long(Long tsl)
        {
            return tsl.GetValue();
        }

        // Equality check
        public override bool Equals(object obj)
        {
            if (obj is Long other)
            {
                return GetValue() == other.GetValue();
            }
            if (obj is long longValue)
            {
                return GetValue() == longValue;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }

        // Equality operators
        public static bool operator ==(Long left, Long right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.GetValue() == right.GetValue();
        }

        public static bool operator !=(Long left, Long right)
        {
            return !(left == right);
        }

        // Comparison with long
        public static bool operator ==(Long left, long right)
        {
            if (left is null)
            {
                return false;
            }

            return left.GetValue() == right;
        }

        public static bool operator !=(Long left, long right)
        {
            return !(left == right);
        }

        public static bool operator ==(long left, Long right)
        {
            if (right is null)
            {
                return false;
            }

            return left == right.GetValue();
        }

        public static bool operator !=(long left, Long right)
        {
            return !(left == right);
        }

        // Arithmetic operators
        public static Long operator +(Long left, long right)
        {
            Interlocked.Add(ref left._value, right);
            return left;
        }

        public static Long operator -(Long left, long right)
        {
            Interlocked.Add(ref left._value, -right);
            return left;
        }

        public static Long operator *(Long left, long right)
        {
            long initialValue, computedValue;
            do
            {
                initialValue = left.GetValue();
                computedValue = initialValue * right;
            } while (Interlocked.CompareExchange(ref left._value, computedValue, initialValue) != initialValue);
            return left;
        }

        public static Long operator /(Long left, long right)
        {
            long initialValue, computedValue;
            do
            {
                initialValue = left.GetValue();
                computedValue = initialValue / right;
            } while (Interlocked.CompareExchange(ref left._value, computedValue, initialValue) != initialValue);
            return left;
        }

        public static Long operator %(Long left, long right)
        {
            long initialValue, computedValue;
            do
            {
                initialValue = left.GetValue();
                computedValue = initialValue % right;
            } while (Interlocked.CompareExchange(ref left._value, computedValue, initialValue) != initialValue);
            return left;
        }
    }

    //===================================================================================================================

    //[DebuggerDisplay("Threadsafe.Long: {GetValue()}")]
    //[JsonConverter(typeof(ThreadSafeConverter))]
    //public sealed class Decimal:IComparable<Decimal>, IComparable<decimal>
    //{
    //    [JsonProperty]
    //    private long _high;
    //    [JsonProperty]
    //    private long _low;
    //    [JsonProperty]
    //    private int _flags;

    //    public Decimal(decimal initialValue = 0)
    //    {
    //        SetValue(initialValue);
    //    }

    //    public decimal GetValue()
    //    {
    //        return DecimalFromLong(_high, _low, _flags);
    //    }

    //    public void SetValue(decimal value)
    //    {
    //        (long high, long low, int flags) = LongFromDecimal(value);
    //        Interlocked.Exchange(ref _high, high);
    //        Interlocked.Exchange(ref _low, low);
    //        Interlocked.Exchange(ref _flags, flags);
    //    }

    //    public void Add(decimal value)
    //    {
    //        long initialHigh, initialLow, newHigh, newLow;
    //        int initialFlags, newFlags;
    //        decimal result;
    //        do
    //        {
    //            initialHigh = Interlocked.Read(ref _high);
    //            initialLow = Interlocked.Read(ref _low);
    //            initialFlags = Interlocked.CompareExchange(ref _flags, 0, 0);
    //            decimal current = DecimalFromLong(initialHigh, initialLow, initialFlags);
    //            result = current + value;
    //            (newHigh, newLow, newFlags) = LongFromDecimal(result);
    //        } while (
    //            Interlocked.CompareExchange(ref _high, newHigh, initialHigh) != initialHigh ||
    //            Interlocked.CompareExchange(ref _low, newLow, initialLow) != initialLow ||
    //            Interlocked.CompareExchange(ref _flags, newFlags, initialFlags) != initialFlags);
    //    }

    //    public void Subtract(decimal value)
    //    {
    //        long initialHigh, initialLow, newHigh, newLow;
    //        int initialFlags, newFlags;
    //        decimal result;
    //        do
    //        {
    //            initialHigh = Interlocked.Read(ref _high);
    //            initialLow = Interlocked.Read(ref _low);
    //            initialFlags = Interlocked.CompareExchange(ref _flags, 0, 0);
    //            decimal current = DecimalFromLong(initialHigh, initialLow, initialFlags);
    //            result = current - value;
    //            (newHigh, newLow, newFlags) = LongFromDecimal(result);
    //        } while (
    //            Interlocked.CompareExchange(ref _high, newHigh, initialHigh) != initialHigh ||
    //            Interlocked.CompareExchange(ref _low, newLow, initialLow) != initialLow ||
    //            Interlocked.CompareExchange(ref _flags, newFlags, initialFlags) != initialFlags);
    //    }

    //    private static (long high, long low, int flags) LongFromDecimal(decimal value)
    //    {
    //        int[] bits = decimal.GetBits(value);
    //        long high = ((long)bits[1] << 32) | bits[0];
    //        long low = ((long)bits[2] << 32) | bits[3];
    //        int flags = bits[3];
    //        return (high, low, flags);
    //    }

    //    private static decimal DecimalFromLong(long high, long low, int flags)
    //    {
    //        int[] bits = new int[4];
    //        bits[0] = (int)(high & 0xFFFFFFFF);
    //        bits[1] = (int)(high >> 32);
    //        bits[2] = (int)(low & 0xFFFFFFFF);
    //        bits[3] = (int)((int)(low >> 32) | (flags & 0x80000000));
    //        return new decimal(bits);
    //    }

    //    public override string ToString()
    //    {
    //        return GetValue().ToString();
    //    }

    //    // Implicit conversion from decimal to ThreadSafe.Decimal
    //    public static implicit operator Decimal(decimal value)
    //    {
    //        return new Decimal(value);
    //    }

    //    // Implicit conversion from ThreadSafe.Decimal to decimal
    //    public static implicit operator decimal(Decimal tsd)
    //    {
    //        return tsd.GetValue();
    //    }

    //    // IComparable<ThreadSafe.Decimal> implementation
    //    public int CompareTo(Decimal other)
    //    {
    //        if (other == null) return 1;
    //        return GetValue().CompareTo(other.GetValue());
    //    }

    //    // IComparable<decimal> implementation
    //    public int CompareTo(decimal other)
    //    {
    //        return GetValue().CompareTo(other);
    //    }

    //    // Equality check
    //    public override bool Equals(object obj)
    //    {
    //        if (obj is Decimal other)
    //        {
    //            return GetValue() == other.GetValue();
    //        }
    //        if (obj is decimal decimalValue)
    //        {
    //            return GetValue() == decimalValue;
    //        }
    //        return false;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return GetValue().GetHashCode();
    //    }

    //    // Equality operators
    //    public static bool operator ==(Decimal left, Decimal right)
    //    {
    //        if (ReferenceEquals(left, right))
    //        {
    //            return true;
    //        }

    //        if (left is null || right is null)
    //        {
    //            return false;
    //        }

    //        return left.GetValue() == right.GetValue();
    //    }

    //    public static bool operator !=(Decimal left, Decimal right)
    //    {
    //        return !(left == right);
    //    }

    //    // Comparison with decimal
    //    public static bool operator ==(Decimal left, decimal right)
    //    {
    //        if (left is null)
    //        {
    //            return false;
    //        }

    //        return left.GetValue() == right;
    //    }

    //    public static bool operator !=(Decimal left, decimal right)
    //    {
    //        return !(left == right);
    //    }

    //    public static bool operator ==(decimal left, Decimal right)
    //    {
    //        if (right is null)
    //        {
    //            return false;
    //        }

    //        return left == right.GetValue();
    //    }

    //    public static bool operator !=(decimal left, Decimal right)
    //    {
    //        return !(left == right);
    //    }

    //    // Arithmetic operators
    //    public static Decimal operator +(Decimal left, decimal right)
    //    {
    //        left.Add(right);
    //        return left;
    //    }

    //    public static Decimal operator -(Decimal left, decimal right)
    //    {
    //        left.Subtract(right);
    //        return left;
    //    }

    //    public static Decimal operator *(Decimal left, decimal right)
    //    {
    //        long initialHigh, initialLow, newHigh, newLow;
    //        int initialFlags, newFlags;
    //        decimal result;
    //        do
    //        {
    //            initialHigh = Interlocked.Read(ref left._high);
    //            initialLow = Interlocked.Read(ref left._low);
    //            initialFlags = Interlocked.CompareExchange(ref left._flags, 0, 0);
    //            decimal current = DecimalFromLong(initialHigh, initialLow, initialFlags);
    //            result = current * right;
    //            (newHigh, newLow, newFlags) = LongFromDecimal(result);
    //        } while (
    //            Interlocked.CompareExchange(ref left._high, newHigh, initialHigh) != initialHigh ||
    //            Interlocked.CompareExchange(ref left._low, newLow, initialLow) != initialLow ||
    //            Interlocked.CompareExchange(ref left._flags, newFlags, initialFlags) != initialFlags);
    //        return left;
    //    }

    //    public static Decimal operator /(Decimal left, decimal right)
    //    {
    //        long initialHigh, initialLow, newHigh, newLow;
    //        int initialFlags, newFlags;
    //        decimal result;
    //        do
    //        {
    //            initialHigh = Interlocked.Read(ref left._high);
    //            initialLow = Interlocked.Read(ref left._low);
    //            initialFlags = Interlocked.CompareExchange(ref left._flags, 0, 0);
    //            decimal current = DecimalFromLong(initialHigh, initialLow, initialFlags);
    //            result = current / right;
    //            (newHigh, newLow, newFlags) = LongFromDecimal(result);
    //        } while (
    //            Interlocked.CompareExchange(ref left._high, newHigh, initialHigh) != initialHigh ||
    //            Interlocked.CompareExchange(ref left._low, newLow, initialLow) != initialLow ||
    //            Interlocked.CompareExchange(ref left._flags, newFlags, initialFlags) != initialFlags);
    //        return left;
    //    }
    //}


}

//===================================================================================================================

public class ThreadSafeConverter:JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ThreadSafe.Integer) ||
               objectType == typeof(ThreadSafe.Long) ||
               objectType == typeof(ThreadSafe.Boolean) ||
               objectType == typeof(ThreadSafe.DateTime);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is ThreadSafe.Integer tsi)
        {
            writer.WriteValue(tsi.GetValue());
        }
        else if (value is ThreadSafe.Long tsl)
        {
            writer.WriteValue(tsl.GetValue());
        }
        else if (value is ThreadSafe.Boolean tsb)
        {
            writer.WriteValue(tsb.GetValue());
        }
        else if (value is ThreadSafe.DateTime tsd)
        {
            writer.WriteValue(tsd.GetValue().ToBinary());
        }
        //else if (value is ThreadSafe.Decimal tsc)
        //{
        //    writer.WriteValue(tsc.GetValue().ToString(CultureInfo.InvariantCulture));
        //}
        else
        {
            throw new JsonSerializationException("Unsupported type");
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject)
        {
            // Read the old format (with private fields)
            var jsonObject = JObject.Load(reader);
            if (objectType == typeof(ThreadSafe.Integer))
            {
                var value = jsonObject["_value"].ToObject<int>();
                return new ThreadSafe.Integer(value);
            }
            else if (objectType == typeof(ThreadSafe.Long))
            {
                var value = jsonObject["_value"].ToObject<long>();
                return new ThreadSafe.Long(value);
            }
            else if (objectType == typeof(ThreadSafe.Boolean))
            {
                var value = jsonObject["_value"].ToObject<bool>();
                return new ThreadSafe.Boolean(value);
            }
            else if (objectType == typeof(ThreadSafe.DateTime))
            {
                var value = jsonObject["_value"].ToObject<long>();
                return new ThreadSafe.DateTime(System.DateTime.FromBinary(value), "dd.MM.yy, HH:mm:ss");
            }
        }
        else if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Boolean || reader.TokenType == JsonToken.String)
        {
            // Read the new format (just the value)
            if (objectType == typeof(ThreadSafe.Integer))
            {
                return new ThreadSafe.Integer(Convert.ToInt32(reader.Value));
            }
            else if (objectType == typeof(ThreadSafe.Long))
            {
                return new ThreadSafe.Long(Convert.ToInt64(reader.Value));
            }
            else if (objectType == typeof(ThreadSafe.Boolean))
            {
                return new ThreadSafe.Boolean(Convert.ToBoolean(reader.Value));
            }
            else if (objectType == typeof(ThreadSafe.DateTime))
            {
                return new ThreadSafe.DateTime(System.DateTime.FromBinary(Convert.ToInt64(reader.Value)), "dd.MM.yy, HH:mm:ss");
            }
            //else if (objectType == typeof(ThreadSafe.Decimal))
            //{
            //    return new ThreadSafe.Decimal(decimal.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture));
            //}
        }

        throw new JsonSerializationException("Unsupported type");
    }
}


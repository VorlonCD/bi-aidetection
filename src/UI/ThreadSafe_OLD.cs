using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;


public class ThreadSafe_OLD

{
    public static int Maximum(ref int target, int value)
    {
        int currentVal = target, startVal, desiredVal;
        do
        {
            startVal = currentVal;
            desiredVal = Math.Max(startVal, value);
            currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
        } while (startVal != currentVal);
        return desiredVal;
    }

    public static long Maximum(ref long target, long value)
    {
        long currentVal = target, startVal, desiredVal;
        do
        {
            startVal = currentVal;
            desiredVal = Math.Max(startVal, value);
            currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
        } while (startVal != currentVal);
        return desiredVal;
    }

    public static int Minimum(ref int target, int value)
    {
        int currentVal = target, startVal, desiredVal;
        do
        {
            startVal = currentVal;
            desiredVal = Math.Min(startVal, value);
            currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
        } while (startVal != currentVal);
        return desiredVal;
    }

    public static long Minimum(ref long target, long value)
    {
        long currentVal = target, startVal, desiredVal;
        do
        {
            startVal = currentVal;
            desiredVal = Math.Min(startVal, value);
            currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
        } while (startVal != currentVal);
        return desiredVal;
    }

    /// <summary>
    /// A Datetime value that is ThreadSafe_OLD - Note only has 20ms resolution because uses Ticks
    /// </summary>
    /// https://stackoverflow.com/questions/1531668/thread-safe-datetime-update-using-interlocked
    public class DateTime
    {
        public long _value;
        private string _dateFormat = "";
        [DebuggerStepThrough]
        public DateTime(System.DateTime value, string dateFormat = "dd.MM.yy, HH:mm:ss")
        {
            this._value = value.ToBinary();
            _dateFormat = dateFormat;
        }
        [DebuggerStepThrough]
        public void Write(System.DateTime value)
        {
            Interlocked.Exchange(ref this._value, value.ToBinary());
        }
        [DebuggerStepThrough]
        public System.DateTime Read()
        {
            long lastvalue = Interlocked.CompareExchange(ref this._value, 0, 0);
            return System.DateTime.FromBinary(lastvalue);

        }
        [DebuggerStepThrough]
        public override string ToString()
        {
            return $"{this.Read().ToString(_dateFormat)}";
        }
    }
    public class Integer
    {
        public int _value;
        [DebuggerStepThrough]
        public Integer(int value)
        {
            this._value = value;
        }
        [DebuggerStepThrough]
        public int ReadUnfenced()
        {
            return this._value;
        }

        public int ReadAcquireFence()
        {
            var value = this._value;
            Thread.MemoryBarrier();
            return value;
        }
        [DebuggerStepThrough]
        public int ReadFullFence()
        {
            var value = this._value;
            Thread.MemoryBarrier();
            return value;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public int ReadCompilerOnlyFence()
        {
            return this._value;
        }

        public void WriteReleaseFence(int newValue)
        {
            this._value = newValue;
            Thread.MemoryBarrier();
        }
        [DebuggerStepThrough]
        public void WriteFullFence(int newValue)
        {
            this._value = newValue;
            Thread.MemoryBarrier();
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WriteCompilerOnlyFence(int newValue)
        {
            this._value = newValue;
        }
        [DebuggerStepThrough]
        public void WriteUnfenced(int newValue)
        {
            this._value = newValue;
        }

        public bool AtomicCompareExchange(int newValue, int comparand)
        {
            return Interlocked.CompareExchange(ref this._value, newValue, comparand) == comparand;
        }

        public int AtomicExchange(int newValue)
        {
            return Interlocked.Exchange(ref this._value, newValue);
        }

        public int AtomicAddAndGet(int delta)
        {
            return Interlocked.Add(ref this._value, delta);
        }

        public int AtomicIncrementAndGet()
        {
            return Interlocked.Increment(ref this._value);
        }

        public int AtomicDecrementAndGet()
        {
            return Interlocked.Decrement(ref this._value);
        }

        public int Maximum(int newValue)
        {
            return ThreadSafe_OLD.Maximum(ref this._value, newValue);
        }

        public int Minimum(int newValue)
        {
            return ThreadSafe_OLD.Minimum(ref this._value, newValue);
        }

        public override string ToString()
        {
            var value = this.ReadFullFence();
            return value.ToString();
        }
    }

    public class Long
    {
        public long _value;

        public Long(long value)
        {
            this._value = value;
        }

        public long ReadUnfenced()
        {
            return this._value;
        }

        public long ReadAcquireFence()
        {
            var value = this._value;
            Thread.MemoryBarrier();
            return value;
        }

        public long ReadFullFence()
        {
            Thread.MemoryBarrier();
            return this._value;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public long ReadCompilerOnlyFence()
        {
            return this._value;
        }

        public void WriteReleaseFence(long newValue)
        {
            Thread.MemoryBarrier();
            this._value = newValue;
        }

        public void WriteFullFence(long newValue)
        {
            Thread.MemoryBarrier();
            this._value = newValue;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WriteCompilerOnlyFence(long newValue)
        {
            this._value = newValue;
        }

        public void WriteUnfenced(long newValue)
        {
            this._value = newValue;
        }

        public bool AtomicCompareExchange(long newValue, long comparand)
        {
            return Interlocked.CompareExchange(ref this._value, newValue, comparand) == comparand;
        }

        public long AtomicExchange(long newValue)
        {
            return Interlocked.Exchange(ref this._value, newValue);
        }

        public long AtomicAddAndGet(long delta)
        {
            return Interlocked.Add(ref this._value, delta);
        }

        public long AtomicIncrementAndGet()
        {
            return Interlocked.Increment(ref this._value);
        }

        public long AtomicDecrementAndGet()
        {
            return Interlocked.Decrement(ref this._value);
        }

        public long Maximum(long newValue)
        {
            return ThreadSafe_OLD.Maximum(ref this._value, newValue);
        }

        public long Minimum(long newValue)
        {
            return ThreadSafe_OLD.Minimum(ref this._value, newValue);
        }

        public override string ToString()
        {
            var value = this.ReadFullFence();
            return value.ToString();
        }
    }

    public class Boolean
    {
        public int _value;
        private const int False = 0;
        private const int True = 1;
        [DebuggerStepThrough]
        public Boolean(bool value)
        {
            this._value = value ? True : False;
        }


        public bool ReadUnfenced()
        {
            return ToBool(this._value);
        }

        public bool ReadAcquireFence()
        {
            var value = ToBool(this._value);
            Thread.MemoryBarrier();
            return value;
        }
        [DebuggerStepThrough]
        public bool ReadFullFence()
        {
            var value = ToBool(this._value);
            Thread.MemoryBarrier();
            return value;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public bool ReadCompilerOnlyFence()
        {
            return ToBool(this._value);
        }

        public void WriteReleaseFence(bool newValue)
        {
            var newValueInt = ToInt(newValue);
            Thread.MemoryBarrier();
            this._value = newValueInt;
        }
        [DebuggerStepThrough]
        public void WriteFullFence(bool newValue)
        {
            var newValueInt = ToInt(newValue);
            Thread.MemoryBarrier();
            this._value = newValueInt;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WriteCompilerOnlyFence(bool newValue)
        {
            this._value = ToInt(newValue);
        }

        public void WriteUnfenced(bool newValue)
        {
            this._value = ToInt(newValue);
        }

        public bool AtomicCompareExchange(bool newValue, bool comparand)
        {
            var newValueInt = ToInt(newValue);
            var comparandInt = ToInt(comparand);

            return Interlocked.CompareExchange(ref this._value, newValueInt, comparandInt) == comparandInt;
        }

        public bool AtomicExchange(bool newValue)
        {
            var newValueInt = ToInt(newValue);
            var originalValue = Interlocked.Exchange(ref this._value, newValueInt);
            return ToBool(originalValue);
        }
        [DebuggerStepThrough]
        public override string ToString()
        {
            return $"{this.ReadFullFence()}";
        }
        [DebuggerStepThrough]
        private static bool ToBool(int value)
        {
            if (value != False && value != True)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return value == True;
        }
        [DebuggerStepThrough]
        private static int ToInt(bool value)
        {
            return value ? True : False;
        }

        public bool CompareAndSet(bool comparand, bool newValue)
        {
            var newValueInt = ToInt(newValue);
            var comparandInt = ToInt(comparand);

            return Interlocked.CompareExchange(ref this._value, newValueInt, comparandInt) == comparandInt;
        }
    }

    public class AtomicReference<T>
        where T : class
    {
        public T _value;

        public AtomicReference(T value)
        {
            this._value = value;
        }

        public T ReadUnfenced()
        {
            return this._value;
        }

        public T ReadAcquireFence()
        {
            var value = this._value;
            Thread.MemoryBarrier();
            return value;
        }

        public T ReadFullFence()
        {
            var value = this._value;
            Thread.MemoryBarrier();
            return value;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public T ReadCompilerOnlyFence()
        {
            return this._value;
        }

        public void WriteReleaseFence(T newValue)
        {
            this._value = newValue;
            Thread.MemoryBarrier();
        }

        public void WriteFullFence(T newValue)
        {
            this._value = newValue;
            Thread.MemoryBarrier();
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void WriteCompilerOnlyFence(T newValue)
        {
            this._value = newValue;
        }

        public void WriteUnfenced(T newValue)
        {
            this._value = newValue;
        }

        public bool AtomicCompareExchange(T newValue, T comparand)
        {
            return Interlocked.CompareExchange(ref this._value, newValue, comparand) == comparand;
        }

        public T AtomicExchange(T newValue)
        {
            return Interlocked.Exchange(ref this._value, newValue);
        }

        public bool CompareAndSet(T comparand, T newValue)
        {
            return Interlocked.CompareExchange(ref this._value, newValue, comparand) == comparand;
        }
    }
}


using System.Diagnostics;


using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ThreadSafeTesting
{
    public class test
    {

        public ThreadSafe.Long lng2 { get; set; }
        public ThreadSafe_OLD.Long lng { get; set; }
        public long lngreal { get; set; }
        public ThreadSafe.Integer intg2 { get; set; }
        public ThreadSafe_OLD.Integer intg { get; set; }
        public int intgreal { get; set; }
        public ThreadSafe.Boolean boolg2 { get; set; }
        public ThreadSafe_OLD.Boolean boolg { get; set; }
        public bool boolgreal { get; set; }
        public ThreadSafe.DateTime dt2 { get; set; }
        public ThreadSafe_OLD.DateTime dt { get; set; }
        public System.DateTime dtreal { get; set; }
        //public ThreadSafe.Decimal decimal2 { get; set; }
        public decimal decimalreal { get; set; }

    }

    public class ThreadSafeTestProgram
    {
        const int Iterations = 1000000;
        const int ThreadCount = 4;
        const int MaxResults = 5;
        //const string LockResultsFile = "lockResults.txt";
        const string InterlockedResultsFile = "interlockedResults.txt";
        const string DateTimeResultsFile = "dateTimeResults.txt";

        //static List<int> lockResults = new List<int>();
        static List<int> interlockedResults = new List<int>();
        static List<int> dateTimeResults = new List<int>();

        static void Main(string[] args)
        {

            test tc = new test();
            tc.intg = new ThreadSafe_OLD.Integer(1);
            tc.intg2 = new ThreadSafe.Integer(1);
            tc.intgreal = 1;
            tc.lng = new ThreadSafe_OLD.Long(1);
            tc.lng2 = new ThreadSafe.Long(2);
            tc.lngreal = 1;
            tc.boolg = new ThreadSafe_OLD.Boolean(true);
            tc.boolg2 = new ThreadSafe.Boolean(true);
            tc.boolgreal = true;
            tc.dt = new ThreadSafe_OLD.DateTime(System.DateTime.Now);
            tc.dt2 = new ThreadSafe.DateTime(System.DateTime.Now, "dd.MM.yy, HH:mm:ss");
            tc.dtreal = System.DateTime.Now;
            //tc.decimal2 = new ThreadSafe.Decimal(1.000321555m);
            tc.decimalreal = 1.000321555m;

            // Create and serialize a new instance of test class using Newtonsoft.Json
            string serializedTest = GetJSONString(tc); //JsonConvert.SerializeObject(testcls);

            //serializedTest = "{\r\n  \"$id\": \"1\",\r\n  \"$type\": \"ThreadSafeTesting.test, ThreadSafeTesting\",\r\n  \"lng2\": {\r\n    \"$id\": \"2\",\r\n    \"$type\": \"AITool.ThreadSafe+Long, ThreadSafeTesting\",\r\n    \"_value\": 99\r\n  },\r\n  \"lng\": {\r\n    \"$id\": \"3\",\r\n    \"$type\": \"AITool.ThreadSafe+Long, ThreadSafeTesting\",\r\n    \"_value\": 1\r\n  },\r\n  \"lngreal\": 1,\r\n  \"intg2\": 1,\r\n  \"intg\": {\r\n    \"$id\": \"4\",\r\n    \"$type\": \"AITool.ThreadSafe+Integer, ThreadSafeTesting\",\r\n    \"_value\": 1\r\n  },\r\n  \"intgreal\": 1,\r\n  \"boolg2\": true,\r\n  \"boolg\": {\r\n    \"$id\": \"5\",\r\n    \"$type\": \"AITool.ThreadSafe+Boolean, ThreadSafeTesting\",\r\n    \"_value\": 1\r\n  },\r\n  \"boolgreal\": true,\r\n  \"dt2\": -8584851288971756652,\r\n  \"dt\": {\r\n    \"$id\": \"6\",\r\n    \"$type\": \"AITool.ThreadSafe+Datetime, ThreadSafeTesting\",\r\n    \"_value\": -8584851288971698129\r\n  },\r\n  \"dtreal\": \"2024-05-23T11:26:28.3078034-04:00\"\r\n}";

            // Deserialize the serialized test instance
            test ntc = SetJSONString<test>(serializedTest);

            //check each property of the deserialized object to make sure it matches:
            bool propsequal = tc.lng.ReadFullFence() == ntc.lng.ReadFullFence() &&
                              tc.lng2 == ntc.lng2 &&
                              tc.lngreal == ntc.lngreal &&
                              tc.intg.ReadFullFence() == ntc.intg.ReadFullFence() &&
                              tc.intg2 == ntc.intg2 &&
                              tc.intgreal == ntc.intgreal &&
                              tc.boolg.ReadFullFence() == ntc.boolg.ReadFullFence() &&
                              tc.boolg2.ToString() == ntc.boolg2.ToString() &&
                              tc.boolgreal == ntc.boolgreal &&
                              tc.decimalreal == ntc.decimalreal;

            bool dt1eq = tc.dt.Read() == ntc.dt.Read();
            bool dt2eq = tc.dt2 == ntc.dt2;
            bool dt3eq = tc.dtreal == ntc.dtreal;

            // Compare the original and deserialized test instances
            bool areEqual = tc.Equals(ntc);

            Console.WriteLine($"Are the original and deserialized test instances equal? {areEqual}");



            //Add a 5m delay to allow the debugger to attach, etc
            Console.WriteLine("Waiting...");
            Thread.Sleep(1000);

            Console.WriteLine("Starting validation tests...");


            // Output validation results
            for (int i = 0; i < 15; i++)
            {
                Console.WriteLine($"    ThreadSafe.Boolean validation passed: {TestThreadSafetyForBoolean(out long boolms)} ({boolms}ms)");
            }
            //for (int i = 0; i < 15; i++)
            //{
            //    Console.WriteLine($"  ThreadSafe.BooleanMB validation passed: {TestThreadSafetyForBooleanMemBarrier(out long boolmbms)} ({boolmbms}ms)");
            //}
            //for (int i = 0; i < 15; i++)
            //{
            //    Console.WriteLine($"  ThreadSafe.Boolean2 validation passed: {TestThreadSafetyForBoolean2(out long boolmbms2)} ({boolmbms2}ms)");
            //}

            //return;

            // Validate ThreadSafeIntegerWithInterlocked
            bool interlockedValidationResult = Validate(() => new ThreadSafe.Integer(), tsi =>
            {
                for (int i = 0; i < Iterations; i++)
                {
                    tsi++;
                    tsi--;
                    tsi += 5;
                    tsi -= 5;
                    //tsi *= 2;
                    //tsi /= 2;
                }
            }, out long intms);

            // Output validation results

            /// Validate ThreadSafe.DateTime
            bool dateTimeValidationResult = Validate(() => new ThreadSafe.DateTime(System.DateTime.Now, "dd.MM.yy, HH:mm:ss"), tsd =>
            {
                for (int i = 0; i < Iterations; i++)
                {
                    tsd.AddDays(1);
                    tsd.AddDays(-1);
                    tsd.AddHours(2);
                    tsd.AddHours(-2);
                    tsd.AddMinutes(30);
                    tsd.AddMinutes(-30);
                    tsd.AddSeconds(45);
                    tsd.AddSeconds(-45);

                    // Equality and comparison checks
                    var now = new ThreadSafe.DateTime(System.DateTime.Now, "dd.MM.yy, HH:mm:ss");
                    var later = new ThreadSafe.DateTime(now.GetValue().AddHours(1), "dd.MM.yy, HH:mm:ss");
                    bool isEqual = now == now;
                    bool isNotEqual = now != later;
                    bool isLessThan = now < later;
                    bool isGreaterThan = later > now;
                    bool isLessThanOrEqual = now <= now;
                    bool isGreaterThanOrEqual = later >= now;

                    // Addition and subtraction checks
                    var addedTime = now + TimeSpan.FromHours(1);
                    var subtractedTime = later - TimeSpan.FromHours(1);
                    TimeSpan difference = later - now;

                    // Ensure correctness
                    if (!isEqual || !isNotEqual || !isLessThan || !isGreaterThan || !isLessThanOrEqual ||
                        !isGreaterThanOrEqual || !(addedTime == later) || !(subtractedTime == now) ||
                        !(difference == TimeSpan.FromHours(1)))
                    {
                        throw new Exception("Validation failed.");
                    }

                    // Implicit conversion checks
                    System.DateTime dateTimeFromThreadSafe = now;
                    var threadSafeFromDateTime = (ThreadSafe.DateTime)dateTimeFromThreadSafe;
                    if (!(now == threadSafeFromDateTime))
                    {
                        throw new Exception("Validation failed.");
                    }
                }
            }, out long datems);



            Console.WriteLine($"    ThreadSafe.Integer validation passed: {interlockedValidationResult} ({intms}ms)");
            Console.WriteLine($"   ThreadSafe.DateTime validation passed: {dateTimeValidationResult} ({datems}ms)");
            Console.WriteLine($"ThreadSafe.DateTime validation #2 passed: {TestThreadSafetyForDateTime(out long date2ms)} ({date2ms}ms)");
            Console.WriteLine($"       ThreadSafe.Long validation passed: {TestThreadSafetyForLong(out long longms)} ({longms}ms)");

            Console.WriteLine("Waiting...");
            Thread.Sleep(5000);

            Console.WriteLine("Starting benchmark...");


            // Load previous results
            LoadResults(InterlockedResultsFile, interlockedResults);
            LoadResults(DateTimeResultsFile, dateTimeResults);

            //ThreadSafeIntegerWithInterlocked blah = 1;

            // Benchmark ThreadSafeIntegerWithInterlocked
            var interlockedResult = Benchmark(() => new ThreadSafe.Integer(), tsi =>
            {
                for (int i = 0; i < Iterations; i++)
                {
                    tsi++;
                    tsi--;
                    tsi += 5;
                    tsi -= 5;
                    //tsi *= 2;
                    //tsi /= 2;
                }
            });

            AddResult(interlockedResults, interlockedResult);
            // Save results
            SaveResults(InterlockedResultsFile, interlockedResults);

            // Output results
            DisplayResults("ThreadSafeIntegerWithInterlocked", interlockedResults);

            // Benchmark ThreadSafeDateTime
            var dateTimeResult = Benchmark(() => (ThreadSafe.DateTime)DateTime.Now, tsd =>
            {
                for (int i = 0; i < Iterations; i++)
                {
                    tsd.AddSeconds(1);
                    tsd.AddSeconds(-1);
                }
            });
            AddResult(dateTimeResults, dateTimeResult);

            // Save results
            SaveResults(DateTimeResultsFile, dateTimeResults);

            // Output results
            DisplayResults("ThreadSafeDateTime", dateTimeResults);

        }

        //this may speed up json serialization
        public static readonly DefaultContractResolver JSONContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = false,
                OverrideSpecifiedNames = false,
                ProcessExtensionDataNames = false
            }

            //Global.JSONContractResolver.NamingStrategy = new CamelCaseNamingStrategy();
        };
        public static readonly JsonSerializerSettings JSONSettingsPretty = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ContractResolver = JSONContractResolver,
            //NullValueHandling = NullValueHandling.Ignore,
            //DefaultValueHandling = DefaultValueHandling.Ignore,
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // Add other settings that may improve performance for your specific case
        };

        public static string GetJSONString(object cls2, Newtonsoft.Json.Formatting formatting = Formatting.Indented,
                                                        Newtonsoft.Json.TypeNameHandling handling = TypeNameHandling.All,
                                                        Newtonsoft.Json.PreserveReferencesHandling reference = PreserveReferencesHandling.Objects)
        {

            string Ret = "";
            try
            {

                JSONSettingsPretty.Formatting = formatting;
                JSONSettingsPretty.TypeNameHandling = handling;
                JSONSettingsPretty.PreserveReferencesHandling = reference;
                JSONSettingsPretty.Error = null;

                string contents2 = JsonConvert.SerializeObject(cls2, formatting, JSONSettingsPretty);

                if (JSONSettingsPretty.Error == null)
                {
                    Ret = contents2;
                }
                else
                {
                    Console.WriteLine($"Error: " + JSONSettingsPretty.Error.ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message);
            }
            finally
            {
            }

            return Ret;

        }

        public static T SetJSONString<T>(string JSONString) where T : new()
        {


            T Ret = default(T);

            try
            {
                //JsonSerializerSettings jset = new JsonSerializerSettings { };
                //jset.TypeNameHandling = TypeNameHandling.All;
                //jset.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                //jset.ContractResolver = Global.JSONContractResolver;
                JSONSettingsPretty.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
                Ret = JsonConvert.DeserializeObject<T>(JSONString, JSONSettingsPretty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: While converting json string '{JSONString}', got: " + ex.Message);
            }
            finally
            {
            }

            return Ret;

        }
        static bool TestThreadSafetyForDateTime(out long milliseconds)
        {
            Stopwatch sw = Stopwatch.StartNew();
            //const int iterations = 10000;
            //const int threadCount = 10;

            DateTime now = DateTime.Now;
            var startDateTime = new ThreadSafe.DateTime(now, "dd.MM.yy, HH:mm:ss");
            var endDateTime = new ThreadSafe.DateTime(startDateTime.GetValue().AddHours(10), "dd.MM.yy, HH:mm:ss");

            var tasks = new Task[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < Iterations; j++)
                    {
                        startDateTime.AddSeconds(1);
                        startDateTime.AddSeconds(-1);
                        endDateTime.AddSeconds(-1);
                        endDateTime.AddSeconds(1);
                    }
                });
            }

            Task.WaitAll(tasks);

            // Ensure the final values are unchanged due to the balanced operations
            bool startDateTimeUnchanged = startDateTime.GetValue() == now;
            bool endDateTimeUnchanged = endDateTime.GetValue() == now.AddHours(10);

            milliseconds = sw.ElapsedMilliseconds;
            return startDateTimeUnchanged && endDateTimeUnchanged;
        }

        static bool TestThreadSafetyForLong(out long milliseconds)
        {
            Stopwatch sw = Stopwatch.StartNew();
            //const int iterations = 10000;
            //const int threadCount = 10;
            var startLong = new ThreadSafe.Long(1000);
            var endLong = new ThreadSafe.Long(5000);

            var tasks = new Task[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < Iterations; j++)
                    {
                        startLong += 1;
                        startLong -= 1;
                        endLong += 1;
                        endLong -= 1;

                        //Atomic Operations on Long: For ThreadSafe.Long, although the addition and subtraction are balanced, the multiplication and division might not be as atomic as required because they involve multiple steps.
                        //startLong *= 2;
                        //startLong /= 2;
                        //endLong *= 2;
                        //endLong /= 2;
                    }
                });
            }

            Task.WaitAll(tasks);

            // Ensure the final values are unchanged due to the balanced operations
            bool startLongUnchanged = startLong.GetValue() == 1000;
            bool endLongUnchanged = endLong.GetValue() == 5000;

            milliseconds = sw.ElapsedMilliseconds;
            return startLongUnchanged && endLongUnchanged;
        }

        static bool TestThreadSafetyForBoolean(out long milliseconds)
        {
            Stopwatch sw = Stopwatch.StartNew();
            //const int iterations = 10000;
            //const int threadCount = 10;
            var startBoolean = new ThreadSafe.Boolean(true);

            var tasks = new Task[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < Iterations; j++)
                    {
                        //The test may fail randomly because the operations of reading the value, negating it, and setting it back are not atomic as a whole.
                        //While the getter and setter of the ThreadSafeBoolean are thread-safe individually, the combination of these operations(startBoolean.Value = !startBoolean.Value;) is not atomic.This means that another thread can change the value of startBoolean after it has been read and before it has been set, leading to inconsistent results.
                        //To make the entire operation atomic, you would need to use a lock or a similar synchronization mechanism
                        //startBoolean = !startBoolean;
                        //startBoolean = !startBoolean;
                        startBoolean = false;
                        startBoolean = true;
                        startBoolean.ToggleValue();
                        startBoolean.ToggleValue();
                    }
                });
            }

            Task.WaitAll(tasks);

            // Ensure the final value is unchanged due to the balanced operations
            bool startBooleanUnchanged = startBoolean == true;
            milliseconds = sw.ElapsedMilliseconds;

            return startBooleanUnchanged;
        }



        static bool Validate<T>(Func<T> createInstance, Action<T> action, out long milliseconds) where T : class
        {
            Stopwatch sw = Stopwatch.StartNew();
            T instance = createInstance();
            var tasks = new Task[ThreadCount];

            for (int i = 0; i < ThreadCount; i++)
            {
                tasks[i] = Task.Run(() => action(instance));
            }

            Task.WaitAll(tasks);

            // The expected value should be the same as the initial value because each operation set is balanced
            if (instance is ThreadSafe.Integer)
            {
                int expectedValue = 0;
                int actualValue = (instance as dynamic).GetValue();
                milliseconds = sw.ElapsedMilliseconds;
                return expectedValue == actualValue;
            }
            else if (instance is ThreadSafe.DateTime)
            {
                System.DateTime initialValue = System.DateTime.Now;
                (instance as dynamic).SetValue(initialValue);
                System.DateTime expectedValue = initialValue;
                System.DateTime actualValue = (instance as dynamic).GetValue();
                milliseconds = sw.ElapsedMilliseconds;
                return expectedValue == actualValue;
            }

            milliseconds = sw.ElapsedMilliseconds;
            return false;
        }

        static void SaveResults(string fileName, List<int> results)
        {
            using (var writer = new StreamWriter(fileName))
            {
                foreach (var result in results)
                {
                    writer.WriteLine(result);
                }
            }
        }

        static void LoadResults(string fileName, List<int> results)
        {
            if (File.Exists(fileName))
            {
                using (var reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (int.TryParse(line, out var result))
                        {
                            results.Add(result);
                        }
                    }
                }
            }
        }
        static TimeSpan Benchmark<T>(Func<T> createInstance, Action<T> action)
        {
            T instance = createInstance();
            var stopwatch = Stopwatch.StartNew();

            var tasks = new Task[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                tasks[i] = Task.Run(() => action(instance));
            }

            Task.WaitAll(tasks);
            stopwatch.Stop();

            return stopwatch.Elapsed;
        }

        static void DisplayResults(string name, List<int> results)
        {
            Console.WriteLine($"\n{name} Results:");
            for (int i = 0; i < results.Count - 1; i++)
            {
                Console.WriteLine($"{i + 1}: {results[i]} ms");
            }

            if (results.Count > 1)
            {
                var lastResult = results[results.Count - 2];
                var currentResult = results[results.Count - 1];
                var percentageDifference = ((double)(currentResult - lastResult) / lastResult) * 100;
                var sign = percentageDifference >= 0 ? "+" : "-";
                Console.WriteLine($"{results.Count}: {currentResult} ms ({sign}{Math.Abs(percentageDifference):0}%)");
            }
            else
            {
                Console.WriteLine($"{results.Count}: {results[results.Count - 1]} ms");
            }
        }

        static void AddResult(List<int> results, TimeSpan result)
        {
            if (results.Count == MaxResults)
            {
                results.RemoveAt(0);
            }
            results.Add((int)result.TotalMilliseconds);
        }
    }

}

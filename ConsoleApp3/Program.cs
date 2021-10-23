using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            Deferred d = new Deferred();
            d.Then(res => {
                Console.WriteLine("1 " + res);
                Deferred d1 = new Deferred();
                var t = new System.Timers.Timer()
                {
                    AutoReset = false,
                    Interval = 1500,
                };

                t.Elapsed += (s, e) => 
                d1.Resolve("a");
                t.Start();

                return d1;
            });
            d.Then(res => { Console.WriteLine("2 " + res); return "b"; });
            d.Then(res => { Console.WriteLine("3 " + res); return "c"; });
            d.Resolve("hello");
        }

        class Deferred
        {
            public List<Func<string, object>> list = new List<Func<string, object>>();
            public string _res;
            private object _timerLock = new object();

            public void Then(Func<string, object> func)
            {
                list.Add(func);
            }

            public void Resolve(string res)
            {
                string tempRes = res;
                _res = res;

                foreach (Func <string, object> func in list)
{
                    var result = func(tempRes);

                    if (result is Deferred)
                    {
                        while (((Deferred)result)._res == null) ; 
                        tempRes = ((Deferred)result)._res;
                    }

                    if (result is string)
                    {
                        tempRes = result.ToString();
                    }
                }
            }
        }

    }
}

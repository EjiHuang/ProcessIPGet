using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IphlpapiEx.Framworks
{
    public class CachedFunc<TKey, TValue> : IDisposable
    {
        // 这个字典用于存放缓存
        private Dictionary<TKey, Tuple<TValue, DateTime>> _cache =
            new Dictionary<TKey, Tuple<TValue, DateTime>>();

        // 声明委托函数
        private readonly Func<TKey, TValue> _func;
        private readonly Task _check_timeout;

        // 是否退出
        private bool _quit;

        // 声明超时参数
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="func">用于缓存方法的传入函数</param>
        public CachedFunc(Func<TKey, TValue> func)
        {
            _quit = false;
            _func = func;
            _check_timeout = Task.Factory.StartNew(() =>
            {
                while (!_quit)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(250));
                    CheckTimeOut();
                }
            });
        }

        /// <summary>
        /// 超时检查函数
        /// </summary>
        private void CheckTimeOut()
        {
            if (!Timeout.HasValue) return;

            lock (_cache)
            {
                var query_time_out =
                    from kv in _cache
                    where DateTime.Now - kv.Value.Item2 > Timeout.Value
                    select kv.Key;
                query_time_out.ToList().ForEach(key => _cache.Remove(key));
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_cache)
                {
                    if (_cache.ContainsKey(key))
                    {
                        return _cache[key].Item1;
                    }
                    else
                    {
                        TValue value = _func(key);
                        _cache.Add(key, Tuple.Create(value, DateTime.Now));
                        return value;
                    }
                }
                
            }
            set
            {
                lock (_cache)
                {
                    _cache[key] = Tuple.Create(value, DateTime.Now);
                }
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Dispose()
        {
            _quit = true;
            _check_timeout.Wait();
            lock (_cache)
            {
                _cache.Clear();
            }
            _cache = null;
        }
    }
}

using IphlpapiEx;
using IphlpapiEx.Framworks;
using System;
using System.Diagnostics;

namespace IphlpapiEx.Models
{
    /// <summary>
    /// Reference
    /// https://github.com/ssashir06/TcpConnectionInformationEx autor：ssashir06
    /// </summary>
    public class ProcessIPInfo : TcpConnectionInformationEx
    {
        /// <summary>
        /// 定义带缓存方法
        /// </summary>
        private static readonly CachedFunc<int, Process> _process_cache =
            new CachedFunc<int, Process>(Process.GetProcessById)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

        /// <summary>
        /// 延迟进程信息
        /// </summary>
        private readonly Lazy<Process> _process;

        public ProcessIPInfo(TcpConnectionInformationEx src) : base(src)
        {
            _process = new Lazy<Process>(() =>
            {
                try
                {
                    return _process_cache[Pid];
                }
                catch (ArgumentException)
                {
                    _process_cache[Pid] = null;
                    throw;
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            });
        }

        /// <summary>
        /// 获取进程
        /// </summary>
        public Process Process => _process.Value;

        /// <summary>
        /// 进程名
        /// </summary>
        public string ProcessName
            => Process == null ? string.Empty : Process.ProcessName;

        #region Operators / Overrides

        /// <summary>
        /// 运算符等号用于比较ViewTcpConnectionInformationEx类型
        /// </summary>
        public static bool operator ==(ProcessIPInfo a, ProcessIPInfo b)
        {
            return a.RemoteEndPoint.ToString() == b.RemoteEndPoint.ToString()
                           && a.LocalEndPoint.ToString() == b.LocalEndPoint.ToString()
                           && a.State == b.State
                           && a.Pid == b.Pid;
        }

        /// <summary>
        /// 运算符不等号用于比较ViewTcpConnectionInformationEx类型
        /// </summary>
        public static bool operator !=(ProcessIPInfo a, ProcessIPInfo b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is ProcessIPInfo
                           && (obj as ProcessIPInfo) == this;
        }

        /// <summary>
        /// 重载输出字符串
        /// </summary>
        public override string ToString()
        {
            return String.Format(
                "LocalEndPoint: {1}{0}" +
                "RemoteEndPoint: {2}{0}" +
                "State: {3}{0}" +
                "Pid:{4}{0}" +
                "Process: {5}",
                Environment.NewLine,
                LocalEndPoint.ToString(),
                RemoteEndPoint.ToString(),
                State,
                Pid,
                Process.ProcessName);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}

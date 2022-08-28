using System;


namespace GameFramework
{
    /// <summary>
    /// 框架异常
    /// </summary>
    public sealed class GameFrameworkException : Exception
    {
        private GameFrameworkException(string message) : base(message) { }

        /// <summary>
        /// 创建一个异常对象
        /// </summary>
        /// <param name="message"></param>
        /// <returns>异常对象</returns>
        public static GameFrameworkException Generate(object message)
        {
            return new GameFrameworkException(message.ToString());
        }

        /// <summary>
        /// 创建一个异常对象
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="messages">参数</param>
        /// <returns>异常对象</returns>
        public static GameFrameworkException GenerateFormat(string format, params object[] messages)
        {
            return Generate(string.Format(format, messages));
        }

        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="state">状态</param>
        public static void Assert(bool state)
        {
            Assert(state, "");
        }

        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="message">异常消息</param>
        public static void Assert(bool state, object message)
        {
            if (state)
            {
                return;
            }
            throw Generate(message);
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="target">目标对象</param>
        public static void IsNull(object target)
        {
            Assert(target != null, new NullReferenceException());
        }
    }
}
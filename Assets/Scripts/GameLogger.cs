using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// 全局日志管理类，提供可控的、带格式的日志输出。
/// </summary>
public static class GameLogger
{
    // 全局日志开关
    public static bool isLogEnabled = true;

    // 日志颜色配置
    private const string ColorInfo = "#FFFFFF";    // 白色
    private const string ColorSuccess = "#00FF00"; // 绿色
    private const string ColorWarning = "#FFFF00"; // 黄色
    private const string ColorError = "#FF0000";   // 红色
    private const string ColorTag = "#00FFFF";     // 青色

    /// <summary>
    /// 普通信息日志
    /// </summary>
    public static void Log(object message, string tag = "GAME")
    {
        if (!isLogEnabled) return;
        Debug.Log(FormatMessage(message, tag, ColorInfo));
    }

    /// <summary>
    /// 格式化信息日志
    /// </summary>
    public static void LogFormat(string format, params object[] args)
    {
        if (!isLogEnabled) return;
        Debug.Log(string.Format(format, args));
    }

    /// <summary>
    /// 成功信息日志 (绿色)
    /// </summary>
    public static void LogSuccess(object message, string tag = "SUCCESS")
    {
        if (!isLogEnabled) return;
        Debug.Log(FormatMessage(message, tag, ColorSuccess));
    }

    /// <summary>
    /// 警告日志 (黄色)
    /// </summary>
    public static void LogWarning(object message, string tag = "WARNING")
    {
        if (!isLogEnabled) return;
        Debug.LogWarning(FormatMessage(message, tag, ColorWarning));
    }

    /// <summary>
    /// 错误日志 (红色)
    /// </summary>
    public static void LogError(object message, string tag = "ERROR")
    {
        // 错误日志通常需要输出，即便普通日志关闭
        Debug.LogError(FormatMessage(message, tag, ColorError));
    }

    /// <summary>
    /// 异常日志
    /// </summary>
    public static void LogException(System.Exception exception)
    {
        Debug.LogException(exception);
    }

    /// <summary>
    /// 打印变量状态
    /// </summary>
    public static void LogVar(string varName, object varValue, string tag = "VAR")
    {
        if (!isLogEnabled) return;
        Log(string.Format("{0} = {1}", varName, varValue), tag);
    }

    /// <summary>
    /// 打印带帧号的日志
    /// </summary>
    public static void LogFrame(object message, string tag = "FRAME")
    {
        if (!isLogEnabled) return;
        Log(string.Format("[Frame:{0}] {1}", Time.frameCount, message), tag);
    }

    /// <summary>
    /// 格式化消息内容，包含标签和颜色
    /// </summary>
    private static string FormatMessage(object message, string tag, string color)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
        return string.Format("[{0}] <color={1}><b>[{2}]</b></color> <color={3}>{4}</color>", 
            timestamp, ColorTag, tag, color, message);
    }

    /// <summary>
    /// 在编辑器中显示当前日志开关状态
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public static void SetEnabled(bool enabled)
    {
        isLogEnabled = enabled;
        Log(enabled ? "日志已开启" : "日志已关闭", "SYSTEM");
    }
}

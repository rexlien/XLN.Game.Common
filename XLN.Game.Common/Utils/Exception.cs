using System;
namespace XLN.Game.Common.Utils
{
    public static class Exception
    {

        public static void LogAggregateException<T>(AggregateException ex)
        {
            foreach(var exception in ex.Flatten().InnerExceptions)
            {
                if (exception is T)
                {
                    LogService.Logger.Log(LogService.LogType.LT_DEBUG, exception.Message + ":" + exception.StackTrace);
                }
            }
              
           
        }
    }
}

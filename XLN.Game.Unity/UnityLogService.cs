using UnityEngine;
using System.Collections;
using Game.Common;
using System;
using System.Runtime.InteropServices;

[GuidAttribute("4716AFDE-D288-4e1d-87CB-971E9EA54825")]
public class UnityLogService : LogService {


    public override void Log(LogType type, string log)
    {
        switch (type)
        {
            case LogType.LT_DEBUG:
                {
                    Debug.Log(log);
                    break;
                }
            case LogType.LT_ERROR:
                {
                    Debug.LogError(log);
                    break;
                }
            case LogType.LT_WARNING:
                {
                    Debug.LogWarning(log);
                    break;
                }
        }
    }
}

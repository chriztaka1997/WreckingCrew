using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Analytics;

public class AnalyticsManager
{
    public static void TestAnalytics()
    {
        AnalyticsEvent.SocialShare("Real Share", "Totally Real Social Network");
    }
}

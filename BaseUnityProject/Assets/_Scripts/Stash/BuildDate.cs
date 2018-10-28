using System.Reflection;
using UnityEngine;
using System.Collections;

[assembly: AssemblyVersion("1.0.*")] // using * automatically sets version.Build to number of days after 1/1/2000.  More here: http://luminaryapps.com/blog/showing-the-build-date-in-a-unity-app/
public static class BuildDate {
    
    /// <summary>
    /// Gets the date this game was built.
    /// </summary>
    public static System.DateTime date {
        get {
            System.DateTime startDate = new System.DateTime(2000, 1, 1, 0, 0, 0);
            System.TimeSpan span = new System.TimeSpan(version.Build, 0, 0, version.Revision * 2);
            System.DateTime buildDate = startDate.Add(span);
            return buildDate;
        }
        
    }

    public static System.Version version {
        get {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }

}

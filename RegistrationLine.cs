using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Each Line in a Registration
/// </summary>
public class RegistrationLine
{
    private float lineCost;
    private Course lineCourse;
    
    public RegistrationLine(Course lineCourse)
	{
        this.lineCourse = lineCourse;
        this.lineCost = lineCourse.Cost;
	}

    public Course LineCourse
    {
        get { return lineCourse; }
        set {lineCourse = value;}
    }
}

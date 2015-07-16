using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SysFeature
/// </summary>
public class SysFeature
{
	private string featureID = "";

    public SysFeature(string featureID)
	{
        this.featureID = featureID;
	}

    public string FeatureID
    {
        get { return featureID; }
        set { featureID = value; }
    }
}

using System;
using System.Collections.Generic;

[Serializable] 
public class POIData
{
    public int id;
    public string name;
    public float x;
    public float y;
    public float z;
    public string tag;
    public string last_updated;
    public string event_type;
}

[Serializable] 
public class POIDataList
{
    public List<POIData> pois;
}

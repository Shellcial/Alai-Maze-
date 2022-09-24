using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class stores plot data
public class PlotStatistics : MonoBehaviour
{
    public Dictionary<int, MultiPlotInOneLevel> plotInAllLevel;

    public static PlotStatistics instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        plotInAllLevel = new Dictionary<int, MultiPlotInOneLevel>();
    }

    //game object is played, isPlayed set to true
    public void UpdatePlayedPlot(int levelIndex, GameObject gameObject)
    {
        //Debug.Log("Plot Level: " + levelIndex);
        plotInAllLevel[levelIndex].multiPlot.Find(x => x.plotObjectName == gameObject.name).isPlayed = true;
        //Debug.Log("Data is updated");
    }

}

[System.Serializable]
public class MultiPlotInOneLevel
{
    public List<SinglePlot> multiPlot;
    public MultiPlotInOneLevel()
    {
        multiPlot = new List<SinglePlot>();
    }
}

[System.Serializable]
public class SinglePlot
{
    public string plotObjectName;
    public bool isPlayed;
    public SinglePlot(string _plotObjectName,  bool _isPlayed)
    {
        plotObjectName = _plotObjectName;
        isPlayed = _isPlayed;
    }
}
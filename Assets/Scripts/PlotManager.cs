using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class updates current level plot data base on PlotManager
public class PlotManager : MonoBehaviour
{
    private GameObject parentPlotObject;
    private string plotTrigger = "Plot_Trigger";

    //public PlotData plotData;
    private int currentLevelIndex;

    public static PlotManager instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        currentLevelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;

        Dictionary<int, MultiPlotInOneLevel> _plotData = PlotStatistics.instance.plotInAllLevel;

        if (_plotData.ContainsKey(currentLevelIndex))
        {
            //current level has data
            int activeNumber = 0;
            foreach (SinglePlot singlePlot in _plotData[currentLevelIndex].multiPlot)
            {
                if (singlePlot.isPlayed == false)
                {
                    activeNumber += 1;
                    //store active plot for current level
                }
            }
            parentPlotObject = GameObject.Find(plotTrigger);

            if (parentPlotObject.transform.childCount > activeNumber)
            {
                //childCount is greater than active number
                //some objects have been played according to data
                //it should be updated to de disactive
                parentPlotObject = GameObject.Find(plotTrigger);
                List<SinglePlot> playedSinglePlot = _plotData[currentLevelIndex].multiPlot.FindAll(x => x.isPlayed == true);
                for (int i = 0; i < playedSinglePlot.Count; i++)
                {
                    Destroy(parentPlotObject.transform.Find(playedSinglePlot[i].plotObjectName).gameObject);
                    //parentPlotObject.transform.Find(playedSinglePlot[i].plotObjectName).gameObject.SetActive(false);
                }
            }
            else
            {
                //data and current level has same number of active game object
                //no data needs to be updated
            }
        }
        else
        {
            //first entry of current level or no data in current level
            if (GameObject.Find(plotTrigger) != null)
            {
                //level contains plot trigger parent
                MultiPlotInOneLevel newMultiPlotInOneLevel = new MultiPlotInOneLevel();
                _plotData.Add(currentLevelIndex, newMultiPlotInOneLevel);

                parentPlotObject = GameObject.Find(plotTrigger);
                if (parentPlotObject.transform.childCount != 0)
                {
                    //there is plot trigger child
                    for (int i = 0; i < parentPlotObject.transform.childCount; i++)
                    {
                        //add each child data into multi plot in one level
                        SinglePlot newSingPlot = new SinglePlot(parentPlotObject.transform.GetChild(i).gameObject.name, false);
                        _plotData[currentLevelIndex].multiPlot.Add(newSingPlot);
                        //Debug.Log(newSingPlot.plotObjectName + ": " + newSingPlot.isPlayed);
                    }
                }
                //don't do anything since there is no child plot object
            }
            //don't do anything since there is no plot trigger
        }
    }
}

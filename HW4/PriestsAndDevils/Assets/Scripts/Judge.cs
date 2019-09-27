using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;

public enum JudgeEventType : int { lose, win, unfinish }
public interface IJudgeCallback
{
    void JudgeEvent(JudgeEventType events);
}

public class Judge : MonoBehaviour
{
    landModel startLand;
    landModel endLand;
    boatModel boat;
    public IJudgeCallback callback;
    public Controller sceneController;

	void Start ()
	{
		sceneController = (Controller)SSDirector.getInstance().CurrentSceneController;
		startLand = sceneController.startLand;
		endLand = sceneController.endLand;
		boat = sceneController.boat;
		callback = sceneController;
        sceneController.judge = this;
	}

    protected void Update ()
    {
        int[] startLandCount = startLand.getRoleCount ();
		int[] endLandCount = endLand.getRoleCount ();
		int[] boatCount = boat.getRoleCount ();

		int startPriestsCount = startLandCount[0];	//including roles on boat if the boat is at the same side
		int startDevilsCount = startLandCount[1];

		int endPriestsCount = endLandCount[0];
		int endDevilsCount = endLandCount[1];
		
		int boatPriestsCount = boatCount[0];
		int boatDevilsCount = boatCount[1];

		if (boat.getBoatFlag () == 1) {
			startPriestsCount += boatPriestsCount;
			startDevilsCount += boatDevilsCount;
		}
		else if (boat.getBoatFlag () == -1) {
			endPriestsCount += boatPriestsCount;
			endDevilsCount += boatDevilsCount;
		}

		if ((startPriestsCount > 0 && startDevilsCount > startPriestsCount) || (endPriestsCount > 0 && endDevilsCount > endPriestsCount)){	//lose
			callback.JudgeEvent (JudgeEventType.lose);
		}
		else if (startDevilsCount + startPriestsCount == 0 && endPriestsCount + endDevilsCount == 6 && boatPriestsCount == 0 && boatDevilsCount == 0) {	//win
			callback.JudgeEvent (JudgeEventType.win);
		}
		else callback.JudgeEvent (JudgeEventType.unfinish);	//continue playing
    }
}
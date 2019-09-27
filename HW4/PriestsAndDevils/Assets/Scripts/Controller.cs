﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;

public class Controller : MonoBehaviour, ISceneController, IUserAction, IJudgeCallback {
	public landModel startLand;
	public landModel endLand;
	public boatModel boat;
	private roleModel[] roles;

	private JudgeEventType judgement = JudgeEventType.unfinish;

	public Judge judge;
	UserGUI userGui;

	public MySceneActionManager actionManager;
	// Use this for initialization
	void Start () {
		SSDirector director = SSDirector.getInstance ();
		director.CurrentSceneController = this;
		userGui = gameObject.AddComponent<UserGUI> () as UserGUI;
		actionManager = gameObject.AddComponent<MySceneActionManager>() as MySceneActionManager;
		judge = gameObject.AddComponent<Judge>() as Judge;
		loadResources ();
	}

	void Update () {
		updateBoatClickReaction ();
		updateRolesClickReaction ();
		if(userGui.guiFlag == guiFlagType.restart) {
			restart ();
			userGui.guiFlag = guiFlagType.unfinish;
		}
	}

	public void JudgeEvent (JudgeEventType type) {
		judgement = type;
		if (type == JudgeEventType.unfinish) {	//continue
			userGui.guiFlag = guiFlagType.unfinish;
		}
		else if (type == JudgeEventType.win) {	//win
			userGui.guiFlag = guiFlagType.win;
		}
		else if (type == JudgeEventType.lose) {	//lose
			userGui.guiFlag = guiFlagType.lose;
		}
	}

	void updateBoatClickReaction () {
		if (judgement != JudgeEventType.unfinish) {	//win or lose
			boat.shutDownClickReaction ();
			return;
		}
		for (int i = 0; i < roles.Length; ++i) {
			if (roles[i].isMoving()) {
				boat.shutDownClickReaction ();
				return;
			}
		}
		boat.turnOnClickReaction ();
	}

	void updateRolesClickReaction () {
		if (boat.isMoving () || judgement != JudgeEventType.unfinish) {
			for (int i = 0; i < roles.Length; ++i) {
				roles[i].shutDownClickReaction();
			}
		}
		else {
			for (int i = 0; i < roles.Length; ++i) {
				roles[i].turnOnClickReaction ();
			}
		}
	}

	public void loadResources() {
		GameObject river = GameObject.Instantiate(Resources.Load("Prefabs/river"), Vector3.zero, Quaternion.identity) as GameObject;
		river.name = "river";
		startLand = new landModel(1);
		endLand = new landModel(-1);
		boat = new boatModel();
		roles = new roleModel[6];

		for (int i = 0; i < 3; ++i) {
			roleModel role = new roleModel(0);	//priest
			role.setName ("priest" + i);
			role.getOnLand (startLand);
			role.lastPosition = role.getPosition ();
			roles[i] = role;
		}

		for (int i = 0; i < 3; ++i) {
			roleModel role = new roleModel(1);	//devil
			role.setName ("devil" + i);
			role.getOnLand (startLand);
			role.lastPosition = role.getPosition ();
			roles[i + 3] = role;
		}
	}

	//change in v2
	public void moveBoat() {
		if (boat.isEmpty ())
			return;
		//action
		Vector3 target = boat.getBoatFlag () == 1 ? boat.getEndPos () : boat.getStartPos ();
		actionManager.moveBoat (boat.getBoat (), target, 100 * Time.deltaTime );
		boat.setBoatFlag (- boat.getBoatFlag ());
	}

	//change in v2
	public void moveRole(roleModel role) {
		if (role.isOnBoat ()) {	// from boat to land
			landModel land;
			if (boat.getBoatFlag () == 1) 
				land = startLand;
			else land = endLand;
			boat.deleteRole (role.getName ());
			//action
			Vector3 startPos = role.getPosition ();
			Vector3 endPos = land.getEmptyPosition ();
			Vector3 middlePos = new Vector3 (startPos.x, endPos.y, startPos.z);
			actionManager.moveRole (role.getRole (), middlePos, endPos, 100 * Time.deltaTime );
			land.addRole (role);
			role.getOnLand (land);
		}
		else if (!role.isOnBoat ()){					//from land to boat
			landModel land = role.getLand ();
			if (boat.getEmptyPosIndex () == -1 || boat.getBoatFlag () != land.getLandFlag ()) {
				return;
			}
			land.deleteRole (role.getName ());
			//action	
			Vector3 startPos = role.getPosition ();
			Vector3 endPos = boat.getEmptyPosition ();
			Vector3 middlePos = new Vector3 (endPos.x, startPos.y, startPos.z);
			actionManager.moveRole (role.getRole (), middlePos, endPos, 100 * Time.deltaTime );
			boat.addRole (role);
			role.getOnBoat (boat);
		}
	}

	public void restart() {
		Debug.Log ("restart");
		actionManager.Reset ();
		startLand.reset ();
		endLand.reset ();
		boat.reset ();
		for (int i = 0; i < roles.Length; ++i) {
			roles[i].reset ();
		}
	}

	// public int check() {
	// 	int[] startLandCount = startLand.getRoleCount ();
	// 	int[] endLandCount = endLand.getRoleCount ();
	// 	int[] boatCount = boat.getRoleCount ();

	// 	int startPriestsCount = startLandCount[0];	//including roles on boat if the boat is at the same side
	// 	int startDevilsCount = startLandCount[1];

	// 	int endPriestsCount = endLandCount[0];
	// 	int endDevilsCount = endLandCount[1];
		
	// 	int boatPriestsCount = boatCount[0];
	// 	int boatDevilsCount = boatCount[1];

	// 	if (boat.getBoatFlag () == 1) {
	// 		startPriestsCount += boatPriestsCount;
	// 		startDevilsCount += boatDevilsCount;
	// 	}
	// 	else if (boat.getBoatFlag () == -1) {
	// 		endPriestsCount += boatPriestsCount;
	// 		endDevilsCount += boatDevilsCount;
	// 	}

	// 	if ((startPriestsCount > 0 && startDevilsCount > startPriestsCount) || (endPriestsCount > 0 && endDevilsCount > endPriestsCount)){	//lose
	// 		return -1;
	// 	}
	// 	else if (startDevilsCount + startPriestsCount == 0 && endPriestsCount + endDevilsCount == 6 && boatPriestsCount == 0 && boatDevilsCount == 0) {	//win
	// 		return 1;
	// 	}
	// 	return 0;	//continue playing
	// }
}
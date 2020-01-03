using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;

public class Controller : MonoBehaviour, ISceneController, IUserAction {
	public landModel startLand;
	public landModel endLand;
	public boatModel boat;
	private roleModel[] roles;

	UserGUI userGui;

	// Use this for initialization
	void Start () {
		SSDirector director = SSDirector.getInstance ();
		director.CurrentSceneController = this;
		userGui = gameObject.AddComponent<UserGUI> () as UserGUI;
		loadResources ();
	}

	void Update () {
		updateBoatClickReaction ();
		updateRolesClickReaction ();
		if(userGui.guiFlag == 0) {
			Debug.Log ("LateUpdate(): userGUi.guiFlag = " + userGui.guiFlag);
			restart ();
			userGui.guiFlag = 1;
		}
		if (check() == 0) {	//continue
			userGui.guiFlag = 1;
		}
		else if (check() == 1) {	//win
			userGui.guiFlag = 2;
		}
		else if (check() == -1) {	//lose
			userGui.guiFlag = 3;
		}
	}

	void updateBoatClickReaction () {
		if (check() != 0) {
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
		if (boat.isMoving () || check () != 0) {
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

	public void moveBoat() {
		if (boat.isEmpty ())	//
			return;
		boat.boatMove ();
	}

	public void moveRole(roleModel role) {
		if (role.isOnBoat ()) {	// from boat to land
			landModel land;
			if (boat.getBoatFlag () == 1) 
				land = startLand;
			else land = endLand;
			role.getOnLand (land);
		}
		else if (!role.isOnBoat ()){					//from land to boat
			landModel land = role.getLand ();
			if (boat.getEmptyPosIndex () == -1 || boat.getBoatFlag () != land.getLandFlag ()) {
				return;
			}
			role.getOnBoat (boat);
		}
	}

	public void restart() {
		Debug.Log ("restart");
		startLand.reset ();
		endLand.reset ();
		boat.reset ();
		for (int i = 0; i < roles.Length; ++i) {
			roles[i].reset ();
		}
	}

	public int check() {
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
			return -1;
		}
		else if (startDevilsCount + startPriestsCount == 0 && endPriestsCount + endDevilsCount == 6 && boatPriestsCount == 0 && boatDevilsCount == 0) {	//win
			return 1;
		}
		return 0;	//continue playing
	}
}

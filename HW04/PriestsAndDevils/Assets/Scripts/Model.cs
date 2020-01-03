using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myGame{

	public interface ISceneController {
		void loadResources();
	}

	public interface IUserAction {
		void moveBoat();
		void moveRole(roleModel role);
		void restart();
	}

	public class SSDirector : System.Object {
		private static SSDirector _instance;

		public ISceneController CurrentSceneController { get; set; }

		public static SSDirector getInstance() {
			if (_instance == null) {
				_instance = new SSDirector ();
			}
			return _instance;
		}
	}

	public class roleModel {
		GameObject role;
		int roleFlag;	//0 for priest, 1 for devil

		bool onBoat = false;

		Click click;
		//Move move;

		public Vector3 lastPosition;

		landModel land = (SSDirector.getInstance().CurrentSceneController as Controller).startLand;
		boatModel boat = (SSDirector.getInstance().CurrentSceneController as Controller).boat;

		public roleModel (int roleFlag) {
			this.roleFlag = roleFlag;
			if (roleFlag == 0) {
				this.roleFlag = roleFlag;
				role = Object.Instantiate (Resources.Load ("Prefabs/priest"), Vector3.zero, Quaternion.identity) as GameObject;
			} else if (roleFlag == 1) {
				this.roleFlag = roleFlag;
				role = Object.Instantiate (Resources.Load ("Prefabs/devil"), Vector3.zero, Quaternion.identity) as GameObject;
			}
			//move = role.AddComponent (typeof(Move)) as Move;
			click = role.AddComponent (typeof(Click)) as Click;
			click.setRole (this);
			reset ();
		}

		public GameObject getRole () {
			return role;
		}
		public int getRoleFlag () {
			return roleFlag;
		}

		public bool isOnBoat () {
			return onBoat;
		}

		public string getName () {
			return role.name;
		}

		public void setName (string name) {
			role.name = name;
		}

		public void setPosition (Vector3 position) {
			role.transform.position = position;
		}

		public Vector3 getPosition () {
			return role.transform.position;
		}

		// public bool moveTo (Vector3 position) {
		// 	move.moveTo (position);
		// 	return true;
		// }

		public landModel getLand () {
			return land;
		}

		public boatModel getBoat () {
			return boat;
		}

		public void getOnLand (landModel land) {
			role.transform.parent = null;
			this.land = land;
			onBoat = false;
		}

		public void getOnBoat (boatModel boat) {
			this.land = null;
			role.transform.parent = boat.getBoat ().transform;
			onBoat = true;
		}

		public bool isMoving () {
			if (lastPosition != getPosition ()) {
				lastPosition = getPosition ();
				return true;
			}
			return false;
		}

		public void shutDownClickReaction () {
			click.setRole (null);
		}

		public void turnOnClickReaction () {
			click.setRole (this);
		}

		public void reset () {
			land = (SSDirector.getInstance().CurrentSceneController as Controller).startLand;
			boat.deleteRole (this.getName ());
			role.transform.position = land.getEmptyPosition ();
			land.addRole (this);
			getOnLand (land);
		}
	}

	public class boatModel {
		GameObject boat;

		Vector3[] startLandPassengersPos;
		Vector3[] endLandPassengersPos;

		const float passengerYPos = 0.48F;
		const float passengerZPos = 0F;
		const float passengerXInterval = 0.4F;

		roleModel[] roles = new roleModel[2];

		Vector3 lastPosition;

		//Move move;
		public Click click;

		Vector3 startPos = new Vector3 (0.9F, 0.11F, 0F);
		Vector3 endPos = new Vector3 (-0.9F, 0.11F, 0F);

		int boatFlag = 1;	//1 for start land side, -1 for end land side, similar to landFlag

		public boatModel () {
			boat = GameObject.Instantiate (Resources.Load ("Prefabs/boat"), startPos, Quaternion.identity) as GameObject;
			this.lastPosition = startPos;
			boat.name = "boat";
			//move = boat.AddComponent (typeof(Move)) as Move;
			click = boat.AddComponent (typeof(Click)) as Click;
			click.setBoat (this);
			startLandPassengersPos = new Vector3[] { new Vector3 (1.1F, passengerYPos, passengerZPos), new Vector3 (1.1F - passengerXInterval, passengerYPos, passengerZPos) };
			endLandPassengersPos = new Vector3[] { new Vector3 (- 1.1F + passengerXInterval, passengerYPos, passengerZPos), new Vector3 (-1.1F, passengerYPos, passengerZPos) };
		}

		public bool isEmpty () {
			for (int i = 0; i < roles.Length; ++i) {
				if (roles [i] != null) {
					return false;
				}
			}
			return true;
		}

		public bool isMoving () {
			if (lastPosition != boat.transform.position) {
				lastPosition = boat.transform.position;
				return true;
			}
			return false;
		}

		public Vector3 getStartPos () {
			return startPos;
		}

		public Vector3 getEndPos () {
			return endPos;
		}

		public void shutDownClickReaction () {
			click.setBoat (null);
		}

		public void turnOnClickReaction () {
			click.setBoat (this);
		}

		// public void boatMove () {
		// 	if (boatFlag == 1) {
		// 		move.moveTo (endPos);
		// 	} else if (boatFlag == -1) {
		// 		move.moveTo (startPos);
		// 	}
		// 	boatFlag = -boatFlag;
		// }
		public void setBoatFlag (int flag) {
			this.boatFlag = flag;
		}

		public int getBoatFlag () {
			return boatFlag;
		}

		public roleModel[] getRoles () {
			return roles;
		}

		public int getEmptyPosIndex () {
			for (int i = 0; i < roles.Length; ++i) {
				if (roles [i] == null) {
					return i;
				}
			}
			return -1;
		}

		public Vector3 getEmptyPosition () {
			Vector3 pos = new Vector3 ();
			if (boatFlag == 1) {
				pos = startLandPassengersPos [getEmptyPosIndex()];
			} else if (boatFlag == -1) {
				pos = endLandPassengersPos [getEmptyPosIndex()];
			}
			return pos;
		}

		public void addRole (roleModel role) {
			roles [getEmptyPosIndex ()] = role;
		}

		public roleModel deleteRole (string name) {
			for (int i = 0; i < roles.Length; ++i) {
				if (roles [i] != null && roles [i].getName () == name) {
					roleModel role = roles [i];
					roles [i] = null;
					return role;
				}
			}
			return null;
		}

		public int[] getRoleCount () {
			int[] count = { 0, 0 };
			for (int i = 0; i < roles.Length; ++i) {
				if (roles [i] != null) {
					count [roles [i].getRoleFlag ()]++;
				}
			}
			return count;
		}

		public GameObject getBoat () {
			return this.boat;
		}

		public void reset () {
			if (boatFlag == -1) {
				boat.transform.position = startPos;
				boatFlag = 1;
				//boatMove ();
			}
			roles = new roleModel[2];
		}
	}

	public class landModel {
		GameObject land;
		Vector3[] positions;
		int landFlag;	//1 for startLand, -1 for endLand
		roleModel[] roles = new roleModel [6];

		private const float roleYPosition = 0.85F;
		private const float roleZPosition = 0F;
		private const float roleXPosInterval = 0.3F;
		private const float firstRoleXPosition = 3.5F;

		private const float landXPositionABS = 2.43F;
		private const float landYPosition = 0.1F;
		private const float landZPosition = 0F;

		public landModel (int flag) {
			positions = new Vector3[] {
				new Vector3 (firstRoleXPosition - roleXPosInterval, roleYPosition, roleZPosition),
				new Vector3 (firstRoleXPosition - 2 * roleXPosInterval, roleYPosition, roleZPosition),
				new Vector3 (firstRoleXPosition - 3 * roleXPosInterval, roleYPosition, roleZPosition),
				new Vector3 (firstRoleXPosition - 4 * roleXPosInterval, roleYPosition, roleZPosition),
				new Vector3 (firstRoleXPosition - 5 * roleXPosInterval, roleYPosition, roleZPosition),
				new Vector3 (firstRoleXPosition - 6 * roleXPosInterval, roleYPosition, roleZPosition)
			};
			land = Object.Instantiate (Resources.Load ("Prefabs/land"), new Vector3 (flag * landXPositionABS, landYPosition, landZPosition), Quaternion.identity) as GameObject;
			landFlag = flag;
			if (landFlag == 1) {
				this.land.name = "startLand";
			}
			else if (landFlag == -1) {
				this.land.name = "endLand";
			}
		}
			
		public int getEmptyPosIndex () {
			for (int i = 0; i < roles.Length; ++i) {
				if (roles [i] == null)
					return i;
			}
			return -1;
		}

		public Vector3 getEmptyPosition () {
			Vector3 pos = positions [getEmptyPosIndex ()];
			pos.x = landFlag * pos.x;
			return pos;
		}

		public void addRole (roleModel role) {
			roles [getEmptyPosIndex ()] = role;
		}

		public roleModel deleteRole(string roleName) {
			for (int i = 0; i < roles.Length; ++i) {
				if (roles [i] != null && roles [i].getName () == roleName) {
					roleModel role = roles [i];
					roles [i] = null;
					return role;
				}
			}
			return null;
		}

		public int[] getRoleCount ()
		{
			int[] count = {0, 0};
			for (int i = 0; i < roles.Length; ++i) {
				if (roles [i] != null) {
					count [roles [i].getRoleFlag ()]++;
				}
			}
			return count;
		}

		public int getLandFlag () {
			return landFlag;
		}

		public GameObject getLand () {
			return this.land;
		}

		public void reset (){
			roles = new roleModel [6];
		}
	}

	public class Click : MonoBehaviour {
		IUserAction action;
		public roleModel role = null;
		public boatModel boat = null;

		public void setRole(roleModel role) {
			this.role = role;
		}

		public void setBoat(boatModel boat) {
			this.boat = boat;
		}

		void Start() {
			action = SSDirector.getInstance ().CurrentSceneController as IUserAction;
		}

		void OnMouseDown () {
			if (boat == null && role == null)
				return;
			if (role != null) {
				action.moveRole (role);
			}
			else if (boat != null) {
				action.moveBoat ();
			}
		}
	}

	// public class Move : MonoBehaviour {
	// 	float speed = 5;
	// 	int moveFlag = 0;	//0 for no action, 1 for move vertically, 2 for move horizontally
	// 	Vector3 endPosition;
	// 	Vector3 tempPosition;

	// 	void Update () {
	// 		if (moveFlag == 1) {
	// 			transform.position = Vector3
	// 				.MoveTowards (transform.position, tempPosition, speed * Time.deltaTime);
	// 			if (transform.position == tempPosition) {
	// 				moveFlag = 2;
	// 			}
	// 		}
	// 		if (moveFlag == 2) {
	// 			transform.position = Vector3.MoveTowards (transform.position, endPosition, speed * Time.deltaTime);
	// 			if (transform.position == endPosition)
	// 				moveFlag = 0;
	// 		}
	// 	}

	// 	public void moveTo (Vector3 position) {
	// 		endPosition = position;
	// 		if (position.y == transform.position.y) {	//boat moves
	// 			moveFlag = 2;
	// 		} else if (position.y < transform.position.y) {	//role move from land to boat
	// 			moveFlag = 1;
	// 			tempPosition = new Vector3 (position.x, transform.position.y, position.x);
	// 		} else if (position.y > transform.position.y) {	//role move from boat to land
	// 			moveFlag = 1;
	// 			tempPosition = new Vector3 (transform.position.x, position.y, transform.position.z);
	// 		}
	// 	}
	// }
}
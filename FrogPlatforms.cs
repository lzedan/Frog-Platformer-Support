using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MoreMountains.InfiniteRunnerEngine;

namespace FrogPlatforms {
	/*-------       CLASS: DecoVariation       -------
	Description: This is an individual variation of decor for a single platform.

	Member variables:
		- isEnabled
		- spawnOdds
		- decorSet
	
	Member Methods:
		- DecoVariation
		- selectDecor
	  -------------------------*/
	  [System.Serializable]
	public class DecoVariation {
		public bool isEnabled = true; //Set whether or not this variation can be spawned in the game
		public int spawnOdds = 1; //Set how great of a chance this variation can occur specific to a platform
		public GameObject decorSet; //The game object associated with the class

		//Create a new DecoVariation object by assigning tiles
		public DecoVariation() {
		}

		//SELECT DECOR
		//This is the critical function. Selecting the decor will return the gameObject associated with
		//the current DecoVariation object so that it can be turned on for an instanced platform.
		public void selectDecor() {
			try {
				this.decorSet.SetActive(true);
			}
			catch(System.InvalidOperationException e){
				throw new System.InvalidOperationException("GameObject must be enabled");
			}
		}
	}


	/*-------       CLASS: DecoOptions       -------
	Description: A composite array of DecoVariation objects so that they don't have to be defined
	per-platform per-instance.

	Member Variables:
		- availableOptions - The array contianing the tilemap GameObjects containing platform decorations

	Member Methods:
		- DecorOptions
		- DecorOptions()
		- reportLength
		- selectVariation
		- resetVariation
	  -------------------------*/
	public class DecorOptions {
		//Internal array of DecoVariation objects that are possible for a type of game platform
		public DecoVariation[] availableOptions;

		//A constructor that takes zero arguments. I just need to copy between component declaration and the start function, don't @ me
		public DecorOptions() {
		}

		//Just in case the dev hasn't de-activated all the decor gameObjects, do it at initialisation with an input public set.
		public DecorOptions(DecoVariation[] inputDecor) {
			//Copy the input array into the internal array
			this.availableOptions = inputDecor;

			//Just in case, reset all DecoVariation GameObjects to inactive
			for(int i = 0; i < this.availableOptions.Length; i++) {
				this.availableOptions[i].decorSet.SetActive(false);
			}
		}

		//This function will check the length of the internal array.
		public int reportLength() {
			int output = 0;
			//For some reason at start, the platforms don't have an availableOptions array yet.
			//This if statement is here so that the programme doesn't freak out.
			if(this.availableOptions == null) {
				output = 0;
			}
			//We're fine now inside this else statement.
			else {
				output = this.availableOptions.Length;
			}
			return output;
		}

		public void selectVariation() {
			//Handler for random number generation
			System.Random rng = new System.Random();

			//Builds an int array for consecutive values of maxChances to be associated with an index of a matching decor set
			int[] chanceList = new int[this.availableOptions.Length];

			int maxChances = 0;

			//This first for loop is for determining the probability of certain sets of decorations appearing on an instanced platform.
			//Add to maxChances to find the total probability of all sets
			//And add the current maxChances to the chanceArray that associates the range of chance with a specific set of decorations
			for(int i = 0; i < this.availableOptions.Length; i++) {
				maxChances += this.availableOptions[i].spawnOdds;
				chanceList[i] = maxChances;
			}

			//Generate a number that associates with a picked decor set
			int selector = rng.Next(maxChances);

			//THIS IS THE MAIN LOOP
			//Iterate through the chanceList. This is mainly to iterate through the indexes of the associated decoration sets.
			for(int j = 0; j < chanceList.Length; j++) {
				//If our randomly generated number corresponds to a probability number at the current index,
				//Activate the member GameObject with the member function
				if(selector <= chanceList[j]){
					try{
						this.availableOptions[j].selectDecor();
						break; //Break out of the loop - we don't need to continue and risk stacking decoration sets on the platform.
					}
					//If the selector generated number does not match to an active set,
					//Reset all the variables so that we can continue finding a set to assign to the platform.
					catch(System.InvalidOperationException) {
						j = 0;
						selector = rng.Next(maxChances);
						continue;
					}
				}
			}

		}

		//RESET FUNCTION
		//Once a platform goes out of bounds, de-activate all GameObjects associated with DecoVariation objects in the internal array.
		//Only then, re-start the process for picking a new decoration set.
		public void resetVariation() {
			//When a platform gets deactivated at runtime by going out of bounds, turn off all active sets and re-select a Variation
			for(int i = 0; i < this.availableOptions.Length; i++) {
				this.availableOptions[i].decorSet.SetActive(false);
			}

			//Use the member function selectVariation to pick a new decoration set for the platform once it's re-entered into the game.
			this.selectVariation();
		}
	}

	/*-------       CLASS: EntityObject      -------
	Description: This is an individual entity object in-game that is inherently dynamic and affects
	points or player condition. Automatically activated depending on the ID code generated during
	platform instancing.

	Member Variables:
		- entityType
		- pointsToAdd
		- pointsCollection
		- isEnabled
		- entityGO -> The GameObject that will implement the entity class
	
	Member Functions:
		- EntityObject
		- EntityObject()
		- addPoints
		- seedActivation
		- deactivateEntity
	  -------------------------*/
	  [System.Serializable]
	public class EntityObject {
		public System.String entityType = "coin";  //Type of entity, whether this is a coin, enemy, pickup, or obstacle
		public float pointsToAdd = 1; //How many points to add depending on type of interaction behaviour
		public int pointsCollection = 1; //If 0, add to generic points collector. Otherwise, add to coin aggregate
		public bool isEnabled = true; //Is this entity enabled for generation?
		public GameObject entityGO; //The GameObject associated with this individual object

		//Creation function for a new EntityObject
		public EntityObject() {
		}

		//Second Creation function that actively takes inputs so we're no longer on planet empty
		public EntityObject(System.String eType, float assoPoints, int wherego, GameObject currObject) {
			this.entityType = eType;
			this.pointsToAdd = assoPoints;
			this.pointsCollection = wherego;
			this.entityGO = currObject;
		}

		//Depending on the value of pointsCollection, this function adds points to the proper collection
		//As determined by the interaction behaviour defined in a separate script.
		public void addPoints() {
			GameObject gameManager = GameObject.Find("GameManager");
			if(pointsCollection == 0){
				gameManager.GetComponent<GameManager>().SendMessage("AddPoints", pointsToAdd);
			}
			else if(pointsCollection == 1){
				gameManager.GetComponent<GameManager>().SendMessage("AddCoins", (int)pointsToAdd);
			}
		}

		//EntityScene generates a random number for the platform called an ID
		//This ID can be calculated to activate certain types of entityObjects for the dynamic part of the scene.
		//Each entity type has a unique seeding function.
		public void seedActivation(int generatorID, int idRange) {
			if(this.isEnabled == true) {
				if(entityType == "coin") { //Roughly 42% of all ID's will activate coins on the platform
					if(generatorID > ((idRange/2)+(0.08*idRange))) {
						this.entityGO.SetActive(true);
					}
					else{
						this.entityGO.SetActive(false);
					}
				}
				else if(entityType == "enemy") { //Roughly 29% of all ID's will activate enemies on the platform
					if((generatorID % 3 == 0) && (generatorID % 7 >= 1)) {
						this.entityGO.SetActive(true);
					}
					else{
						this.entityGO.SetActive(false);
					}
				}
				else if(entityType == "pickup") { //Roughly 9% of all ID's will activate pickups on the platform
					if(generatorID % 11 == 0) {
						this.entityGO.SetActive(true);
					}
					else{
						this.entityGO.SetActive(false);
					}
				}
				else if(entityType == "obstacle") { //Roughly 20% of all ID's will generate any obstacles on the platform
					if((generatorID + 3) % 5 == 0) {
						this.entityGO.SetActive(true);
					}
					else{
						this.entityGO.SetActive(false);
					}
				}
			}
		}

		//When the collector EntityScene resets, it uses this de-activate function so it doesn't access member variables
		public void deactivateEntity() {
			this.entityGO.SetActive(false);
		}


	}

	/*-------       CLASS: EntityScene       -------
	Description: A composite array of EntityObject objects that is associated with a tile layout
	platform style.

	Member Variables:
		- availableEntities - an array of EntityObject objects
		- id_range

	Member Methods:
		- EntityScene()
		- EntityScene([])
		- objectActivation
		- resetScene

	  -------------------------*/
	public class EntityScene {
		public EntityObject[] availableEntities;
		public int id_range = 255;
		//public int generated_id = 10;

		public EntityScene() {
			//Nothing lol, just here for a copy argument
		}

		public EntityScene(EntityObject[] inputEntities) {
			this.availableEntities = inputEntities;
		}

		public void objectActivation(int inputRange) {
			//Handler for random number generation
			System.Random rng = new System.Random();

			this.id_range = inputRange;

			int selector = rng.Next(this.id_range);
			for(int v = 0; v < this.availableEntities.Length; v++) {
				this.availableEntities[v].seedActivation(selector, this.id_range);
			}
		}

		//Still need a reset function
		//This gets called when the containing platform is out of bounds of the play area.
		public void resetScene() {
			if(this.availableEntities == null){
				return;
			}
			else{
				int entityListLength = this.availableEntities.Length;
				for(int i = 0; i < entityListLength; i++) {
					availableEntities[i].deactivateEntity();
				}

				this.objectActivation(this.id_range);
			}
		}
	}

}

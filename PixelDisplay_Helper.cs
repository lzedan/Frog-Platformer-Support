using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelDisplay_Helper : MonoBehaviour {

	public int native_Width = 0;
	public int native_Height = 368;

	GameObject debugAgent;

	public float pixelsPerUnit = 16f;
	//public int wiggleRange = 8;

	Vector3 cameraPos = Vector3.zero;

	// Use this for initialization
	void Start () {
		debugAgent = GameObject.Find("DebugPanel UI");
		WindowSetup();
		ApplyZoom();
		AdjustCamera();
		//WiggleValues();
	}

	public void AdjustCamera() {
		Camera.main.transform.position = new Vector3(
			RoundToNearestPixel(cameraPos.x),
			RoundToNearestPixel(cameraPos.y),
			-10f
		);
	}

	public float RoundToNearestPixel(float pos) {
		float screenPixelsPerUnit = Screen.height / (Camera.main.orthographicSize * 2f);
		float pixelValue = Mathf.Round(pos * screenPixelsPerUnit);

		return pixelValue / screenPixelsPerUnit;
	}

	public void ApplyZoom() {
		float smallestDimension = Screen.height < Screen.width ? Screen.height : Screen.width;
		float pixelScale = Mathf.Clamp(Mathf.Round(smallestDimension / native_Height), 1f, 8f);

		Camera.main.orthographicSize = (Screen.height / (pixelsPerUnit * pixelScale)) * 0.5f;
	}

	/*void WiggleValues() {
		//This function is being weird. It's meant to approximate within a range of values, what would scale the best for a proposed native height
		//Right now it somehow decides that the last number is always best. I'll need to find a way to detect which values have the most zeroes 
		//after the decimal place. Or, at least, closest to becoming a zero.

		float closestLow = 1.0f;
		float subNumber = 0.0f;
		float proposedValue = 1.0f;
		int optimalDimension = 0;

		for(int currValue = (native_Height - wiggleRange); currValue <= (native_Height + wiggleRange); currValue++) {
			proposedValue = Screen.height/(float)currValue;
			Debug.Log(proposedValue);
			subNumber = proposedValue - (int)proposedValue;
			if(subNumber <= closestLow){
				closestLow = subNumber;
				optimalDimension = currValue;
			}
		}

		Debug.Log(optimalDimension);
	}*/

	void WindowSetup() {
		//First values
		//First calculate how many native screens we can fit into the full screen based on height.
		//This number will inevitably be a decimal, so we need to compare both the lower value with the decimals truncated
		//and that value + 1
		float compareBase = Screen.height/native_Height;
		float Low_option = (float)(System.Math.Truncate(compareBase)); //The Truncate function just knocks out whatever values follow the decimal point.
		float High_option = (float)(System.Math.Truncate(compareBase) + 1.0f);

		//Setting up values that will be used in our for loop
		int diff1 = 0;
		int diff2 = 0;
		bool legitValues = true; //Assuming that all our calculations will result in usable numbers
		int output = 0;

		//This for loop sort of filters through our options values we calculated earlier. We need to find out which resolution
		//best suits the kind of game we're running, based on the available screen resolution output by the display.
		//We're iterating through a range of (0, 2) because there's literally only two options.
		for(int i = 0; i < 2; i++) {
			//The first option, the lower multiple, Low_option
			if(i == 0) {
				//CHECK: Will this multiple, if applied to our native height, be greater than what our display screen is capable of?
				if((Low_option * native_Height) > Screen.height) {
					legitValues = false;
					continue;
				}
				//Otherwise, calculate what the difference is between this value and our native height
				//We want whatever value has the lowest absolute value
				else{
					diff1 = Mathf.Abs(native_Height - ((int)Low_option*native_Height));
				}
			}

			//The second option, the higher multiple, High_option
			if(i == 1) {
				//CHECK: Will this multiple, if applied to our native height, be greater than what our display screen is capable of?
				if((High_option * native_Height) > Screen.height) {
					legitValues = false;
					continue;
				}
				//Otherwise, calculate what the difference is between this value and our native height
				//We want whatever value has the lowest absolute value
				else{
					diff2 = Mathf.Abs(native_Height - ((int)High_option*native_Height));
				}
			}
		}

		//Okay cool, so we have the absolute value differences for both options.
		//NOW CHECK: Which has the lowest absolute value? The lowest absolute value represents the option closest to our native height.
		//If both values are compliant in that they don't extend beyond our screen's capabilities, set this value as our output.
		if(legitValues) {
			if(diff1 < diff2) {
				output = (int)Low_option*native_Height;
			}
			else if(diff2 > diff1) {
				output = (int)High_option*native_Height;
			}
		}
		//If one of our options is not compliant, find whatever value is greater than zero.
		else{
			if(diff1 > 0) {
				output = (int)Low_option*native_Height;
			}
			else if(diff2 > 0) {
				output = (int)High_option*native_Height;
			}
		}

		//Our last step is to find the width.
		//REMEMBER, the important dimension is height, so the width is whatever complies with our screen's aspect ratio so that the
		//game view fills the whole available space.
		//With that in mind, multiply the output to our aspect ratio to get the width.
		//debugAgent.GetComponent<DebugHandler>().SendMessage("adjustedHeight", output);
		int new_Width = Mathf.RoundToInt((Screen.width*output)/Screen.height);
		//debugAgent.GetComponent<DebugHandler>().SendMessage("adjustedWidth", new_Width);
		//Set the resolution with our new values. We did it!
		Screen.SetResolution(new_Width, output, true);

	}
}

﻿using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour
{
    Unit player;

    // Use this for initialization
    void Start()
    {
        //Finds a gameobject that is tagged with player
        //We now have access to class unit
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();

    }
    public void setPlayer() { player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>(); }
    // Update is called once per frame
    void Update()
    {

        //TODO handle touch screen
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        //If the mouse is hovering over something
        if (Physics.Raycast(mouseRay, out hitInfo))
        {
            GameObject mousedOverObj = hitInfo.collider.transform.parent.gameObject;
            //Debug.Log("Are hovering over" + hitInfo.collider.transform.parent.name);


            if (Input.GetMouseButton(0))
            {
                //some stubbed function for attacking things i guess
            }

            //Move player to selected tile
			if (Input.GetMouseButtonUp(1))
            {
                //if we right click a hex
                if (mousedOverObj.GetComponent<Hex>() != null)
                {
                    Debug.Log("You have right clicked " + mousedOverObj.name);
                    if (player.moveCounter < 5)
                    {
                        player.moveTo(mousedOverObj);
                    }
                }
            }
            //if we right click a unit
            else if (mousedOverObj.GetComponent<Unit>() != null)
            {
                
            }
        }
    }
	public void aquireTarget(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)){
			GameObject mousedOverObj = hit.collider.transform.parent.gameObject;
			Debug.Log("target selected " + hit.collider.transform.parent.name);

		}
	}
}
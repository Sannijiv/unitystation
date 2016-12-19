﻿using PlayGroup;
using SS.NPC;
using UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{

    public Sprite deadSprite;
    public GameObject meatPrefab;
    public int amountSpawn = 1;

    private SpriteRenderer spriteRenderer;
    private RandomMove randomMove;
    private PhysicsMove physicsMove;
    private bool dead = false;
	private bool sliced = false;
    private PhotonView photonView;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        randomMove = GetComponent<RandomMove>();
        physicsMove = GetComponent<PhysicsMove>();
        photonView = GetComponent<PhotonView>();
    }

    void OnMouseDown()
    {
        if (!dead && UIManager.control.hands.CurrentSlot.Item != null)
        {
            if (UIManager.control.hands.CurrentSlot.Item.GetComponent<ItemAttributes>().type == ItemType.Knife)
            {
                if (PhotonNetwork.connectedAndReady)
                {
                    photonView.RPC("Die", PhotonTargets.All, null); //Send death to all clients for pete
                }
                else
                {
                    Die(); //Dev mode
                }
            }
        }
        else if (UIManager.control.hands.CurrentSlot.Item != null && dead && !sliced)
        {    
            if (UIManager.control.hands.CurrentSlot.Item.GetComponent<ItemAttributes>().type == ItemType.Knife)
            {
                
                
                if (PhotonNetwork.connectedAndReady)
                {
                    GameMatrix.control.RemoveItem(photonView.viewID);
					photonView.RPC ("SpawnMeat", PhotonTargets.MasterClient, null);
                }
                else
                {
					SpawnMeat(); //For dev mode
                    Destroy(gameObject); 
                }
				sliced = true;
            }
        }
    }

    [PunRPC]
    void Die()
    {
        dead = true;
        randomMove.enabled = false;
        physicsMove.enabled = false;
        spriteRenderer.sprite = deadSprite;
        SoundManager.control.Play("Bodyfall");
    }

	[PunRPC]
    void SpawnMeat()
    {
        for (int i = 0; i < amountSpawn; i++)
        {
            if (PhotonNetwork.connectedAndReady)
            {
				if(PhotonNetwork.isMasterClient){
					GameMatrix.control.MasterClientCreateItem(meatPrefab.name,transform.position,Quaternion.identity,0,null); //Create scene owned object
				} 
			}else
            { //Dev mode
                var meat = Instantiate(meatPrefab); 
                meat.transform.position = transform.position;
            }
        }
    }
}

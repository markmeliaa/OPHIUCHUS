﻿using UnityEngine;

public class MinimapRoomSpawner : MonoBehaviour 
{
	private RoomTemplates templates;

    private void Awake()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        templates.rooms.Add(gameObject);
    }
}
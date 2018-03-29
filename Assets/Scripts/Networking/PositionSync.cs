using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSync : MonoBehaviour
{
	public float syncTime = 0.05f;
	private float time;
	private EnemyScript es;

	void Awake()
	{
		es = GetComponent<EnemyScript>();
	}

	void Update()
	{
		if(es.id == 0) return;

		time += Time.deltaTime;
		if (time > syncTime)
		{
			time = 0;
			string msg = (int)ProtocolDef.PosSync + "|"
			             + es.id + ";"
			             + transform.position.x + ";"
			             + transform.position.y + ";"
			             + transform.position.z + ";"
			             + transform.rotation.eulerAngles.x + ";"
			             + transform.rotation.eulerAngles.y + ";"
			             + transform.rotation.eulerAngles.z;
			Connection.SendToAll(msg);
		}
	}
}
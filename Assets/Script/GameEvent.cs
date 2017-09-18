using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{

	public TextAsset Script;
	public float Distance = 1;

	GameMain main;

	bool use;

	// Use this for initialization
	void Start()
	{
		use = false;
		main = Component.FindObjectOfType<GameMain>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (use) return;

		Vector3 eyePos = main.Player.EyeMarker.transform.position;
		Vector3 diff = transform.position - eyePos;
		float dist2 = Distance * Distance;
		if (diff.x * diff.x + diff.y * diff.y < dist2)
		{
			main.SetScript(Script.text);
			use = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		use = false;
	}
}

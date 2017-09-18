using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	public Animator PlayerAnimater;
	public GameObject EyeMarker;

	Vector2 velocity;
	bool playable;

	// Use this for initialization
	void Start()
	{
		this.transform.position = new Vector2(0, 0);
		playable = false;
	}

	public void SetPlayable(bool flag)
	{
		playable = flag;
	}

	public void SetAnimation(int id)
	{
		PlayerAnimater.SetInteger("state", id);
	}

	// Update is called once per frame
	void Update()
	{
		float dt = Time.deltaTime;
		Vector2 playerPos = this.transform.position;
		Vector2 markerPos = EyeMarker.transform.position;
		bool touch = Input.GetMouseButton(0);

		if (touch && playable)
		{
			markerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			EyeMarker.transform.position = new Vector3(markerPos.x, markerPos.y, -1);
		}
		if (playable && Vector2.Distance(playerPos, markerPos) > 0.1f)
		{
			Vector2 diffPos = markerPos - playerPos;
			diffPos = diffPos.normalized * 2;
			velocity += (diffPos - velocity) * 0.5f;
		}
		else velocity *= 0.8f;

		Rigidbody2D body = this.GetComponent<Rigidbody2D>();
		body.velocity = velocity;

		if (playable)
		{
			int state = 0;
			if (Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y) > 0.1f) state = 1;
			PlayerAnimater.SetInteger("state", state);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectBoard : MonoBehaviour
{
	public GameObject[] Buttons;
	public Vector2[] targets;

	int state;
	int selectLength;
	int selectId;
	float timer;

	string[] results;

	// Use this for initialization
	void Start()
	{
		for (int l = 0; l < Buttons.Length; l++)
		{
			var id = l;
			Buttons[l].GetComponent<Button>().onClick.AddListener(() => { ClickButton(id); });
			Buttons[l].SetActive(false);
		}
		state = 0;

		//SetSelect(new string[] { "調べてみる", "やめとこう・・・", "食べよう！", "眠い！" });
		//SetSelect(new string[] { "調べてみる:a", "やめとこう・・・:b", "食べよう！:c" });
		//SetSelect(new string[] { "調べてみる", "やめとこう・・・" });
	}

	// Update is called once per frame
	void Update()
	{
		float dt = Time.deltaTime;
		timer += dt;

		if (state == 1)
		{
			for (int l = 0; l < selectLength; l++)
			{
				var trans = Buttons[l].GetComponent<RectTransform>();
				var pos = trans.localPosition;
				if (timer > l * 0.2f) trans.localPosition = new Vector2(pos.x * 0.9f, pos.y);
			}
		}
		else if (state == 2)
		{
			for (int l = 0; l < selectLength; l++)
			{
				var trans = Buttons[l].GetComponent<RectTransform>();
				var pos = trans.localPosition;
				if (timer > l * 0.1f)
				{
					if (selectId == l) trans.localPosition = new Vector2(pos.x + (50 - pos.x) * 0.1f, pos.y);
					else trans.localPosition = new Vector2(pos.x + (-1000 - pos.x) * 0.1f, pos.y);
				}
			}
			if (timer > 1)
			{
				var image = Buttons[selectId].GetComponent<Image>();
				var col = image.color;
				image.color = new Color(col.r, col.g, col.b, col.a * 0.8f);
				if (col.a < 0.1f)
				{
					state = 0;
					image.color = new Color(col.r, col.g, col.b, 1.0f);
					for (int l = 0; l < selectLength; l++) Buttons[l].SetActive(false);
				}
			}
		}
	}

	public void SetSelect(string[] selects)
	{
		selectLength = selects.Length;
		results = new string[selectLength];
		timer = 0;
		state = 1;

		int y = 100 + selectLength * 25;
		for (int l = 0; l < selectLength; l++)
		{
			string[] sp = selects[l].Split(':');
			Buttons[l].SetActive(true);
			Buttons[l].GetComponent<RectTransform>().localPosition = new Vector2(-1000, y - l * 100);
			Buttons[l].GetComponentInChildren<Text>().text = sp[0];
			results[l] = sp[1];
		}
	}

	public void ClickButton(int id)
	{
		selectId = id;
		timer = 0;
		state = 2;
	}

	public bool IsSelecting()
	{
		return results != null;
	}

	public string RequestSelectResult()
	{
		if (state != 0) return null;	 // まだ選択中
		if (results == null) return null; // もう選択済み
		var ret = results[selectId];
		results = null;
		return ret;
	}
}

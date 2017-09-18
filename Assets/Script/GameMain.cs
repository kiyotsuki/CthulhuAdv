using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour {

	public Text TextBox;
	public SelectBoard SelectBoard;
	public PlayerController Player;
	public Image FadeImage;
	
	string source;
	int index;
	bool tapWait;
	float timer;
	float touchTime;

	int fade;
	float fadePower;
	float fadeSpeed;

	Dictionary<string, int> varMap;
	
	// Use this for initialization
	void Start ()
    {
		source = "";
		timer = index = 0;
		varMap = new Dictionary<string, int>();

	    string opening = (Resources.Load("Text/opening") as TextAsset).text;
        this.SetScript(opening);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!IsRunning())
		{
			Player.SetPlayable(true);
			return;
		}

		float dt = Time.deltaTime;
		timer -= dt;
		bool tap = false;

		if (fade != 0)
		{
			fadePower += dt * 0.3f * fade;
			if (fade < 0 && fadePower < 0)
			{
				fadePower = 0;
				fade = 0;
			}
			if (fade > 0 && fadePower > 1)
			{
				fadePower = 1;
				fade = 0;
			}
			FadeImage.color = new Color(0, 0, 0, fadePower);
			return;
		}

		if (SelectBoard.IsSelecting())
		{
			string label = SelectBoard.RequestSelectResult();
			if (label != null) processGoto(label);
			return;
		}

		if (Input.GetMouseButton(0))
		{
			touchTime += dt;
		}
		else
		{
			tap = (touchTime > 0 && touchTime < 1);
			touchTime = 0;
		}

		if (tapWait)
		{
			if (!tap) return;
			tapWait = tap = false;
			string text = TextBox.text;
			TextBox.text = text.Substring(0, text.Length - 1);
			timer = 0;
		}

		if (timer > 0 && !tap) return;

		bool forceBreak = false;
		while (IsRunning() && !forceBreak)
		{
			char c = source[index];
			if (c == '<')
			{
				int endIndex = source.IndexOf('>', index + 1);
				string tag = source.Substring(index + 1, endIndex - index - 1);
				index = endIndex + 1;
				string[] cmd = tag.Split(' ');
				switch (cmd[0])
				{
					case "return":
						index = source.Length;
						break;

					case "pass":
						timer = int.Parse(cmd[1]) / 1000.0f;
						if (!tap) forceBreak = true;
						break;

					case "br":
						TextBox.text += '\n';
						break;

					case "speaker":
						TextBox.text = "【" + cmd[1] + "】\n";
						break;

					case "tap":
						TextBox.text += "▽";
						tapWait = true;
						forceBreak = true;
						break;

					case "anim":
						int anim = int.Parse(cmd[1]);
						Player.SetAnimation(anim);
						break;

					case "active":
						{
							GameObject room = GameObject.Find("RoomMap");
							GameObject obj = room.transform.Find(cmd[1]).gameObject;
							if (obj) obj.SetActive(true);
						}
						break;

					case "inactive":
						{
							GameObject obj = GameObject.Find(cmd[1]);
							if (obj) obj.SetActive(false);
						}
						break;

					case "var":
						varMap[cmd[1]] = int.Parse(cmd[2]);
						break;

					case "if":
						processIf(cmd[1], cmd[2], cmd[3]);
						break;

					case "goto":
						processGoto(cmd[1]);
						break;

					case "select":
						{
							int len = cmd.Length - 1;
							string[] selects = new string[len];
							for (int l = 0; l < len; l++) selects[l] = cmd[l + 1];
							SelectBoard.SetSelect(selects);
						}
						break;

					case "fadein":
						fade = -1;
						fadePower = 1;
						forceBreak = true;
						break;

					case "fadeout":
						fade = 1;
						fadePower = 0;
						forceBreak = true;
						break;

					case "endif":
					case "label":
						// 何もしない
						break;
				}
			}
			else
			{
				TextBox.text += c;
				index++;
				switch (c)
				{
					case '、':
					case '。':
						timer = 0.5f;
						break;

					default:
						timer = 0.1f;
						break;
				}
				if (!tap) forceBreak = true;
			}
		}
	}

	private void processGoto(string label)
	{
		string labeltag = "<label " + label + ">";
		int labelidx = source.IndexOf(labeltag);
		if (labelidx > 0) index = labelidx + labeltag.Length;
	}

	private void processIf(string key, string operater, string value)
	{
		int v = 0;
		if (varMap.ContainsKey(key)) v = varMap[key];

		bool res = false;
		int i = int.Parse(value);
		switch (operater)
		{
			case "==": res = v == i; break;
			case "!=": res = v != i; break;
			case "<": res = v < i; break;
			case "<=": res = v <= i; break;
			case ">": res = v > i; break;
			case ">=": res = v >= i; break;
		}
		if (!res)
		{
			int offset = index + 1;
			while (true)
			{
				int endidx = source.IndexOf("<endif>", offset);
				int ifidx = source.IndexOf("<if", offset);

				if (ifidx == -1 && endidx == -1) break;
				if (ifidx == -1 || ifidx > endidx)
				{
					index = endidx + "<endif>".Length;
					break;
				}
				else offset = endidx + 1;
			}
		}
	}

	public void SetScript(string text)
    {
		Player.SetPlayable(false);
		Player.SetAnimation(0);
		source = text.Replace("\n", "");
		timer = index = 0;
		tapWait = false;
    }

	public bool IsRunning()
	{
		return source.Length > index;
	}
}

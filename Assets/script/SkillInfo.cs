﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJson;

public class SkillInfo : MonoBehaviour {
	public Text panelName;
	public Text itemName;
	public Text attackType;
	public Text needMp;
	public Text turn;
	public Text needStep;
	public Text itemInfo;
	public JsonObject data;
	public Image icon;
	public Image bg;
	public string type;
	public Button levelupBtn;
	public Text levelUpNeedInfo;
	// Use this for initialization
	void Awake () {
		
		PoolManager.getInstance ().initPoolByType (type,this,1);
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void init(JsonObject jo,JsonObject herodata = null){
		data = jo;
		//if (jo.ContainsKey ("staticdata")) {
		//	jo = jo ["staticdata"] as JsonObject;
		//}
		/// <summary>
		/// 初始化武器信息
		/// </summary>
		/// //技能
		//JsonObject herodata = DataManager.getInstance().heroDicJson[int.Parse(jo["heroId"].ToString())];
		//jo = DataManager.getInstance().skillDicJson[int.Parse(herodata["skill1"].ToString())];
		if (jo.ContainsKey ("icon")) {
			icon.sprite = (Resources.Load(jo["icon"].ToString(), typeof(Sprite)) as Sprite);
		} else {
			icon.sprite = (Resources.Load(jo["id"].ToString(), typeof(Sprite)) as Sprite);
		}
		data ["skillLevel"] = 1;
		icon.SetNativeSize();
		itemName.text = "Lv." + data["skillLevel"].ToString() + " " + jo ["name"].ToString ();
		if (herodata != null) {
			int _demage = int.Parse (jo ["attackDamage"].ToString ());
			float _add = float.Parse (jo ["attackAdd"].ToString ());
			int _equipDamage = DataManager.getInstance ().getHeroDamage (herodata);
			int _skillDamage = _demage + (int)(_add * _equipDamage);
			itemInfo.text = string.Format (jo ["desc"].ToString (),_skillDamage );
		} else {
			itemInfo.text = jo ["desc"].ToString ();
		}
		int attacktype = int.Parse (jo ["attackType"].ToString ());
		int isUseActionStep = int.Parse (jo ["isUseActionStep"].ToString ());
		int damageAdd = (int.Parse (data ["skillLevel"].ToString ()) - 1) * 2;
		attackType.text = DataManager.getInstance ().languageJson[10002+attacktype]["name"].ToString();
		needMp.text = string.Format(DataManager.getInstance ().languageJson[10008]["name"].ToString (),int.Parse(jo["needMP"].ToString ()));
		turn.text = string.Format (DataManager.getInstance ().languageJson[10007]["name"].ToString(),int.Parse(jo["turn"].ToString ()));

		needStep.text = DataManager.getInstance ().languageJson [10009 + isUseActionStep] ["name"].ToString ();
		//itemShuLiang.text = (damage + damageAdd).ToString()  + "%";
		updateBtn(levelupBtn,levelUpNeedInfo,1000,"skillLevel","skillLevelUpNeed","levelUp");	
	}
	public void onClose(){
		PoolManager.getInstance ().addToPool (type,this);
	}
	public void onLevelUp(){
		JsonObject userMessage = new JsonObject();
		userMessage.Add ("heroId", data["heroId"]);
		//userMessage.Add ("heroId", heroId);
		ServerManager.getInstance ().request("area.playerHandler.skillUp", userMessage, (databack)=>{
			Debug.Log(databack.ToString());


		});
	}
	public void updateBtn(Button btn,Text txt,int itemid,string level,string need,string dataDicName){
		JsonObject item = BagManager.getInstance ().getItemByItemId (itemid);
		int nextLevel = int.Parse(data[level].ToString()) + 1;
		if (DataManager.getInstance ().dataDic[dataDicName].ContainsKey (nextLevel)) {

			JsonObject jo8 = DataManager.getInstance ().dataDic[dataDicName][nextLevel];
			int haveNum = 0;
			int needNum = int.Parse (jo8 [need].ToString ());
			if (needNum > 0) {
				btn.gameObject.SetActive (true);
				if (item != null) {
					haveNum = int.Parse (item ["count"].ToString ());
					txt.text = item ["count"].ToString () + "/" + jo8 [need].ToString ();
				} else {
					haveNum = 0;
					txt.text = "0/" + jo8 [need].ToString ();
				}
				if (haveNum < needNum) {
					//btn.interactable = false;
					txt.color = Color.red;
				} else {
					//btn.interactable = true;
					txt.color = Color.white;
				}
			} else {
				btn.gameObject.SetActive (false);
			}

		} else {
			btn.gameObject.SetActive (false);
		}
	}
}

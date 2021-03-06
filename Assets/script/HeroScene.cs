﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TFSG;
using System.Collections.Generic;
using SimpleJson;
using Spine.Unity;
using Spine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public delegate void callBackFunc<JsonObject>(IconBase jo);
public delegate void callBackFunJ<JsonObject>(JsonObject jo);
public delegate void callBack();
public class HeroScene : Observer {
    //public UnityEngine.UI.Image bg;
    // Use this for initialization
	public Transform content;
	public Button heroHeadDemo;
	public Image heroStyle;
	public Text heroBB;
    public Button weapon;
    public Button Armor;
    public Button Shoes;
    public Button Amulet;
    public Button selectKind;

	public Button shengxingBtn;
	public Button shengjiBtn;
    public Text heroAttack;
	//public Text heroAttackRange;
	public Text heroAttackSpeed;
	public Text heroHP;
	public Text heroDefence;
	public Text heroPingFen;
	public Text shengxingNeedInfo;
	public Text shengjiNeedInfo;
	public Image panel;
	public Dictionary<string,Button> equips;

	//public HeroStyle skeletonGraphic;
	public RawImage star1;
	public RawImage star2;
	public RawImage star3;
	public RawImage star4;
	private ArrayList starArr;
	public Button skill1;
	public Button skill2;
	public Button skill3;
	public Button skill4;
	public Text skillName;
	public Vector3 pos1;
	public Text heroName;
	public ArrayList heroHeadList;
	public int heroSharedId = 0;
	public JsonObject data = null;
	private bool isUpdate = false;
	public int heroId = 0;
	//public Image skillInfoPanel;
	public ArrayList equipedList;
	public ArrayList skillList;
	public JsonObject staticdata;
	//public JsonObject data;
	void Awake () {
		messageArr.Add (Message.HERO_UPDATE);
		HeroManager.getInstance().heroscene = this;
	}
	void Start () {
        //屏幕适配,按宽度缩放
		Debug.Log("进入英雄界面");
		equips = new Dictionary<string, Button> ();
		equips ["weapon"] = weapon;
		equips ["armor"] = Armor;
		equips ["shoes"] = Shoes;
		equips ["amulet"] = Amulet;
		equipedList = new ArrayList ();
		starArr = new ArrayList ();
		skillList = new ArrayList ();
		starArr.Add (star1);
		starArr.Add (star2);
		starArr.Add (star3);
		starArr.Add (star4);
		/////////////////////////////////////
		skillList.Add (skill1);
		skillList.Add (skill2);
		skillList.Add (skill3);
		skillList.Add (skill4);
        //content.rect.width = 600;
       

        heroHeadList = new ArrayList();
		Dictionary<int,JsonObject> heroarr = HeroManager.getInstance().getHeros();
		int index = 0;
		foreach(KeyValuePair<int,JsonObject> kvp in heroarr)
        {
			
			addHero (kvp.Value);
			if(index == 0){
				OnChangeHero (kvp.Value);
			}
			index++;
		}
		//HeroStyle.heroarr = HeroManager.getInstance ().getHerosArrayList ();
		//skeletonGraphic.Func = new callBackFunc<JsonObject> (OnChangeHero);
    }
	
	// Update is called once per frame
	void Update () {
		if (notificationQueue.Count > 0) {
			Notification nt = notificationQueue [0];
			notificationQueue.RemoveAt (0);
			switch (nt.name) {
			case Message.HERO_UPDATE:
				{
					isUpdate = false;
					JsonObject _data = (JsonObject)nt.data;
					if (data != null) {
						int updateheroId = int.Parse(_data["heroId"].ToString());
						int curheroId = int.Parse(data["heroId"].ToString());
						if (updateheroId == curheroId) {
							data = _data;
							isUpdate = true;
						}
						//HeroStyle.heroarr = HeroManager.getInstance ().getHerosArrayList ();
					} else {
						data = _data;
						isUpdate = true;
					}
					if (isUpdate) {
						isUpdate = false;
						updateHero (data);
					}
				}
				break;
			}
		}
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
	public void onClickHeroHead(int heroId){
        
		
	}
	public void onClickLevelUp(){
		if (data != null) {
			JsonObject userMessage = new JsonObject();
			userMessage.Add ("heroId", heroId);
			ServerManager.getInstance ().request("area.playerHandler.levelUp", userMessage, (databack)=>{
				Debug.Log(databack.ToString());
			});
		}

	}
	public void onClickStarUp(){//升星
		if (data != null) {
			JsonObject userMessage = new JsonObject();
			userMessage.Add ("heroId", heroId);
			ServerManager.getInstance ().request("area.playerHandler.starUp", userMessage, (databack)=>{
				Debug.Log(databack.ToString());
			});
		}

	}
	public void onEquip(BagPanel bp){//英雄装备武器
		if (data != null) {
			
			JsonObject userMessage = new JsonObject();
			userMessage.Add ("equipId", bp.data["id"]);
			userMessage.Add ("heroId", heroId);
			ServerManager.getInstance ().request("area.equipHandler.equip", userMessage, (databack)=>{
				Debug.Log(databack.ToString());
				AudioManager.instance.playEquip();

			});
			if (ListPanel._currentListPanel != null) {
				ListPanel._currentListPanel.onClickCloseBtn ();
			}
			//BagManager.getInstance ().getGameScene ().onclickBtn (2);

			//selectKind.GetComponent<Image> ().sprite = bp.icon.sprite;
			//bp.transform.SetParent (null);
			//BagManager.getInstance ().addToPool (bp);

		}
	}

	public void onUnEquip(string type){//英雄卸下武器
		if (data != null) {

			JsonObject userMessage = new JsonObject();
			userMessage.Add ("equipType",type);//准备部位
			userMessage.Add ("heroId", heroId);
			ServerManager.getInstance ().request("area.equipHandler.unEquip", userMessage, (databack)=>{
				Debug.Log(databack.ToString());
				AudioManager.instance.playUnEquip();

			});
			//ListPanel _ListPanel = BagManager.getInstance ().getGameScene ().gameObject.GetComponent<ListPanel> ();

			//selectKind.GetComponent<Image> ().sprite = bp.icon.sprite;
			//bp.transform.SetParent (null);
			//BagManager.getInstance ().addToPool (bp);

		}
	}
	public void openBagByType(string type){
		JsonObject equip = BagManager.getInstance ().getEquipByHeroIdAndKind (heroId, type);
		if (equip != null) {//装备了武器
			EquipInfo _equipInfo = (EquipInfo)PoolManager.getInstance().getGameObject(PoolManager.EQUIP_INFO);
			_equipInfo.transform.SetParent (this.transform.parent.transform);
			_equipInfo.transform.localPosition = new Vector3 (0.0f,0.0f,0.0f);
			_equipInfo.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
			_equipInfo.init (equip,1);
			//onUnEquip(type);
		} else {
			//BagManager.getInstance ().showEquipByType (type);
			List<JsonObject> list = BagManager.getInstance ().getEquipByType(type);
			ListPanel _listPanel= (ListPanel)PoolManager.getInstance ().getGameObject (PoolManager.LIST_PANEL);
			_listPanel.transform.SetParent (this.transform.parent.transform);
			_listPanel.transform.localPosition = new Vector3 (0.0f,0.0f,0.0f);
			_listPanel.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
			_listPanel.init (list,null,2);
		}

	}
	public void onCallBack(IconBase jo){
		//Debug.Log (jo.ToString());
		if (jo.data.ContainsKey ("skillType")) {
			SkillInfo skillinfo = (SkillInfo)PoolManager.getInstance().getGameObject(PoolManager.SKILL_INFO);

			skillinfo.init (jo.data,data);
			skillinfo.transform.SetParent (BagManager.getInstance().getGameScene().transform);
			skillinfo.transform.localPosition = new Vector3 (0.0f,0.0f,0.0f);
		} else {
			string key = BagManager.getInstance().getItemStaticData(jo.data) ["kind"].ToString ();
			onClickEquip (key);
		}

	}
	public void onClickEquip(string equipType){
		openBagByType (equipType);
		selectKind = equips[equipType];
	}
    public void onClickBack()
    {
        SceneManager.LoadScene("MainScene");
    }
	public void onClickHead(IconBase d){
		OnChangeHero(d.data);
	}
	public void addHero(JsonObject herodata){
		
		JsonObject staticdata = HeroManager.getInstance ().getHeroStaticData (herodata);
		IconBase icon = (IconBase)PoolManager.getInstance ().getGameObject (staticdata["color"].ToString());
		icon.init (herodata).Func = new callBackFunc<JsonObject> (onClickHead);
		//icon.init (equip).Func = new callBackFunc<JsonObject> (onClickStone);
		icon.transform.SetParent (content);
		//icon.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		icon.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
		Button btn = icon.GetComponent<Button> ();
		/**string name = staticdata ["name"].ToString ();
		if (content.childCount == 0) {
			heroHeadDemo.transform.FindChild ("Text").GetComponent<Text>().text = name;
			heroHeadDemo.transform.SetParent (content.transform);
			btn = heroHeadDemo;
			OnChangeHero(herodata,heroHeadDemo);
		} else {
			btn = (Button)GameObject.Instantiate (heroHeadDemo,heroHeadDemo.transform.position,heroHeadDemo.transform.rotation,heroHeadDemo.transform.parent);
			btn.interactable = true;
			btn.transform.FindChild ("Text").GetComponent<Text>().text = name;

			//btn.transform.SetParent (content.transform);
		}**/
		heroHeadList.Add (btn);
		//btn.onClick.AddListener(delegate() {
		//	JsonObject data = herodata;
		//
		//
		//});

	}
	public void updateHero(JsonObject herodata){
		//if (data == null || herodata.heroId == data.heroId) {
			//data = herodata;
			staticdata = HeroManager.getInstance ().getHeroStaticData (herodata);
			data =herodata;
		for(int i=0;i < equipedList.Count;i++){
			//Button btn = equips [kvp.Key];
			IconBase icon = (IconBase)equipedList[i];
			if (icon != null) {

				PoolManager.getInstance ().addToPool (icon.type, icon);
			}
		}
		equipedList.Clear ();
		//JsonObject staticdata = data ["staticdata"] as JsonObject;
		//JsonObject data = data ["data"] as JsonObject;
		heroId = int.Parse (data ["heroId"].ToString ());
		heroAttack.text = DataManager.getInstance().getHeroDamage(data).ToString();
		//heroAttackRange.text = data["attackRange"].ToString();
		heroAttackSpeed.text = DataManager.getInstance().getHeroAttackSpeed(data).ToString();
		heroHP.text = DataManager.getInstance().getHeroHp(data).ToString();
		heroDefence.text =DataManager.getInstance().getHeroDefence(data).ToString();

		//升星更新
		updateBtn(shengxingBtn,shengxingNeedInfo,heroSharedId,"starLevel","starLevelUpNeed","levelUp");


		//升级更新
		updateBtn(shengjiBtn,shengjiNeedInfo,1000,"level","needExpPoint","levelUp");


		heroPingFen.text = (int.Parse(heroAttack.text) * 8 + int.Parse(heroDefence.text) * 5 + (int.Parse(heroAttackSpeed.text) * 6)).ToString();
		heroBB.text = staticdata["desc"].ToString();
		heroName.text = "Lv." + data["level"].ToString() + " " + staticdata["name"].ToString();
		heroName.color = DataManager.getInstance().getColor(staticdata["color"].ToString());
		ArrayList equipArr = BagManager.getInstance ().getEquipByHeroId (heroId);
		for(int i=0;i < equipArr.Count;i++){	
			JsonObject jo = equipArr [i] as JsonObject;
			JsonObject sdjo =  BagManager.getInstance().getItemStaticData (jo);
			string key = sdjo ["kind"].ToString ();
			if (equips.ContainsKey (key)) {
				Button equip = equips [key];

				//equip.sprite = 
				IconBase icon = (IconBase)PoolManager.getInstance ().getGameObject (sdjo["color"].ToString());
				icon.init (jo).Func = new callBackFunc<JsonObject> (onCallBack);
				//icon.Func = new callBackFunc<JsonObject> (onCallBack);
				icon.transform.SetParent (equip.transform);
				icon.transform.localPosition = Vector3.zero;
				icon.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
				//iTween.ScaleTo(icon.gameObject, iTween.Hash("y", 0.5f,"x", 0.5f,"z", 0.5f ,"delay", 0.0f,"time",0.5f));
				equip.gameObject.SetActive (true);
				//(icon.GetComponent<Button>()).interactable = false;
				equipedList.Add (icon);
			}
		}
		int starLevel = int.Parse (data ["starLevel"].ToString ());
		for(int i = 0;i < starLevel;i++){
			RawImage star = (RawImage)starArr [i];
			star.gameObject.SetActive (false);
			//Destroy (star);
		}
		//isUpdate = false;
		//}


	}
	public void OnClick(){
		//if (type == 1) {
		//}
		SkillInfo skillinfo = (SkillInfo)PoolManager.getInstance().getGameObject(PoolManager.SKILL_INFO);

		skillinfo.init (data);
		skillinfo.transform.SetParent (BagManager.getInstance().getGameScene().transform);
		skillinfo.transform.localPosition = new Vector3 (0.0f,0.0f,0.0f);

	}
	public void OnClickUp(BaseEventData eventData){
		

	}
	public void fresh(){
		OnChangeHero (data);
	}
	public void OnChangeHero(JsonObject herodata){
		if (herodata == null || heroHeadList == null)
			return;
        selectKind = null;
		staticdata = HeroManager.getInstance ().getHeroStaticData (herodata);
		data = herodata;
		heroStyle.sprite = Resources.Load("hero/" + staticdata["style"].ToString(),typeof(Sprite)) as Sprite;
		heroStyle.SetNativeSize ();

		for (int i = 0; i < heroHeadList.Count; i++) {
			Button btn2 = (Button)heroHeadList[i];
			btn2.interactable = true;
		}
		//if (skeletonAnimation != null && skeletonAnimation.isActiveAndEnabled) {
		//	skeletonAnimation.transform.parent = null;
		//	skeletonAnimation.gameObject.SetActive (false);
		//}
		//skeletonGraphic.init(herodata);

		//skeletonGraphic.startingAnimation = "attack";
		//btn.interactable = false;
		//技能
		for (int i = 1; i <= 4; i++) {
			JsonObject skilldata = DataManager.getInstance().skillDicJson[int.Parse(staticdata["skill" + i.ToString()].ToString())];
			//skillName.text = skilldata["name"].ToString();
			Button skill = (Button)skillList[i-1];
			skill.image.sprite = Resources.Load(skilldata["icon"].ToString(),typeof(Sprite)) as Sprite;
			skill.image.SetNativeSize ();
			IconBase icon = (IconBase)PoolManager.getInstance ().getGameObject (skilldata["color"].ToString());
			icon.init (skilldata).Func = new callBackFunc<JsonObject> (onCallBack);
			//icon.Func = new callBackFunc<JsonObject> (onCallBack);
			icon.transform.SetParent (skill.transform);
			icon.transform.localPosition = Vector3.zero;
			icon.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
			//iTween.ScaleTo(icon.gameObject, iTween.Hash("y", 0.5f,"x", 0.5f,"z", 0.5f ,"delay", 0.0f,"time",0.5f));
			//skill.gameObject.SetActive (true);
		}


		for(int i = 0;i < starArr.Count;i++){
			RawImage star = (RawImage)starArr [i];
			star.gameObject.SetActive (true);
			//Destroy (star);
		}

		heroSharedId = int.Parse(staticdata["heroSharedId"].ToString());
		updateHero (herodata);
		//yield return isUpdate;

	}
}

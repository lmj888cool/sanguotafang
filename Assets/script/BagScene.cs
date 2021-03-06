﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJson;

public class BagScene : Observer {
	public Transform content;
	public BagPanel _bagpanel;
	public Button equipTab;
	public Button bagTab;
	public Button heroShardTab;
	public Button equipShardTab;
	public Button stoneTab;
	public int curType;
	void Awake(){
		messageArr.Add (Message.BAG_ADD);
		BagManager.getInstance ()._bagScene = this;
	}
	// Use this for initialization
	void Start () {
		
		//BagManager.getInstance ().showAll ();

    }
	
	// Update is called once per frame
	void Update () {
		if (notificationQueue.Count > 0) {
			Notification nt = notificationQueue [0];
			notificationQueue.RemoveAt (0);
			switch (nt.name) {
			case Message.BAG_ADD:
				{
					JsonObject _data = (JsonObject)nt.data;
					if (curType == int.Parse(_data ["itemType"].ToString ())) {
						add (_data);
					}
				}
				break;
			}
		}
	}
	public void reflesh(){
	}
	public void add(JsonObject cd,int openType = 0)
	{
		JsonObject sd = BagManager.getInstance().getItemStaticData(cd);;
		BagPanel bagItem = (BagPanel)PoolManager.getInstance ().getGameObject (PoolManager.BAG_ITEM + sd["color"].ToString());

		bagItem.transform.SetParent(content);
		bagItem.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		bagItem.init(cd,openType);
		BagManager.getInstance ().addItem (bagItem);
		/**
		//Button btn;
		if (content.childCount == 0)
		{
			BagPanel._demoPanel.transform.SetParent(content);
			BagPanel._demoPanel.init(cd);
			//OnChangeHero(cd,heroHeadDemo);
		}
		else
		{
			BagPanel panel = (BagPanel)GameObject.Instantiate(BagPanel._demoPanel, BagPanel._demoPanel.transform.position, BagPanel._demoPanel.transform.rotation, BagPanel._demoPanel.transform.parent);
			//btn.interactable = true;
			panel.init(cd);

			//btn.transform.SetParent (content.transform);
		}
		//heroHeadList.Add (btn);
		//btn.onClick.AddListener(delegate () {
		//    this.OnChangeHero(cd, btn);

		//});
		**/
	}
	void showPanel(int _type,Button btn){
		//if (btn.interactable)
        {
			curType = _type;
            if (btn != bagTab) {bagTab.interactable = true; }
			if (btn != equipTab) {equipTab.interactable = true; }
			if (btn != heroShardTab) {heroShardTab.interactable = true; }
			if (btn != equipShardTab) {equipShardTab.interactable = true; }
			if (btn != stoneTab) {stoneTab.interactable = true; }
            btn.interactable = false;
			BagManager.getInstance ().showItemByType (_type.ToString(),1);
        }
		
	}
	public void onclickBtn(int type){
		switch (type) {
		case 1:
			showPanel (IconBase.ITEM_TYPE_EXP,bagTab);
			break;
		case 2:
			showPanel (IconBase.ITEM_TYPE_EQUIP,equipTab);
			break;
		case 3:
			showPanel (IconBase.ITEM_TYPE_HEROSUB,heroShardTab);//英雄碎片
			break;
		case 4:
			showPanel (IconBase.ITEM_TYPE_EQUIPSUB,equipShardTab);//碎片
			break;
		case 5:
			showPanel (IconBase.ITEM_TYPE_STONE,stoneTab);//宝石
			break;
		default:
			break;
		}
	}
}

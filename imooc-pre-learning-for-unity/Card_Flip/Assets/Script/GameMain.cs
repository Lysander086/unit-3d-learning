using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour {

	public Button btn_level1;
	public Button btn_level2;
	public Button btn_level3;

	public Button btn_over;
	public Button btn_restart;

	public Transform panelStart;
	public Transform panelCard;
	public Transform panelOver;

	// Use this for initialization
	void Start () {
		btn_level1.onClick.AddListener (() => {
			panelStart.gameObject.SetActive(false);
			panelCard.gameObject.SetActive(true);
			LoadLevelCard(2, 3);
		});	
		btn_level2.onClick.AddListener (() => {
			panelStart.gameObject.SetActive(false);
			panelCard.gameObject.SetActive(true);
			LoadLevelCard(2, 4);
		});	
		btn_level3.onClick.AddListener (() => {
			panelStart.gameObject.SetActive(false);
			panelCard.gameObject.SetActive(true);
			LoadLevelCard(2, 5);
		});	
		btn_over.onClick.AddListener (() => {
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		});
		btn_restart.onClick.AddListener (() => {
			panelStart.gameObject.SetActive(true);
			panelCard.gameObject.SetActive(false);
			panelOver.gameObject.SetActive(false);
		});
	}

	void LoadLevelCard(int row, int col)
	{
		//加载所有卡牌图片
		Sprite[] sps = Resources.LoadAll<Sprite>("");

		int totalCount = row * col / 2;
		List<Sprite> spsList = new List<Sprite>();
		for (int i = 0; i < sps.Length; ++i) {
			spsList.Add(sps[i]);
		}

		//通过随机找到需要显示的卡牌图片
		List<Sprite> needShowCardList = new List<Sprite>();
		while (totalCount > 0) {
			int radom = Random.Range(0, spsList.Count);
			needShowCardList.Add (spsList [radom]);
			needShowCardList.Add (spsList [radom]);
			spsList.RemoveAt (radom);
			totalCount--;
		}

		Transform contentRoot = panelCard.Find ("Panel");
		GameObject itemTemplate = contentRoot.GetChild (0).gameObject;

		//高等级通关后在玩低等级要销毁对象，且解除关联
		for (int i = 1; i < contentRoot.childCount; ++i) {
			GameObject itemTemp = contentRoot.GetChild (i).gameObject;
			Sprite ss = itemTemp.transform.Find ("Image_front").GetComponent<Image> ().sprite;
			Debug.Log (i + "," + ss.name);
			itemTemp.transform.SetParent (null);
			Destroy (itemTemp);
		}
			
		//for (int i = 0; i < needShowCardList.Count; ++i) {
		int normal_index = 0;
		while(needShowCardList.Count > 0){
			int index = Random.Range(0, needShowCardList.Count);
			GameObject itemObject = null;
			if (normal_index < contentRoot.childCount) {
				itemObject = contentRoot.GetChild (normal_index).gameObject;
			} else {
				itemObject = GameObject.Instantiate<GameObject> (itemTemplate);
				itemObject.transform.SetParent (contentRoot, false);
			}
			itemObject.transform.Find("Image_front").GetComponent<Image> ().sprite = needShowCardList [index];
			CardFlipAnimation cardAnimal = itemObject.GetComponent<CardFlipAnimation> ();
			cardAnimal.SetDefaultState ();

			needShowCardList.RemoveAt (index);
			++normal_index;
		}
		GridLayoutGroup glg = contentRoot.GetComponent<GridLayoutGroup> ();
		float panelWidth = col * glg.cellSize.x + glg.padding.left + glg.padding.right + (col - 1) * glg.spacing.x;
		float panelHeight = row * glg.cellSize.y + glg.padding.top + glg.padding.bottom + (row - 1) * glg.spacing.y;
		contentRoot.GetComponent<RectTransform> ().sizeDelta = new Vector2 (panelWidth, panelHeight);
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void checkGameOver()
	{
		CardFlipAnimation[] allCards = GameObject.FindObjectsOfType<CardFlipAnimation> ();
		if (allCards != null && allCards.Length > 0) {
			List<CardFlipAnimation> cardInFront = new List<CardFlipAnimation> ();
			for (int i = 0; i < allCards.Length; ++i) {
				CardFlipAnimation cardTemp = allCards [i];
				if (cardTemp.isFront && !cardTemp.isOver) {
					cardInFront.Add (cardTemp);
				}

				if (cardInFront.Count >= 2) {
					
					string cardImageName1 = cardInFront[0].GetCardImageName ();
					string cardImageName2 = cardInFront[1].GetCardImageName ();
					if (cardImageName1 == cardImageName2) {
						cardInFront [0].MatchSuccess ();
						cardInFront [1].MatchSuccess ();

					} else {
						cardInFront [0].MatchFailed ();
						cardInFront [1].MatchFailed ();
					}

					allCards = GameObject.FindObjectsOfType<CardFlipAnimation> ();
					bool AllOver = true;
					for(int j = 0; j < allCards.Length;++j){
						if (!allCards [j].isOver) {
							AllOver = false;
						}
					}
					if (AllOver) {
						ToGameOverPage ();
					}

					break;
				}
			}
		}
	}

	public void ToGameOverPage()
	{
		panelStart.gameObject.SetActive(false);
		panelCard.gameObject.SetActive(false);
		panelOver.gameObject.SetActive(true);
	}
}

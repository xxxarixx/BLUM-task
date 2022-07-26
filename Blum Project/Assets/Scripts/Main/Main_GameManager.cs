using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
/// <summary>
/// game centrelizesion it hold some key references and hold item list
/// </summary>
public class Main_GameManager : MonoBehaviour
{
    public static Main_GameManager instance;
    
    
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private GameObject dropItemPrefab;
    
    private int coinCount;
    public delegate void collectedItemDel(Item item, Vector3 position);
    public event collectedItemDel OnCollectedItem;

    [System.Serializable]
    public class Item 
    {
        public string name;
        public int id;
        public Sprite sprite;
        public Player_Weapon weapon;
        public AnimationClip animation;
        public List<GameObject> spawnOnCollect = new List<GameObject>();
    }
    private void _SetupItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            item.id = i;
        }
    }
    public Item GetItemFromID(int id)
    {
        return items[id];
    }
    public Item GetItemFromName(string name)
    {
        return items.Find(x => x.name.ToLower() == name.ToLower());
    }
    public void DropItem(int id, Vector3 position, int count = 1, float dropTime = 0f)
    {
        StartCoroutine(_spawnDropItem(GetItemFromID(id),position,count, dropTime));
    }
    public void DropItem(string name, Vector3 position, int count = 1, float dropTime = 0f)
    {
        StartCoroutine(_spawnDropItem(GetItemFromName(name), position, count, dropTime));
    }
    public void AddItemToInventory(int id,Vector3 position)
    {
        //if it is coin then automaticlly move to player pocket
        if (id == 0)
        {
            //coin
            coinCount++;
            Main_UiController.instance.coinCountText.SetText(GetCoinText());
        }
        var item = items[id];
        OnCollectedItem?.Invoke(item,position);
        foreach (var itemspawn in item.spawnOnCollect)
        {
            Instantiate(itemspawn, position, Quaternion.identity);
        }
        //Debug.Log(item.name);
        if (item.weapon != null)
        {
            //add weapon to player
            Player_References.instance.attack.weapon = item.weapon;
            Debug.Log("Changed weapon");
        }
    }
    public void AddItemToInventory(int id)
    {
        //if it is coin then automaticlly move to player pocket
        if (id == 0)
        {
            //coin
            coinCount++;
            Main_UiController.instance.coinCountText.SetText(GetCoinText());
        }
        var item = items[id];
        OnCollectedItem?.Invoke(item, Vector3.zero);
        foreach (var itemspawn in item.spawnOnCollect)
        {
            Instantiate(itemspawn, Vector3.zero, Quaternion.identity);
        }
        //Debug.Log(item.name);
        if (item.weapon != null)
        {
            //add weapon to player
            Player_References.instance.attack.weapon = item.weapon;
            Debug.Log("Changed weapon");
        }
    }
    private IEnumerator _spawnDropItem(Item item, Vector3 position, int count = 1, float dropTime = 0f)
    {
        while (count > 0)
        {
            var spawnedDropItem = Instantiate(dropItemPrefab.gameObject, position, Quaternion.identity);
            var dropItemScript = spawnedDropItem.GetComponent<Items_DropItemPrefab>();
            dropItemScript.sprend.sprite = item.sprite;
            dropItemScript.itemID = item.id;
            dropItemScript.SetAnimation(item);
            count--;
            if(dropTime > 0f)yield return new WaitForSeconds(dropTime / count);
        }
        yield return null;
    }
    private void Awake()
    {
        instance = this;
        _SetupCoinCount();
        _SetupItems();
    }
    private void OnDrawGizmosSelected()
    {
        _SetupItems();
    }
    private void _SetupCoinCount()
    {
        coinCount = 0;
    }
    private string GetCoinText()
    {
        if(coinCount < 10)
        {
            return $"00{coinCount}";
        }
        else if(coinCount >= 10)
        {
            return $"0{coinCount}";
        }
        else if(coinCount >= 100)
        {
            return $"{coinCount}";
        }
        return coinCount.ToString();
    }
    public void SpawnDamagePopup(Vector3 _position, float _damageDealt)
    {
        var spawnedDamagePopup = Instantiate(Main_UiController.instance.ui_DamagePopup, Main_UiController.instance.canvas_World.transform);
        float positionRandomRange = 0.3f;
        //set damage popup small randomness to spawn position for better visual appeal
        spawnedDamagePopup.transform.position = _position + new Vector3(Random.Range(-positionRandomRange, positionRandomRange), Random.Range(-positionRandomRange, positionRandomRange), 0f);
        var damagePopupText = spawnedDamagePopup.GetComponent<TextMeshProUGUI>();
        damagePopupText.text = _damageDealt.ToString();
    }
}

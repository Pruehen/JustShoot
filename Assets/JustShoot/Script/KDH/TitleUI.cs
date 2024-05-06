using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public GameObject[] weaponUIs;

    public List<int> selectWeapons = new List<int>();
    public int nowWeapon = 0;

    private void Awake()
    {
        for (int i = 0; i < weaponUIs.Length; i++)
        {
            weaponUIs[i].SetActive(false);
        }
        weaponUIs[nowWeapon].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnLeftButtonClick()
    {
        weaponUIs[nowWeapon].SetActive(false);
        if (nowWeapon > 0)
            weaponUIs[--nowWeapon].SetActive(true);
        else
        {
            nowWeapon = weaponUIs.Length - 1;
            weaponUIs[nowWeapon].SetActive(true);
        }
    }
    public void OnRightButtonClick()
    {
        weaponUIs[nowWeapon].SetActive(false);
        if (nowWeapon < weaponUIs.Length - 1)
            weaponUIs[++nowWeapon].SetActive(true);
        else
        {
            nowWeapon = 0;
            weaponUIs[nowWeapon].SetActive(true);
        }
    }
    public void OnSelectButtonClick()
    {
        bool isAdd = true;
        if(selectWeapons.Count > 0)
        {
            for (int i = 0; i < selectWeapons.Count; i++)
            {
                if (nowWeapon == selectWeapons[i])
                {
                    isAdd = false;
                }
            }
        }
        if(isAdd && selectWeapons.Count < 3)
        {
            selectWeapons.Add(nowWeapon);
        }
        else
        {
            Debug.Log("추가 불가 : 같은 무기거나, 3개 다 고름");
        }   
    }
    public void OnConfirmButtonClick()
    {
        if (selectWeapons.Count == 3)
        {
            Player.Instance.WeaponSelect(selectWeapons[0], selectWeapons[1], selectWeapons[2]);
            gameObject.SetActive(false);
        }
    }
}

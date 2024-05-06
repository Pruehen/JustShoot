using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public GameObject[] weaponUIs;

    List<int> selectWeapons = new List<int>();
    public int nowWeapon = 0;

    private void Awake()
    {
        for (int i = 0; i < weaponUIs.Length; i++)
        {
            weaponUIs[i].SetActive(false);
        }
        weaponUIs[nowWeapon].SetActive(true);
    }
    void Start()
    {

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
}

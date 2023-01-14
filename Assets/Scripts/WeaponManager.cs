using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform[] weapons;
    int currentWeaponIndex = 0,nextWeaponIndex=0,previousWeaponIndex=0;
    int i = 0;
    bool inSwitching = false;
    void Awake()
    {
        weapons = new Transform[transform.childCount];
        foreach(Transform weapon in transform)
        {
            weapons[i] = weapon;
            i++;
            weapon.gameObject.SetActive(false);
        }
        weapons[currentWeaponIndex].gameObject.SetActive(true);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!inSwitching)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (currentWeaponIndex >= weapons.Length - 1)
                {
                    nextWeaponIndex = 0;
                }
                else
                {
                    nextWeaponIndex = currentWeaponIndex + 1;
                }
                StartCoroutine(SwitchWeapons(nextWeaponIndex));
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (currentWeaponIndex <= 0)
                {
                    previousWeaponIndex = weapons.Length - 1;
                }
                else
                {
                    previousWeaponIndex = currentWeaponIndex - 1;
                }
                StartCoroutine(SwitchWeapons(previousWeaponIndex));
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) && weapons.Length >= 1)
            {
                StartCoroutine(SwitchWeapons(0));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Length >= 2)
            {
                StartCoroutine(SwitchWeapons(1));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Length >= 3)
            {
                StartCoroutine(SwitchWeapons(2));
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && weapons.Length >= 4)
            {
                StartCoroutine(SwitchWeapons(3));
            }
        }


    }
    private IEnumerator SwitchWeapons(int selectedWeaponIndex)
    {
        inSwitching = true;
        WeaponShooter shooter = weapons[currentWeaponIndex].GetComponent<WeaponShooter>();
        shooter.UnWield();
        yield return new WaitForSeconds(1);
        weapons[currentWeaponIndex].gameObject.SetActive(false);
        weapons[selectedWeaponIndex].gameObject.SetActive(true);
        currentWeaponIndex = selectedWeaponIndex;
        inSwitching = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using GGJ2020.Game;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private LayerMask buttonLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask slotLayer;
    
    private bool cursor = false;
    private Vector3 startPosition;
    
    private Vector3 targetPos;
    private float smoothing = 20;

    void Awake()
    {
        startPosition = transform.position;
        targetPos = transform.position;
    }

    void Update()
    {
        bool hoverButton = false;
        MenuOption hoveredOption = null;

        RaycastHit hit;
        if (RaycastUtils.RaycastMouse(out hit, buttonLayer))
        {
            if (hit.transform.gameObject == gameObject)
            {
                hoverButton = true;
            }
        }

        if (hoverButton && Input.GetMouseButtonDown(0))
        {
            cursor = true;
        }

        if (cursor)
        {
            RaycastHit groundHit;
            if (RaycastUtils.RaycastMouse(out groundHit, groundLayer))
            {
                targetPos = groundHit.point;
            }
            
            RaycastHit slotHit;
            if (RaycastUtils.RaycastMouse(out slotHit, slotLayer))
            {
                hoveredOption = slotHit.transform.gameObject.GetComponent<MenuOption>();
            }

            if (Input.GetMouseButtonUp(0))
            {
                cursor = false;

                if (hoveredOption != null)
                {
                    targetPos = hoveredOption.transform.position;
                    StartCoroutine(COnSelect(hoveredOption));

                }
                else
                {
                    targetPos = startPosition;
                }
            }
        }

        transform.position = transform.position + (targetPos - transform.position) * Mathf.Min(1, Time.deltaTime * smoothing);
    }

    IEnumerator COnSelect(MenuOption menuOption)
    {
        foreach (MenuOption otherOption in FindObjectsOfType<MenuOption>())
        {
            if (otherOption != menuOption)
            {
                otherOption.targetPos = new Vector3(otherOption.transform.position.x, otherOption.transform.position.y - 0.1f, otherOption.transform.position.z);
            }
        }
        menuOption.targetPos = new Vector3(menuOption.transform.position.x, menuOption.transform.position.y + 0.5f, menuOption.transform.position.z);
        yield return new WaitForSeconds(0.5f);
        menuOption.onSelect.Invoke();
    }
}
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SelectOnHover))]
public class DropdownHelper : TMP_Dropdown
{
    private SelectOnHover _selectOnHover;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        _selectOnHover = GetComponent<SelectOnHover>();
    }

    protected override GameObject CreateDropdownList(GameObject template)
    {
        GameObject dropdownList = base.CreateDropdownList(template);
        _selectOnHover.SetEnabled(false);
        return dropdownList;
    }

    protected override void DestroyDropdownList(GameObject dropdownList)
    {
        _selectOnHover.SetEnabled(true);
        base.DestroyDropdownList(dropdownList);
    }
}

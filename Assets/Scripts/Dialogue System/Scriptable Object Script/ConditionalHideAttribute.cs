using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionField;

    public ConditionalHideAttribute(string conditionField)
    {
        ConditionField = conditionField;
    }
}

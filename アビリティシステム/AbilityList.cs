using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityList", menuName = "Ability/Create Ability List")]
public class AbilityList : ScriptableObject {
    public List<Ability> abilityList;
}

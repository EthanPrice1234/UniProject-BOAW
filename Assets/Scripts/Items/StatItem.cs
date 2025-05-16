using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EP {
    [CreateAssetMenu(menuName = "Items/Stat Item")]

    public class StatItem : Item
    {
        public GameObject modelPrefab;
        public string itemDescription;
    }
}

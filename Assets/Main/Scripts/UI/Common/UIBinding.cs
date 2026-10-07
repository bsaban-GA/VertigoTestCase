using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.UI
{
    public static class UIBinding
    {
        private static void BindChild<T>(this Component owner, ref T field, string childName) where T : Component
        {
            if (field != null && field.name == childName)
                return;
            
            var child = FindDeep(owner.transform, childName);
            field = child != null ? child.GetComponent<T>() : null;
            
            if(field == null)
                Debug.LogWarning($"{owner.name}: no {typeof(T).Name} named '{childName}' in its children.", owner);
        }
        
        private static Transform FindDeep(Transform parent, string childname)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childname)
                    return child;
                
                var found = FindDeep(child, childname);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}
